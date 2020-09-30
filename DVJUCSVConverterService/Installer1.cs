using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace DVJUCSVConverterService
{
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;
        public Installer1()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "docAlpha_DocProf_Converter";
            serviceInstaller.Description = "Prepares a merged *.csv and *.djvu files from docAlpha output csv and pdf files";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
