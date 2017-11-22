using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Document;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;

namespace WEP.GSP.Data
{
    public class RequestData
    {
        private readonly Database _db;
        private readonly string connString;
        public RequestData()
        {
            var dbFactory = new DatabaseProviderFactory();
            connString = Constants.GSPConnect;
            _db = dbFactory.Create(connString);
        }

        public bool InsertAuditLog(string message)
        {
            
            if (_db.ExecuteNonQuery("GSP_Ins_AuditLog"
                                                   ,message
                                                   ) > 0)
                return true;
            else
                return false;

        }

        public void InsertRequestAsync(string reqjson, string token, string response, int stage,string actualTime)
        {
            Task.Factory.StartNew(() => InsertRequest(reqjson, token, response, stage, actualTime))
                .ContinueWith(p =>
                {
                    if (p.Exception != null)
                        p.Exception.Handle(x =>
                        {
                            new ExceptionData().InsertExceptionLog(token, x.Message, x.StackTrace, stage);
                            return false;
                        });
                });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqjson"></param>
        /// <param name="requestToken"></param>
        /// <param name="response"></param>
        /// <param name="stage"></param>
        /// <returns></returns>
        public bool InsertRequest(string reqjson, string requestToken, string response, int stage,string actualTime)
        {
            if (_db.ExecuteNonQuery(Constants.SPName.GSP_Create_RequestTrackLog  
                                                   , requestToken
                                                   , reqjson
                                                   , response
                                                   , stage
                                                   , actualTime
                                                   ) >0)
                  return true;
            else
                return false;
           
        }

       

        public bool InsertRequest(string response, int stage)
        {
            if (_db.ExecuteNonQuery(Constants.SPName.GSP_Create_RequestDehydrationLog
                                                   , response
                                                   , stage
                                                   ) > 0)
                return true;
            else
                return false;

        }

        public bool InsertRequest(string Exception, string response, int stage)
        {
            if (_db.ExecuteNonQuery(Constants.SPName.GSP_Create_Exception
                                                   , Exception
                                                   , response
                                                   , stage
                                                   ) > 0)
                return true;
            else
                return false;

        }



        #region InsertCircuitBreaker
        /// <summary>
        /// InsertCircuitBreaker
        /// </summary>
        /// <param name="RequestToken"></param>
        /// <param name="Username"></param>
        /// <param name="Tempjsondata"></param>
        /// <returns></returns>
        public bool InsertBacklogRequest(string RequestToken, string Username, string Tempjsondata)
        {
            if (_db.ExecuteNonQuery(Constants.SPName.GSP_INS_BacklogRequest
                                                   , RequestToken
                                                   , Tempjsondata
                                                   , Username
                                                   ) > 0)
                return true;
            else
                return false;
        }
        #endregion

        #region GetPendingRequest
        /// <summary>
        /// GetPendingRequest
        /// </summary>
        /// <returns></returns>
        public List<BacklogRequest> GetPendingRequest()
        {
            List<BacklogRequest> lstCB = new List<BacklogRequest>();
           
                using (IDataReader dr = _db.ExecuteReader(Constants.SPName.Get_GSTN_Request_CircuitBreaker_List))
                {
                    while (dr.Read())
                    {
                        lstCB.Add(new BacklogRequest()
                        {
                            RequestToken = Convert.ToString(dr["ReqToken"]),
                            Data = Convert.ToString(dr["ReqData"]),

                        });

                    }
                    if (!dr.IsClosed)
                        dr.Close();
                    return lstCB;
                }
           
            
        }
        #endregion

        #region UpdateResendStatus
        /// <summary>
        /// UpdateResendStatus
        /// </summary>
        /// <param name="reqtoken"></param>
        public void UpdateResendStatus(string reqtoken)
        {
            _db.ExecuteNonQuery(Constants.SPName.Update_GSTN_Request_CircuitBreaker, reqtoken);
        }
        #endregion

        //#region InsertResponseLog
        ///// <summary>
        ///// InsertResponseLog
        ///// </summary>
        ///// <param name="results"></param>
        //public void InsertResponseLog(GstnResponse results)
        //{
        //    if (results.status_cd ==1)
        //    {
        //        _db.ExecuteNonQuery(Constants.SPName.GSTN_Response_Log
        //                                           , results.username
        //                                           , results.reqtoken
        //                                           , results.status_cd
        //                                           , null
        //                                           , null
        //                                           ,results.apiAction
        //                                           ,results.blobUrl
        //                                           );
        //    }
        //    else if (results.status_cd == 0)
        //    {
        //        _db.ExecuteNonQuery(Constants.SPName.GSTN_Response_Log
        //                                           , results.username
        //                                           , results.reqtoken
        //                                           , results.status_cd
        //                                           , results.error.message
        //                                           , results.error.error_cd
        //                                           , results.apiAction
        //                                           , results.blobUrl
        //                                           );
        //    }

        //}
        //#endregion

        #region InsertResponseLog
        /// <summary>
        /// InsertResponseLog
        /// </summary>
        /// <param name="results"></param>
        public void InsertResponseLog(GstnResponse results)
        {
            
                _db.ExecuteNonQuery(Constants.SPName.GSTN_Response_Log
                                                   , results.username
                                                   , results.reqtoken
                                                   , results.status_cd
                                                   , results.apiAction
                                                   , results.blobUrl
                                                   ,results.respBlobId
                                                   );            

        }
        #endregion

        public async Task InsertStageTrack(string jsonReqst, string token, string  response, int stage)
        {
            using (var cmd = _db.GetStoredProcCommand(Constants.SPName.GSP_Create_RequestTrackLog, token, jsonReqst, response, stage))
            {
                await cmd.ExecuteNonQueryAsync();
            }

        }

        public void UpdateBlobPathForGstnResponse(string blobPath, string requestToken)
        {
            _db.ExecuteNonQuery(Constants.SPName.Update_BlobPath_By_ReqToken,blobPath, requestToken);
        }

        public async Task InsertExceptionLogAync(string token, string message, string source, int stage)
        {
            using (var cmd = _db.GetStoredProcCommand(Constants.SPName.GSP_Create_RequestTrackLog, token, message, source, stage))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public string RetrievePartitionOffset(string PartitionId)
        {
          return  Convert.ToString(_db.ExecuteScalar(Constants.SPName.Retrieve_Partition_Offset , PartitionId));
        }

        public bool UpdatePartitionOffset(string PartitionId, string Offset)
        {
            if (_db.ExecuteNonQuery(Constants.SPName.Update_Partition_Offset
                                                   , PartitionId
                                                   , Offset
                                                   ) > 0)
                return true;
            else
                return false;
        }

        public bool UpdateReleasePartitionLock(string PartitionId)
        {
            if (_db.ExecuteNonQuery(Constants.SPName.Update_Release_Partition_Lock
                                                   , PartitionId
                                                   ) > 0)
                return true;
            else
                return false;
        }
        public bool UpdateReleasePartitionLockOfInstance(string PartitionId)
        {
            if (_db.ExecuteNonQuery(Constants.SPName.Update_Release_Partition_Lock_Instance
                                                   , PartitionId
                                                   ) > 0)
                return true;
            else
                return false;
        }
        public List<string> GetInstancePartitions(string InstanceName)
        {
            List<string> lst = new List<string>();

            using (IDataReader dr = _db.ExecuteReader(Constants.SPName.Retrive_Instance_PartitionId, InstanceName))
            {
                while (dr.Read())
                {
                    lst.Add(Convert.ToString(dr["PartitionId"]));
                }
                if (!dr.IsClosed)
                    dr.Close();
                return lst;
            }
        }
    }
}
