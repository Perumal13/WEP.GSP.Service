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
    public class UserData
    {
        private readonly Database _db;
        public UserData()
        {
            var dbFactory = new DatabaseProviderFactory();
            _db = dbFactory.Create(Constants.GSPConnect);
        }

        public bool ValidateUser()
        {
            using (IDataReader dr = _db.ExecuteReader(Constants.SPName.Gsp_USP_ValidateUser))
            {
                while (dr.Read())
                {
                    return true;
                }
            }

            return false;
        }

        public Blob GetBlobConnectionByToken(string token)
        {
            Blob objBlob = new Blob();

            using (IDataReader reader = _db.ExecuteReader(Constants.SPName.GSP_Get_GstnResponseByToken, token))
            {
                while (reader.Read())
                {
                    var attributes = new Blob()
                    {
                        Id = Convert.IsDBNull(reader["ResponseBlobId"]) ? 0 : Convert.ToInt32(reader["ResponseBlobId"]),
                        AcountName = Convert.IsDBNull(reader["Account"]) ? null : Convert.ToString(reader["Account"]),
                        Keys= Convert.IsDBNull(reader["Key"]) ? null : Convert.ToString(reader["Key"]),
                        Connection= Convert.IsDBNull(reader["Connection"]) ? null : Convert.ToString(reader["Connection"])
                    };
                    return attributes;    
                }
            }

            return null;
        }


    }
}
