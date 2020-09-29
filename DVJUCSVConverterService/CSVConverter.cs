using Logger;
using PDFToDJVU;
using System;
using CSVMergerCore;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using CsvMerger;
using System.Threading;

namespace DVJUCSVConverterService
{
    partial class CSVConverter : ServiceBase
    {
        Thread _thread;
        CancellationTokenSource source;
        public CSVConverter()
        {
            InitializeComponent();            
        }
        ServiceWorker serviceWorker;
        protected override void OnStart(string[] args)
        {
            source = new CancellationTokenSource();
            var token = source.Token;
            serviceWorker = new ServiceWorker(token);
            serviceWorker.Prepare();
            _thread = new Thread(new ThreadStart(serviceWorker.Start));
            _thread.Start();
        }

        protected override void OnStop()
        {
            source.Cancel();
            _thread.Join(25 * 1000);            
            _thread = null;
        }
    }
}
