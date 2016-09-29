using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using FileLib.DataFiles;
using DatabaseLib.Update;
using System.Configuration;
using System.IO;

namespace DatabaseLib
{
    public class DBSingleton
    {
        public List<string> ViewList { get; set; }
        public List<string> ProcedureList { get; set; }
        public DataTable CurrentDatatable { get; set; }
        public string DataFilePath { get; set; }
        public string UserName { get; set; }
        public string Station { get; set; }
        public string Testname { get; set; }

        private static DBSingleton _dbSingleton = null;
        private static object lockObj = new object();
        private SqlConnection sqlConn;
        private SqlCommand command = null;
        private SqlDataAdapter adapter = null;
        private SqlCommandBuilder cmdBuilder = null;             
        private string logFile = "C:\\data\\logfile.txt";
        private StreamWriter writer;

        private DBSingleton()
        {
            SqlConnectionStringBuilder connSqlBuilder = new SqlConnectionStringBuilder();
            connSqlBuilder.DataSource = "QPDXSQL84";
            connSqlBuilder.InitialCatalog = "Ops_DB_Dev";
            connSqlBuilder.UserID = "ServiceUser";
            connSqlBuilder.Password = "SQLServiceUserPW";

            sqlConn = new SqlConnection(connSqlBuilder.ConnectionString);
            DataFilePath = ConfigurationManager.AppSettings.Get("DataFilePath");
            if (DataFilePath == null)
            {
                DataFilePath = @"C:\TMFlex\Test Results\";
            }
            ViewList = LoadViews();
            ProcedureList = LoadProcedures();
            writer = new StreamWriter(logFile);
            writer.AutoFlush = true;
        }

        public void Init()
        {

        }

        public static DBSingleton Instance
        {
            get
            {
                if (_dbSingleton == null)
                {
                    lock (lockObj)
                    {
                        if (_dbSingleton == null)
                        {
                            _dbSingleton = new DBSingleton();
                        }
                    }
                }

                return _dbSingleton;
            }
        }

        public void WriteLog(string logText)
        {            
            writer.WriteLine(logText);
        }

        #region SQL General Functions

        public SqlConnection Connection
        {
            get
            {
                return sqlConn;
            }
            set
            {
                sqlConn = value;
            }
        }

        public SqlCommand Command
        {
            get
            {
                return command;
            }
            set
            {
                command = value;
            }
        }

        public SqlDataAdapter Adapter
        {
            get
            {
                return adapter;
            }
            set
            {
                adapter = value;
            }
        }

        public SqlCommandBuilder CmdBuilder
        {
            get
            {
                return cmdBuilder;
            }
            set
            {
                cmdBuilder = value;                
            }
        }

        public DataTable SelectQuery(string Cmd)
        {
            DataTable dt = new DataTable();
            Adapter = new SqlDataAdapter();
            Adapter.SelectCommand = new SqlCommand(Cmd);
            Adapter.SelectCommand.Connection = Connection;
            Connection.Open();
            Adapter.Fill(dt);
            Connection.Close();
            return dt;
        }

        public bool InsertData(string Table, Dictionary<string, object> data)
        {
            SqlCommand cmd = CreateInsertCommand(Table, data);
            DataTable dt = SelectQuery(string.Format("SELECT * FROM {0}", Table));
            DataRow dr = dt.NewRow();
            foreach (string field in data.Keys)
            {
                dr[field] = Convert.ChangeType(data[field], data[field].GetType());
            }
            dt.Rows.Add(dr);
            Connection.Open();
            cmd.ExecuteNonQuery();
            Connection.Close();

            return true;
        }

        public DataTable SelectView(string ViewName)
        {
            DataTable dt = new DataTable();
            Adapter = new SqlDataAdapter();
            Adapter.SelectCommand = new SqlCommand(string.Format("SELECT * FROM {0}", ViewName));
            Adapter.SelectCommand.Connection = Connection;
            Connection.Open();
            Adapter.Fill(dt);
            Connection.Close();
            return dt;
        }

        public DataTable SelectProcedure(string ProcedureName)
        {
            DataTable dt = new DataTable();
            Adapter = new SqlDataAdapter();
            Adapter.SelectCommand = new SqlCommand(string.Format("SELECT * FROM {0}", ProcedureName));
            Adapter.SelectCommand.Connection = Connection;
            Connection.Open();
            Adapter.Fill(dt);
            Connection.Close();
            return dt;
        }        

        #endregion

        public bool Ready
        {            
            get
            {
                if (sqlConn.State == System.Data.ConnectionState.Closed)
                {
                    return true;
                }
                return false;                
            }
        }        

        public void SetDatatable(DataTable dt)
        {
            CmdBuilder = new SqlCommandBuilder(Adapter);
            Adapter.Fill(dt);
            CurrentDatatable = dt;
        }

        /// <summary>
        /// Gets the ID that matches the correct table and date time.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public int GetID(string table, DateTime dateTime)
        {
            int result = -1;
            DataTable dt = SelectQuery(string.Format("SELECT TOP 1 * FROM {0} WHERE TimeDate = '{1}'", table, dateTime));
            if (dt.Columns.Contains("P_Id"))
            {
                if (dt.Rows.Count > 0)
                {
                    result = int.Parse(dt.Rows[0]["P_Id"].ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// Updates the test data tables with data from a test fixture file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool UpdateData(DataFile file)
        {
            Abstract_Update_Test_Data TestData = null;

            TestData = Update_Test_Data_Factory.CreateUpdateTest(file);

            return TestData.UpdateTestData(file);
        }

        #region "Linktable"

        public TMXF_Model LoadLinkedTMXF(string SerialNumber)
        {
            string serialNumberToFind = SerialNumber;
            bool linking = true;
            TMXF_Model tmxf = new TMXF_Model();
            // Walk backwards to the root of the link.
            while (linking)
            {                
                DataTable dt = SelectQuery(string.Format("SELECT * FROM TMflexLinkInfoState WHERE LinkedSerialNumber = '{0}'", serialNumberToFind));
                if (dt.Rows.Count >= 1)
                {
                    linking = LinkTMXFObject(ref serialNumberToFind, tmxf, dt, false);             
                }
                else
                {
                    linking = false;
                }                
            }

            // Reset the flag and walk foward.
            linking = true;
            serialNumberToFind = SerialNumber;
            while (linking)
            {
                DataTable dt = SelectQuery(string.Format("SELECT * FROM TMflexLinkInfoState WHERE SerialNUmber = '{0}'", serialNumberToFind));
                if (dt.Rows.Count >= 1)
                {
                    linking = LinkTMXFObject(ref serialNumberToFind, tmxf, dt, true);
                }
                else
                {
                    linking = false;
                }
            }

            // Add the actual assembly
            tmxf.SetSerialNumber(SerialNumber);
            return tmxf;
        }

        public bool UpdateLinkState(TMXF_Model TMXF)
        {
            bool success = false;

            Dictionary<string, string> linkage = TMXF.CreateLinkageDictionary();
            TMXF.AddEmptyLinks(linkage);
            Connection.Open();
            SqlCommand cmd = Connection.CreateCommand();
            cmd.Transaction = Connection.BeginTransaction("UpdateLinkTable");
            try
            {
                // Clear previous links.
                foreach (string SerialNumber in linkage.Keys)
                {
                    cmd.CommandText = string.Format("UPDATE {0} SET UpdatedDateTime='{1}',SerialNumber='{2}',LinkedSerialNumber='{3}' WHERE SerialNumber='{4}'",
                                                    "TMflexLinkInfoState", DateTime.Now.ToString(), SerialNumber, "", SerialNumber);
                    int rows = cmd.ExecuteNonQuery();                   
                }

                foreach (string SerialNumber in linkage.Keys)
                {
                    cmd.CommandText = string.Format("UPDATE {0} SET UpdatedDateTime='{1}',SerialNumber='{2}',LinkedSerialNumber='{3}' WHERE SerialNumber='{4}'",
                                                    "TMflexLinkInfoState", DateTime.Now.ToString(), SerialNumber, linkage[SerialNumber], SerialNumber);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        cmd.CommandText = string.Format("INSERT INTO {0} (UpdatedDateTime,SerialNumber,LinkedSerialNumber) VALUES ('{1}','{2}','{3}')",
                                                    "TMflexLinkInfoState", DateTime.Now.ToString(), SerialNumber, linkage[SerialNumber]);
                        rows = cmd.ExecuteNonQuery();
                    }
                }

                cmd.Transaction.Commit();

                success = true;
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
            }
            finally
            {
                Connection.Close();
            }

            return success;
        }

        public bool SplitLink(TMXF_Model TMXF, string SerialNumber)
        {
            Dictionary<string, string> master = TMXF.CreateLinkageDictionary();
            Dictionary<string, string> tmxf1 = new Dictionary<string,string>();
            Dictionary<string, string> tmxf2 = new Dictionary<string, string>();
            bool found = false;
            foreach (string sn in master.Keys)
            {
                if (sn == SerialNumber)
                {
                    found = true;
                }
                if (!found)
                {
                    tmxf1.Add(sn, master[sn]);
                }
                else
                {
                    tmxf2.Add(sn, master[sn]);
                }
            }

            TMXF_Model splitLower = new TMXF_Model();
            foreach (string sn in tmxf1.Keys)
            {
                splitLower.SetSerialNumber(sn);
            }            

            TMXF_Model splitupper = new TMXF_Model();
            foreach (string sn in tmxf2.Keys)
            {
                splitupper.SetSerialNumber(sn);
            }

            return UpdateLinkState(splitLower) & UpdateLinkState(splitupper);
        }

        public bool UnlinkSingle(TMXF_Model TMXF, string SerialNumber)
        {
            SubAssemblyModel sAssem = TMXF.GetSubAssembly(SerialNumber);
            sAssem.SerialNumber = "";
            return UpdateLinkState(TMXF);
        }

        public bool UnlinkAll(TMXF_Model TMXF)
        {
            Dictionary<string, string> linkage = TMXF.CreateLinkageDictionary();
            foreach (string key in linkage.Keys)
            {
                if (linkage[key] != "")
                {
                    TMXF.GetSubAssembly(linkage[key]).SerialNumber = "";
                }
            }

            return UpdateLinkState(TMXF);
        }     

        /// <summary>
        ///  Gets a list of the assemblies that are currently linked.
        /// </summary>
        /// <param name="assemblyType"></param>
        /// <returns></returns>
        public string[] GetUnlinkedAssembly(string assemblyType)
        {
            string result = "";
            string sCmd = string.Format("SELECT SerialNumber FROM TMflexLinkInfoState WHERE SerialNumber LIKE '{0}%' AND LinkedSerialNumber = ''", assemblyType);
            DataTable dt = SelectQuery(sCmd);
            foreach (DataRow r in dt.Rows)
            {
                result += r["SerialNumber"].ToString() + ",";
            }

            if (result.Length == 0)
                return new string[0] { };
            return result.Substring(0, result.Length - 1).Split(',');
        }

        /// <summary>
        /// This deletes all of the linked assemblies from the linked table state.
        /// Use this when a unit has shipped.
        /// </summary>
        /// <param name="TMXF"></param>
        /// <returns></returns>
        public bool DeleteLinkedAssemblies(TMXF_Model TMXF)
        {
            bool success = false;

            Dictionary<string, string> linkage = TMXF.CreateLinkageDictionary();
            Connection.Open();
            SqlCommand cmd = Connection.CreateCommand();
            cmd.Transaction = Connection.BeginTransaction("UpdateLinkTable");
            try
            {
                foreach (string SerialNumber in linkage.Keys)
                {
                    cmd.CommandText = string.Format("DELETE FROM {0} WHERE SerialNumber='{1}'",
                                                    "TMflexLinkInfoState", SerialNumber);
                    int rows = cmd.ExecuteNonQuery();                   
                }

                cmd.Transaction.Commit();

                success = true;
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
            }
            finally
            {
                Connection.Close();
            }

            return success;
        }

        #endregion 

        #region "Private Methods"


        private List<string> LoadViews()
        {
            List<string> list = new List<string>();
            DataTable dt = SelectQuery("SELECT * FROM Ops_DB_Dev.information_schema.VIEWS");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(row[2].ToString());
            }

            return list;
        }

        private List<string> LoadProcedures()
        {
            List<string> list = new List<string>();
            DataTable dt = SelectQuery("SELECT * FROM Ops_DB_Dev.INFORMATION_SCHEMA.ROUTINES WHERE SPECIFIC_NAME LIKE 'p_%'");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(row[2].ToString());
            }

            return list;
        }

        private bool LinkTMXFObject(ref string serialNumber, TMXF_Model tmxf, DataTable dt, bool direction)
        {
            bool linking = true;
            serialNumber = dt.Rows[0]["SerialNumber"].ToString();
            string linkedSerialNumber = dt.Rows[0]["LinkedSerialNumber"].ToString();
            DateTime updatedDateTime = DateTime.Parse(dt.Rows[0]["UpdatedDateTime"].ToString());
            tmxf.Updated = updatedDateTime;
            if (direction)
            {
                if (linkedSerialNumber == string.Empty)
                {
                    linking = false;
                }
                else if (tmxf.HasSerialNumber(linkedSerialNumber) == false)
                {
                    tmxf.SetSerialNumber(linkedSerialNumber);
                    serialNumber = linkedSerialNumber;
                }
                else
                {
                    linking = false;
                }
            }
            else
            {
                if (tmxf.HasSerialNumber(serialNumber) == false)
                {
                    tmxf.SetSerialNumber(serialNumber);
                    if (linkedSerialNumber == string.Empty)
                    {
                        linking = false;
                    }
                }
                else
                {
                    linking = false;
                }
            }
            return linking;
        }

        private SqlCommand CreateInsertCommand(string Table, Dictionary<string, object> data)
        {
            // Creates the initial command.
            string insertCmd = string.Format("INSERT INTO {0}(", Table);

            // Adds the fields to the command
            foreach (string field in data.Keys)
            {
                insertCmd += string.Format("{0},", field);
            }

            // Removes the extra comma and adds the values command
            insertCmd = insertCmd.Substring(0, insertCmd.Length - 1) + ") VALUES (";

            // Adds the values to the command
            foreach (string field in data.Keys)
            {
                insertCmd += string.Format("@{0},", field);
            }

            // Removes the final comma and finishes the command.
            insertCmd = insertCmd.Substring(0, insertCmd.Length - 1) + ")";

            SqlCommand cmd = new SqlCommand(insertCmd, Connection);
            foreach (string field in data.Keys)
            {
                string param = string.Format("@{0}", field);
                string getType = data[field].GetType().ToString();
                switch (getType)
                {
                    case "System.DateTime":
                        {
                            cmd.Parameters.Add(param, SqlDbType.DateTime);
                            break;
                        }
                    case "System.Byte[]":
                        {
                            cmd.Parameters.Add(param, SqlDbType.Text);
                            byte[] bytes = (byte[])data[field];
                            cmd.Parameters[param].Value = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                            continue;
                        }
                    case "System.String":
                        {
                            if (data[field].ToString().Length > 255)
                            {
                                cmd.Parameters.Add(param, SqlDbType.Text);
                            }
                            else
                            {
                                cmd.Parameters.Add(param, SqlDbType.NVarChar);
                            }
                            break;
                        }
                    default:
                        {
                            cmd.Parameters.Add(param, SqlDbType.Text);
                            break;
                        }


                }
                cmd.Parameters[param].Value = data[field];
            }

            return cmd;
        }

        #endregion 
    }
}
