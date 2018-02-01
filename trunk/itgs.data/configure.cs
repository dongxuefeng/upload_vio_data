using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ehl.lib;

namespace ehl.uplaoddata
{
    class configure
    {
        public static string fileFolder = AppDomain.CurrentDomain.BaseDirectory.ToString() + "configure";
        public static string configure_file_name = fileFolder+"/config_upload.xml";
        private static ehl.lib.XmlConfigUtil xcu = new XmlConfigUtil(configure_file_name);
        public static string  read_configure(string default_value, params string[] nodes)
        {
            //string[] tnodes = nodes;
            string tmpvalue = xcu.Read(nodes);
            if(tmpvalue == null)
            {
                tmpvalue = default_value;
                xcu.Write(tmpvalue,nodes);
            }
            return tmpvalue;
        }

    }
}
