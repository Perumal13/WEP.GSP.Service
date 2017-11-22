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

namespace WEP.GSP.WindowsServices
{
    public partial class GSTWindowApplication : ServiceBase
    {

        public GSTWindowApplication()
        {

            InitializeComponent();
            eventLogger = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("WEP.GST.P2"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "WEP.GST.P2", "Application");
            }
            eventLogger.Source = "WEP.GST.P2";
            eventLogger.Log = "Application";
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {

            //this.timerService.Interval = 120000;// 2 minutes
            eventLogger.WriteEntry("Start working", EventLogEntryType.Information, 2);
            MyThread thr1 = new MyThread();
            MyThread thr2 = new MyThread();
            try
            {
                //thr1.WorkerProcessForLiveRequest();
                //thr2.WorkerProcessForBacklog();
                Thread liveReqThread = new Thread(new ThreadStart(thr1.WorkerProcessForLiveRequest));
                Thread backlogReqThread = new Thread(new ThreadStart(thr2.WorkerProcessForBacklog));
                liveReqThread.Start();
                backlogReqThread.Start();
            }
            catch (Exception ex)
            {
            }

            eventLogger.WriteEntry("Completed successfully", EventLogEntryType.Information, 1);
        }

        protected override void OnStop()
        {
            eventLogger.WriteEntry("Stop working", EventLogEntryType.Information, 3);
            MyThread thr3 = new MyThread();
            thr3.UnRegisterEH();
            var requestdata = new WEP.GSP.Data.RequestData();
            requestdata.UpdateReleasePartitionLockOfInstance(System.Environment.MachineName);
        }


        private void timerService_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //eventLogger.WriteEntry("Start working", EventLogEntryType.Information, 2);
            //MyThread thr1 = new MyThread();
            //MyThread thr2 = new MyThread();
            //try
            //{
            //    Thread liveReqThread = new Thread(new ThreadStart(thr1.WorkerProcessForLiveRequest));
            //    Thread backlogReqThread = new Thread(new ThreadStart(thr2.WorkerProcessForBacklog));
            //    liveReqThread.Start();
            //    backlogReqThread.Start();
            //}
            //catch (Exception ex)
            //{
            //}

            //eventLogger.WriteEntry("Completed successfully", EventLogEntryType.Information, 1);
        }
    }
}
