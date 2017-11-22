using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Document;

namespace WEP.GSP.Data
{
    public class ConfigData
    {

        private readonly Database _db;
        public ConfigData()
        {
            var dbFactory = new DatabaseProviderFactory();
            _db = dbFactory.Create(Constants.GSPConnect);
        }

        public Blob GetBlobDetail(int Id)
        {
            using (IDataReader dr = _db.ExecuteReader(Constants.SPName.Gsp_Get_BlobDetail,Id))
            {
                while (dr.Read())
                {
                    return new Blob
                        {
                            Id = (int)dr["Id"],
                            AcountName = (string)dr["Account"],
                            Keys = (string)dr["Key"],
                            Connection = (string)dr["Connection"]
                        };                         
                }
            }
            return null;
        }

        public Dictionary <int, Blob> GetGstnReqBlob()
        {
            bool isRequest = true;
            var blobDict = new Dictionary<int,Blob>();
            using (IDataReader dr = _db.ExecuteReader(Constants.SPName.Gsp_Get_Blob, isRequest))
            {
                while (dr.Read())
                {
                    blobDict.Add((int)dr["Id"], new Blob { Id = (int) dr["Id"],
                                           AcountName =(string)dr["Account"],
                                           Keys = (string)dr["Key"],
                                           Connection = (string)dr["Connection"]
                    }
                                );
                }                
            }

            return blobDict;
        }

        public Dictionary<int, Blob> GetGStnRespBlob()
        {
            bool isRequest = false;
            var blobGstnDict = new Dictionary<int, Blob>();
            using (IDataReader dr = _db.ExecuteReader(Constants.SPName.Gsp_Get_Blob, isRequest))
            {
                while (dr.Read())
                {
                    blobGstnDict.Add((int)dr["Id"], new Blob
                    {
                        Id = (int)dr["Id"],
                        AcountName = (string)dr["Account"],
                        Keys = (string)dr["Key"],
                        Connection = (string)dr["Connection"]
                    }
                                );
                }
            }

            return blobGstnDict;
        }
    }
}
