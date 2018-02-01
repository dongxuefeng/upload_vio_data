using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using ehl.itgs.uploaddate.service;



namespace ehl.itgs.uploaddata
{
    class Program
    {


        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            if(false)
            {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new Service() 
            };
            ServiceBase.Run(ServicesToRun);
            }
            else
            {
                Service tservice = new Service();
                tservice.runstart();
            }
        }
    }
}
