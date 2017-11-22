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
  
    public class MasterData
    {
        private readonly Database _db;
        public MasterData()
        {
            var dbFactory = new DatabaseProviderFactory();
            _db = dbFactory.Create(Constants.GSPConnect);
        }

        public int GetStorageAccountIdByName(string storageAccName)
        {
            int storageAccId = 0;
            //using (IDataReader dr = _db.ExecuteReader(Constants.SPName.Get_GSTN_StorageAccount_NameById, storageAccName))
            //{
            //    while (dr.Read())
            //    {
            //        storageAccId = Convert.ToInt32(dr["Id"]);
            //    }
            //    if (!dr.IsClosed)
            //        dr.Close();
            //    return storageAccId;
            //}
            return storageAccId;

        }


    }
}
