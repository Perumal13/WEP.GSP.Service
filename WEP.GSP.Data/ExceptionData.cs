using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Document;

namespace WEP.GSP.Data
{
    public class ExceptionData
    {
        private readonly Database _db;
        public ExceptionData()
        {
            var dbFactory = new DatabaseProviderFactory();
            _db = dbFactory.Create(Constants.GSPConnect);
        }

        public bool InsertException(string Exception, string StackTrace, string Source, int stage)
        {
            if (_db.ExecuteNonQuery(Constants.SPName.GSP_Create_Exception
                                                   , Exception
                                                   , StackTrace
                                                   , Source
                                                   , stage
                                                   ) > 0)
                return true;
            else
                return false;

        }

        #region InsertExceptionLog
        /// <summary>
        /// InsertExceptionLog
        /// </summary>
        /// <param name="RequestToken"></param>
        /// <param name="ExceptionMessage"></param>
        /// <param name="Source"></param>
        /// <param name="stage"></param>
        /// <returns></returns>
        public bool InsertExceptionLog(string RequestToken, string ExceptionMessage, string Source, int stage)
        {
            try
            {
                if (_db.ExecuteNonQuery(Constants.SPName.GSP_Insert_Exception_Log
                                                   , RequestToken
                                                   , stage
                                                   , ExceptionMessage
                                                   , Source
                                                   ) > 0)
                    return true;
                else
                    return false;
            }

            catch (Exception ex)
            {
                new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey
                                                        , Constants.RowKey
                                                        , RequestToken
                                                        , stage
                                                        , ExceptionMessage
                                                        , ex.Message
                                                        , ex.StackTrace
                                                        , Constants.GSTNException
                                                        , Constants.currentTime
                                                       ).InsertToExTableStorage(ex.Message);
            }
            return false;
        }
        #endregion

    }
}
