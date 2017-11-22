using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WEP.GSP.WindowsServicesA1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //Un comment below code to run app in debug mode
            //#if DEBUG
            GSTWindowApplicationA1 obj = new GSTWindowApplicationA1();
            obj.OnDebug();
            Thread.Sleep(Timeout.Infinite);
            //#else
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new GSTWindowApplication()
            //};
            //ServiceBase.Run(ServicesToRun);
            //Thread.Sleep(Timeout.Infinite);
            //#endif
        }
    }
}
