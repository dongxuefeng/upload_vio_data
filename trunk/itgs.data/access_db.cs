using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OracleClient;
using XcDownLoadFile;
using ehl.itgs.uploaddata.lib;

namespace ehl.itgs.uploaddata.access_db
{
    class access_db
    {
        public bool connectioned = false;
        OracleConnection conn;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool conn_oracledb(string connString)
        {

            
            try
            {
                conn = new OracleConnection(connString);
                conn.Open();
                connectioned = true;
                Log.Savelog(conn.State.ToString());
            }
            catch (Exception ex)
            {
                connectioned = false;
                Log.Savelog(ex.Message.ToString());
                conn.Close();
            }
            finally
            {
                
              
            }
            return connectioned;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql_text"></param>  
        /// <returns></returns>
        public bool execsql_reader(ref OracleDataReader reader, string sql_text)
        {

            bool bresult=false;
            Log.Savelog("in execsql_reader");
            if (connectioned)
            {
                OracleCommand dc = new OracleCommand(sql_text, conn);
                try
                {
                    Log.Savelog(sql_text);
                    reader = dc.ExecuteReader();
                    Log.Savelog("exec sql result:" + reader.HasRows.ToString());
                    bresult = true;
                }
                catch (Exception ex)
                {
                    connectioned = false;
                    Log.Savelog(ex.Message.ToString());
                }
                finally
                {

                }
            }
            else
            {
                Log.Savelog("connectioned is false");
            }


            return bresult;

        }
        /// <summary>
        /// exec sql noresult
        /// </summary>
        /// <param name="sql_text"></param>
        /// <returns></returns>
        public int execsql_noresult(string sql_text)
        {
            if (connectioned)
            {
                OracleCommand dc = new OracleCommand(sql_text, conn);
                int iresult = dc.ExecuteNonQuery();
                return iresult;
            }
            return -1;
        }

    }
}