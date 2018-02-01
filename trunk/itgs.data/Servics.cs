using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ehl.itgs.uploaddata.data;
using ehl.itgs.uploaddata.lib;
using System.Configuration;
using System.Threading;

namespace ehl.itgs.uploaddate.service
{
    
    public partial class Service : ServiceBase
    {
        public static bool run_flag = false;
        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            runstart();

        }

        protected override void OnStop()
        {
            runstop();
        }

        //委托与需要代表的方法具有相同的参数和返回类型
         public delegate void RunHandler();
         RunHandler handler;
         IAsyncResult result;
        
         public  void runstart()
         {

             //IAsyncResult: 异步操作接口(interface)
             //BeginInvoke: 委托(delegate)的一个异步方法的开始
             handler = new RunHandler(runstart_t);
             result = handler.BeginInvoke(null, null);
             //  testc();
             Console.ReadLine();
         }

        public static void runstart_t()
        {
            Log.Savelog("thread begin");
            run_flag = true;
            while(run_flag)
            {
                
             
 //               Log.Savelog("test");
                string str = ehl.uplaoddata.configure.read_configure("d:\\ehl_exp\\", "dir", "export");
                //
                itgs_data idp = new itgs_data(str);
                idp.process_data();
                //
 //               Log.Savelog(str);
                Thread.Sleep(3000);
            };
            Log.Savelog("thread end");
        }
        public void runstop()
        {
            run_flag = false;
        }
    }
}
