using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using XcDownLoadFile;
//using System.Data.OleDb;
using System.Data.OracleClient;
using ehl.itgs.uploaddata.access_db;
using ehl.itgs.uploaddata.lib;
using System.Runtime.InteropServices;
using System.IO;
using System.Web;
using ehl.itgs.uploaddata.WebReference;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;

namespace ehl.itgs.uploaddata.data
{

    class itgs_data
    {


        private string sexp_dir;
        public itgs_data(string exp_dir)
        {
            sexp_dir = exp_dir;

        }

        public string format_drdata(object readitem)
        {
            if (readitem == null)
                return "";
            else
                return readitem.ToString();

        }

        public string format_device_info(string devicecode, string prop,string value)
        {
            return ehl.uplaoddata.configure.read_configure(value, "device", "code_"+devicecode,"prop_"+prop); //接口ID
        }

        public string format_viotype(string vio_type)
        {
            return ehl.uplaoddata.configure.read_configure(vio_type, "dict", "viotype", "v_"+vio_type); //接口ID
        }

      

        /// <summary>
        ///  convert database to string data 
        /// </summary>
        /// <param name="read"></param>
        /// <returns></returns>
        public string format_data(OracleDataReader read, string[] filename)
        {
            //越黄线 违法类型：001	
            //超速 违法类型：002	
            //闯红灯 违法类型：003	
            //牌照比对 违法类型：009
            //大货车禁行 违法类型：010	
            //黄标车 违法类型：011	
            //违法停车 违法类型：012
            //走专用道 违法类型：013
            StringBuilder dataFormat = new StringBuilder();

            string stringformat = "{0,-2}{1,-8}{2,-11}{3,-21}{4,-9}{5,-9}{6,-9}"
                    + "{7,-9}{8,-9}{9,-30}{10,-5}{11,-8}{12,-5}{13,-5}{14,-3}"
                     + "{15,-20}{16,-10}{17,-30}{18,-30}";

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(stringformat,
                format_drdata(read["HPZL"]),                //号牌种类  2位  {0,-2}
                format_drdata(read["HPHM"]),               //号牌号码 8 {1,-8}
                format_drdata(read["WFRQ"]),        //抓拍日期年月日（不可空yyyy-mm-dd）11 {2,-11}}
                format_drdata(read["wfdz"]),                 //路段名称（不可空） 21 {3,-21}
                format_drdata(read["wfdd"]),                //9	 {4,-9}
                format_drdata(read["FXBH"]),                     //{5,-9}
                format_drdata(read["redlamp_starttime"]),      //{6,-9}
                format_drdata(read["WFSJ"]),                    //{7,-9}
                format_drdata(read["redlamp_ehdtime"]),        // {8,-9}
                format_drdata(read["jpgfilename1"]),           //,30{9,-30}
                format_drdata(read["clsd"]),               // {10,-5}
                format_drdata(read["CDMC"]),               //8{11,-8}
                format_drdata(read["CDBH"]),             //{12,-5}
                format_drdata(read["SBLX"]),            //{13,-5}
                format_viotype(format_drdata(read["viotype"])),                //{14,-3}
                format_drdata(read["SBBH"]),            //{15,-20}
                format_drdata(filename[1]),
                format_drdata(filename[2]),
                format_drdata(filename[3]));
            //read["police_code"].ToString(),            //{16,-10}
            //read["jpgfilename2"].ToString(),           //{17,-30}
            //read["jpgfilename3"].ToString());          //{18,-30}
            return sb.ToString();
        }


        public static string get_uft8(string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }

        #region webservice upload data
        private System.Drawing.Imaging.ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            System.Drawing.Imaging.ImageCodecInfo[] encoders;
            encoders = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            for (int j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="read"></param>
        private bool webservice_write_viodata(OracleDataReader read)
        {
            //http://10.177.98.6:9080/rminf/services/RmOutAccess?wsdl
            bool bresult = false;
 
     
            //      1、	接口类型：写入类接口
            //      2、	系统类别：60
            //      3、	接口ID：60W01
            //      4、	节点名称：surscreen
            //      5、	接口说明：提供给交通违法行为取证设备将抓拍的非现场交通违法写入系统，
            //写入的记录需通过系统的筛选功能进行部分信息的人工补录；

            string xtlb = "60";         //系统类别 
            string jkxlh = ehl.uplaoddata.configure.read_configure("7A1E1D0F02070305091502010002090200060904030B1791823F6D72692E636E", "uplaod_webservice", "data", "接口序列号"); //接口ID;     //接口序列号  //7A1E1D0F02070305091502010002090200060904030B1791823F6D72692E636E
            string jkid = "60W01";//ehl.uplaoddata.configure.read_configure("60W01", "uplaod_webservice", "data", "接口ID"); //接口ID


            //格式化文件名
            string[] filename = format_filename(read);

            int ipccount = 0;
            bool bwrite = true;
            byte[] jp1 = null;
            byte[] jp2 = null;
            byte[] jp3 = null; ;
            //下载jpeg file
//            Log.Savelog(format_drdata(read["jpgfilename1"]));
            jp1 = download_jpegdata(read["jpgfilename1"].ToString());
            

            ipccount++;

            if (read["jpgfilename2"].ToString() != "")
            {
                jp2 = download_jpegdata(read["jpgfilename2"].ToString());
               
                ipccount++;
            }

            if (read["jpgfilename3"].ToString() != "")
            {
                jp3 = download_jpegdata(read["jpgfilename2"].ToString());
               
                ipccount++;
            }


 

            string devicecode = format_drdata(read["SBBH"]);
            string WriteXmlDoc =
            "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>"
             + "<root>"
                 + "<surscreen>"
                     + "<fxjg>" + format_device_info(devicecode, "fxjg", format_drdata(read["cjjg"])) + "</fxjg>"      //12位管理部门代码
                     + "<zqmj>"/* + format_drdata(read["zqmj"])*/ + "</zqmj>"                                              //警员代号
                     + "<clfl>"+ format_drdata(read["clfl"]) +"</clfl>"                                                //车辆分类              
                     + "<hpzl>" + format_drdata(read["HPZL"]) + "</hpzl>"                                              //号牌种类
                     + "<hphm>" + format_drdata(read["HPHM"]) + "</hphm>"
                     + "<sblx>" + format_device_info(devicecode, "sblx","1") + "</sblx>"                               //设备类型
                     + "<sbbh>" + format_device_info(devicecode, "SBBH", devicecode) + "</sbbh>"
                     + "<xzqh>" + format_device_info(devicecode, "xzqh", "000000") + "</xzqh>"                         //违法地行政区划
                     + "<wfsj>" + format_drdata(read["WFSJ"]) + "</wfsj>"                                              //违法时间 YYYY-MM-DD hh24:mi:ss 
                     + "<wfdd>"/* + format_drdata(read["wfdd"]) */+ "</wfdd>"                                              //违法地点
                     + "<lddm>" + format_device_info(devicecode, "lddm", "") + "</lddm>"                               //路段代码 公里数
                     + "<ddms>" + format_device_info(devicecode, "ddms", "") + "</ddms>"                               //地点米数
                     + "<wfsj1>" + format_drdata(read["WFSJ"]) + "</wfsj1>"                                            //违法时间1
                     + "<wfdz>" + format_drdata(read["wfdz"]) + "</wfdz>"                                              //违法地址
                     + "<wfxw>" + format_viotype(format_drdata(read["wfxw"])) + "</wfxw>"                              //违法行为 viotype
                     + "<scz>" + format_drdata(read["clsd"]) + "</scz>"                                      //实测值
                     + "<bzz>" + format_device_info(devicecode, "bzz" + format_drdata(read["HPZL"]), "1") + "</bzz>"                                  //标准值
                     + "<zpsl>" + ipccount.ToString() + "</zpsl>"                                //照片数量
                     + "<zpwjm>" /*+ filename[1]*/+ "</zpwjm>";//图片文件名
            Log.Savelog(xtlb + jkxlh + jkid + WriteXmlDoc);


            if (jp1 != null)
            {
                try
                {
                    /*              
                    MemoryStream memoryStream = new MemoryStream(jp1);
                    Bitmap bitmap = new Bitmap(memoryStream);
                    ImageFormat tFormat = bitmap.RawFormat;
                    MemoryStream ms = new MemoryStream();
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    byte[] arr = new byte[ms.Length];
                    memoryStream.Close();
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    ms.Close();
               */
                    //20150522  修改,增加图像压缩率

                    MemoryStream min = new MemoryStream(jp1);
                    MemoryStream ms = new MemoryStream();
                    GetPicThumbnailStream(min, ms, 60);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, arr.Length);
                    min.Close();
                    ms.Close();


                    string picdata = HttpUtility.UrlEncode(Convert.ToBase64String(arr, 0, arr.Length), Encoding.UTF8);
                    WriteXmlDoc += "<zpstr1>" + picdata + "</zpstr1>"; //图片1
                }
                catch (Exception e)
                {
                    Log.Savelog(e.Message);
                    WriteXmlDoc += "<zpstr1></zpstr1>"; //图片1
                }
            }
            else
            {
                WriteXmlDoc += "<zpstr1></zpstr1>"; //图片1
            }
            if (ipccount >= 2)
            {
                try
                {
                    /*
                    MemoryStream memoryStream = new MemoryStream(jp2);
                    Bitmap bitmap = new Bitmap(memoryStream);
                    MemoryStream ms = new MemoryStream();
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] arr = new byte[ms.Length];
                    memoryStream.Close();
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    ms.Close();
                    */

                    //20150522  修改,增加图像压缩率

                    MemoryStream min = new MemoryStream(jp2);
                    MemoryStream ms = new MemoryStream();
                    GetPicThumbnailStream(min, ms, 60);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, arr.Length);
                    min.Close();
                    ms.Close();


                string picdata = HttpUtility.UrlEncode(Convert.ToBase64String(arr, 0, arr.Length), Encoding.UTF8);
  //            WriteXmlDoc += "<zpstr1>" + picdata + "</zpstr1>"; //图片1

                WriteXmlDoc += "<zpstr2>" + picdata + "</zpstr2>";
                }
                catch (Exception e)
                {
                    Log.Savelog(e.Message);
                    WriteXmlDoc += "<zpstr2></zpstr2>";
                }
            }
            else
            { WriteXmlDoc += "<zpstr2></zpstr2>"; }                     //图片2

            if (ipccount >= 3)
            {
                /*
                MemoryStream memoryStream = new MemoryStream(jp3);
                Bitmap bitmap = new Bitmap(memoryStream);
                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                memoryStream.Close();
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                */
                //20150522  修改,增加图像压缩率

                MemoryStream min = new MemoryStream(jp3);
                MemoryStream ms = new MemoryStream();
                GetPicThumbnailStream(min, ms, 60);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, arr.Length);
                min.Close();
                ms.Close();


                string picdata = HttpUtility.UrlEncode(Convert.ToBase64String(arr, 0, arr.Length), Encoding.UTF8);
                WriteXmlDoc += "<zpstr3>" + picdata + "</zpstr3>";
            }
            else
            { WriteXmlDoc += "<zpstr3></zpstr3>"; }

            WriteXmlDoc += "</surscreen>"
                        + "</root>";

            string filepath = sexp_dir + "/" + read["WFRQ"].ToString() + "/";
            write_datafile(filepath + filename[0] + ".xml", WriteXmlDoc);
            try
            {
    //            Log.Savelog(xtlb + jkxlh+ jkid +WriteXmlDoc );

                WebReference.RmJaxRpcOutAccessService  webclient = new RmJaxRpcOutAccessService();
                webclient.Url = ehl.uplaoddata.configure.read_configure
                    ("http://61.12.16.2:9080/rmweb/services/RmOutAccess?wsdl", "uplaod_webservice", "data", "url");
                /*http://10.177.98.6:9080/rminf/services/RmOutAccess?wsdl  old webservice address*/

                string webresult = webclient.writeObjectOut(xtlb, jkxlh, jkid, WriteXmlDoc);
                             
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webresult);
                 
                XmlNode node = xmlDoc.SelectSingleNode("/root/head/code");
                String rs = node.InnerText;
                Log.Savelog(webresult);
                if (rs == "1" || rs == "0")
                {
                    Log.Savelog(read["XH"] + "save ok");
                    bresult = true;
                    upflag(read["XH"].ToString());
                };
                Log.Savelog(xmlDoc.SelectNodes("root/head/head/code").ToString()
                    + ","
                    + xmlDoc.SelectNodes("root/head/head/message").ToString()
                    + ","
                    + xmlDoc.SelectNodes("root/head/head/value").ToString());
                
            }
            catch (System.InvalidOperationException sss)
            {
                Log.Savelog("web service error:" + sss.Message);
                System.Threading.Thread.Sleep(5000);
                return bresult;
            };
           
           // string xmlresult = tmp.writeObjectOut(xtlb, jkxlh, jkid, WriteXmlDoc);
            /*
              <?xml version="1.0" encoding="UTF-8" ?>
              <root>
                   <head>
                       <code>1</code>
                       <message>正确</message>
                       <value>3202020000000072</value>
                  </head>
              </root>61.12.16.2 9080
             */
          

            return bresult;
        }


        public static bool GetPicThumbnailStream(MemoryStream sFile, MemoryStream outPath, int flag)
        {

            System.Drawing.Image iSource = System.Drawing.Image.FromStream(sFile);
            ImageFormat tFormat = iSource.RawFormat;
            //以下代码为保存图片时，设置压缩质量  
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100  
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    iSource.Save(outPath, jpegICIinfo, ep);//dFile是压缩后的新路径  
                }
                else
                {
                    iSource.Save(outPath, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                iSource.Dispose();
            }
        }
        #endregion webservice upload data

        /// <summary>
        /// format data&jpeg filename
        /// </summary>
        /// <param name="read"></param>
        /// <returns>string[0]:datafilename,string[1]jpeg1 name,string[2] jpeg2,string[3] jpegname3</returns>

        #region upload data save to file
        public string[] format_filename(OracleDataReader read)
        {

            string[] stringfilename = new string[4];
            stringfilename[0] = read["xh"].ToString() + ".dat";
            stringfilename[1] = read["xh"].ToString() + "_1.jpg";
            if (read["jpgfilename2"].ToString() != "")
                stringfilename[2] = read["xh"].ToString() + "_2.jpg";
            else stringfilename[2] = "";


            if (read["jpgfilename3"].ToString() != "")
                stringfilename[3] = read["xh"].ToString() + "_3.jpg";
            else stringfilename[3] = "";
            return stringfilename;
        }

        #region read data from db
        public static access_db.access_db acd = new access_db.access_db();
        public void process_data()
        {

            //Data Source=TORCL;User Id=myUsername;Password=myPassword; 
            string connString = "Data Source="
            + ehl.uplaoddata.configure.read_configure("atmsfg", "database", "name")
            + ";User ID="
            + ehl.uplaoddata.configure.read_configure("tgs_admin", "database", "user")
            + ";Password="
            + ehl.uplaoddata.configure.read_configure("ehl1234", "database", "password")
            + ";";

            // string connString = "Provider=MSDAORA.1;User ID=atmsfg;Password=ehl1234;Data Source=(DESCRIPTION = (ADDRESS_LIST= (ADDRESS = (PROTOCOL = TCP)(HOST = 127.0.0.1)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME = ATMSFG)))";
            // 大货车禁行,号牌种类,号牌号码,抓拍日期年月日（不可空yyyy-mm-dd）
            //路段名称（不可空）,道路路段代码（5位道路代码+4为路段代码）	抓拍车行驶方向（不可空）
            //红灯开始时间（空9位）,违章时间（不可空hh:mm:ss）	红灯结束时间
            //相应第一个jpg文件名（不可空）			
            //行驶速度（空5位）,行驶车道(汉字)（空8位）							
            //车道编码(数字),	设备类型,	违法类型：010
            //设备编号（不可空，且在“六合一”中备案）,
            //执勤民警警号（可空，无关联执勤民警时传10位空格）
            //相应第二个jpg文件名（不可空）,相应第三个jpg文件名（可空）																																				


            /*              sql从配置文件读取
            string sqlstr =
 
               " SELECT   t.XH,t.fxbh,t.sbbj,t.cdbh,decode(t.HPZL,'01',3,'02',3,'23',4,'24',5,'25',4,'13',6,'14',6,3) AS CLFL,"
               +" decode(t.HPZL,'23','02','24','02','05','02','25','07','26','07',HPZL) AS HPZL, "
               +" t.HPHM,"
               +" NVL(T.CLLX,decode(t.HPZL,'13','H41','K31')) AS JTFS, "
               +" NVL(T.FZJG,'陕K')AS FZJG,"
               +" decode(t.SJLY,'1',1,'2',2,'3',3,'4',4,'5',5,'6',6,'8',9,'9',9)AS SJLY,"
               +" TO_Char(t.WFSJ,'yyyy-mm-dd') as WFRQ,"
               + " TO_CHAR(t.WFSJ,'yyyy-mm-dd hh24:mi:ss') as WFSJ, t1.fxcddbh as wfdd,t1.sbbh as sbbh,"     
               +" t1.ddmc  as wfdz, t.WFXW, t.CSBJ,CJJG,"
               +" f_get_value ('WFXWMS','T_TMS_PECCNACY_TYPE','WFXW',t.WFXW) AS WFNR,"
               +" '' as zqmj, nvl(t.jdssyr,'未知')as jdssyr,t.dh,t.lxfs,t.zsxxdz,t.zsxzqh,"
               +" t1.xzqh as xzqh ,"
               +" t.clpp,t.cllx,t.csys,t.fdjh,t.clsbdh,t.clsd,"
                +"Fzl) as clxs,"
                +"t.jcbj,NVL(t.SYXZ,'A') AS SYX_GET_LIMITSPEED(t.wfdd,t.hpZ,"
	        +"(select 'http://'||zjwjip||zjwjlj  from  tgs_admin.t_tgs_capture where xh = t.xh and zjwjsx=1) as jpgfilename1,"
            +"(select 'http://'||zjwjip||zjwjlj  from  tgs_admin.t_tgs_capture where xh = t.xh and zjwjsx=2) as jpgfilename2,"
            +"(select 'http://'||zjwjip||zjwjlj  from  tgs_admin.t_tgs_capture where xh = t.xh and zjwjsx=3) as jpgfilename3"
            +" FROM   t_tms_peccancy t "
            +" left join t_tgs_location t1 on t.WFDD = t1.ddbh "
            +" where t.CSBJ = '0'"
           +" and t.WFSJ>= sysdate-30 "
           +" and t.WFDD in ('620031002000',620031018000) "
           +" and rownum < 50";
            */
            string sqlstr = "";
             sqlstr =   ehl.uplaoddata.configure.read_configure(sqlstr , "database", "querysql");
            /*
               "xh,cjjg ,clfl,wfxw,csbj,jtfs  ,HPZL,HPHM "
                + ", TO_CHAR (t.WFSJ, 'yyyy-MM-dd') as WFRQ "
             + " , wfdz ,wfdd as wfdd,substr(FXBH,-1,1) as FXBH  ,"
             + "'' as redlamp_starttime,TO_CHAR (t.WFSJ, 'yyyy-MM-dd hh24:mi:ss') as WFSJ,"
             + "clsd as clsd,'' as CDMC,"
             + "CDBH as CDBH,sbbj as SBLX,wfxw as viotype,"
             + "wfdd as SBBH,zqmj as zqmj,"
             + "(select 'http://'||zjwjip||zjwjlj  from  tgs_admin.t_tgs_capture where xh = t.xh and zjwjsx=1) as jpgfilename1,"
             + "(select 'http://'||zjwjip||zjwjlj  from   tgs_admin.t_tgs_capture where xh = t.xh and zjwjsx=2) as jpgfilename2, "
             + "(select 'http://'||zjwjip||zjwjlj  from   tgs_admin.t_tgs_capture where xh = t.xh and zjwjsx=3) as jpgfilename3 "
             + " from tgs_admin.vt_tms_peccancy_upload t where  rownum < 30 "
            */

            Log.Savelog(connString);
            Log.Savelog(sqlstr);
            if (!acd.connectioned)
            {
                acd.conn_oracledb(connString);
                Log.Savelog("reconnected dbservice"); 
            }
            else
            {
                
            };
            if (acd.connectioned)
            {
                Log.Savelog("proce_data!");
                OracleDataReader reader = null;
                acd.execsql_reader(ref reader, sqlstr);
                if(reader != null)
                while (reader.Read())
                {
                    bool bwrite = false;
                    //本地文件目录
                    string filepath = sexp_dir + "/" + reader["WFRQ"].ToString() + "/";
                    Log.Savelog(filepath);
                    bwrite = IOHelper.CreateDir(filepath);
                    //获取文件名
                    // sqlstr = "select * from tgs_admin.t_tgs_capture where xh='" + reader["xh"].ToString() + "'";
                    // OleDbDataReader rjpegfile = null;
                    // acd.execsql_reader(ref rjpegfile, sqlstr);

                    //格式化文件
                    string[] filename = format_filename(reader);
                    if (false)
                    {
                        //下载jpeg file
                        bwrite = bwrite && download_jpegdata(reader["jpgfilename1"].ToString(), filepath + filename[1]);
                        if (reader["jpgfilename2"].ToString() != "")
                            bwrite = bwrite && download_jpegdata(reader["jpgfilename2"].ToString(), filepath + filename[2]);
                        if (reader["jpgfilename3"].ToString() != "")
                            bwrite = bwrite && download_jpegdata(reader["jpgfilename3"].ToString(), filepath + filename[3]);
                        //格式化数据
                        string strdata = format_data(reader, filename);
                        //写入数据文件
                        bwrite = bwrite && write_datafile(filepath + filename[0], strdata);
                    }
                    else
                    {
                        bwrite = webservice_write_viodata(reader);
                    }


                    //写入库导出标记
                    if (bwrite)
                    {
                        upflag(reader["XH"].ToString());
                    }
                    else
                    {
                        Log.Savelog("process data item error.");
                    };

                };
                reader.Close();
            }
        }
        #endregion read data from db

        public bool write_datafile(string datafilepath, string strdata)
        {
            
            bool bresult = IOHelper.Write(datafilepath, strdata);
            return bresult;
        }
        #endregion upload data save to file

        #region update database flag
        public void upflag(string xh)
        {
            string sqlstr1 = " update  tgs_admin.vt_tms_peccancy_upload t set csbj  = '1' where xh = " + xh;
            Log.Savelog(sqlstr1);
            int d = acd.execsql_noresult(sqlstr1);
            Log.Savelog("sql resutl = " + d);
        }
        #endregion update database flag

        #region download jpeg pic from http
        public byte[] download_jpegdata(string httpfilename)
        {
            Log.Savelog(httpfilename);
            byte[] bresult = null;

            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                bresult = wc.DownloadData(httpfilename);


            }
            catch (System.Net.WebException we)
            {
                Log.Savelog(we.ToString());
            }
            return bresult;
            //return bresult;
        }

        public bool download_jpegdata(string httpfilename, string jpegfilename)
        {
            Log.Savelog(httpfilename);
            bool bresult = false;
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                wc.DownloadFile(httpfilename, jpegfilename);
                bresult = true;
            }
            catch (System.Net.WebException we)
            {
                Log.Savelog(we.ToString());
            }
            return bresult;
        }
        #endregion download jpeg pic from http
    }
}
