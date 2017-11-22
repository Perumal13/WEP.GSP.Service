using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WEP.GSP.Business;

namespace WEP.GSP.WindowsServicesA1
{
    public partial class GSTWindowApplicationA1 : ServiceBase
    {

        public GSTWindowApplicationA1()
        {

            InitializeComponent();
            eventLogger = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("WEP.GST.P2.A1"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "WEP.GST.P2.A1", "Application");
            }
            eventLogger.Source = "WEP.GST.P2.A1";
            eventLogger.Log = "Application";
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            eventLogger.WriteEntry("Start working", EventLogEntryType.Information, 2);
            MyThread liveThread = new MyThread();
            MyThread backLogThread = new MyThread();
            try
            {
                Thread liveReqThread = new Thread(new ThreadStart(liveThread.WorkerProcessForLiveRequest));
                Thread backlogReqThread = new Thread(new ThreadStart(backLogThread.WorkerProcessForBacklog));

                liveReqThread.Start();
                backlogReqThread.Start();

                eventLogger.WriteEntry("Completed successfully", EventLogEntryType.Information, 1);
            }
            catch (Exception ex)
            {
                eventLogger.WriteEntry(ex.InnerException.ToString(), EventLogEntryType.Error, 9);
            }           
        }

        protected override void OnStop()
        {
            MyThread thr3 = new MyThread();
            thr3.UnRegisterEH();
            //var requestdata = new WEP.GSP.Data.RequestData();
            //requestdata.UpdateReleasePartitionLockOfInstance(System.Environment.MachineName);
            eventLogger.WriteEntry("Stop working", EventLogEntryType.Information, 3);
        }

    }
}
