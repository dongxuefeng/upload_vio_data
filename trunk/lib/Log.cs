using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO; 

namespace ehl.itgs.uploaddata.lib
{
    class Log
    {
        //保存日志
        public static void Savelog(string s)
        {
            Console.WriteLine(s);
            try
            {
                string fileFolder = AppDomain.CurrentDomain.BaseDirectory.ToString() + "log";
                if (!Directory.Exists(fileFolder))
                {
                    Directory.CreateDirectory(fileFolder);
                }
                string filePath = fileFolder + "\\" + GetLogfile();
                FileStream fs;
                if (!File.Exists(filePath))
                {
                    fs = File.Create(filePath);
                }
                else
                {
                    fs = File.Open(filePath, FileMode.Append);
                }
                string strToWrite = "\r\n" + System.DateTime.Now.ToString() + "\r\n" + s + "\r\n";
                byte[] b = System.Text.Encoding.Default.GetBytes(strToWrite);
                fs.Write(b, 0, b.Length);
                fs.Close();
            }
            catch
            { }
        }

        //获取日志的名称，按天
        public static string GetLogfile()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("ehl_expdata");
            string date = System.DateTime.Today.ToString("yyyy-MM-dd");
            sb.Append(date);
            sb.Append(".log");

            return sb.ToString();
        }
    }
}
