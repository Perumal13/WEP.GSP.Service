using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Data;

namespace WEP.GSP.Business
{
    public class ExceptionBusiness
    {
        public void InsertExceptionLog(string token, string ExceptionMessage, string Source, int stage)
        {
            Task.Factory.StartNew(() => new ExceptionData().InsertExceptionLog(token, ExceptionMessage, Source, stage));
        }
    }
}
