using System.ServiceProcess;
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
            _thread = new Thread(new ThreadStart(serviceWorker.Start));
            if (serviceWorker.Prepare())
            {
                _thread.Start();
            }            
        }

        protected override void OnStop()
        {
            if (_thread.IsAlive)
            {
                source.Cancel();
                _thread.Join(25 * 1000);
                _thread = null;
            }
        }
    }
}
