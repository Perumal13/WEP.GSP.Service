namespace WEP.GSP.WindowsServices
{
    partial class GSTWindowApplication
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.eventLogger = new System.Diagnostics.EventLog();
            this.timerService = new System.Timers.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.eventLogger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timerService)).BeginInit();
            // 
            // timerService
            // 
            this.timerService.Enabled = true;
            this.timerService.Interval = 120000D;
            // 
            // GSTWindowApplication
            // 
            this.ServiceName = "GSTWindowApplication";
            ((System.ComponentModel.ISupportInitialize)(this.eventLogger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timerService)).EndInit();

        }

        #endregion

        private System.Diagnostics.EventLog eventLogger;
        private System.Timers.Timer timerService;
    }
}
