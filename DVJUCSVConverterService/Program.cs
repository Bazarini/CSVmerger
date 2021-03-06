﻿using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DVJUCSVConverterService
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                var c = new CSVConverter();
                c.TestStartUpAndStop(null);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new CSVConverter()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
