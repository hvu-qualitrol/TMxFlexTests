using FileLib.DataFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib.Update
{
    public abstract class Abstract_Update_Test_Data
    {
        public abstract bool UpdateTestData(DataFile testData);
        private const string GetOneRow = "SELECT TOP 1 * FROM ";
        public string HistoryDataTable = "";
        public string TestDataTable = "";
        public string SerialNumber { get; set; }

        protected string[] testDataRow = new string[8];
        protected string[] testDataRowFields = new string[8];

        protected bool UpdateHistoryDataRow(string datatable, string[] row, string[] columns)
        {
            DataTable dt = CreateDatatable(datatable, row, columns);
            bool success = false;
            if (dt.Rows.Count > 0)
            {
                int rtn = DBSingleton.Instance.Adapter.Update(dt);
                if (rtn > 0)
                {
                    success = true;
                }
                else
                {
                    success = false;
                }
            }
            return success;
        }

        protected int UpdateDataRow(string datatable, string[] row, string[] columns)
        {
            int ID = -2;
            DataTable dt = CreateDatatable(datatable, row, columns);
            if (dt.Rows.Count > 0)
            {
                DBSingleton.Instance.SetDatatable(dt);
                DBSingleton.Instance.Command = DBSingleton.Instance.CmdBuilder.GetUpdateCommand();
                int rtn = DBSingleton.Instance.Adapter.Update(dt);
                if (rtn > 0)
                {
                    ID = DBSingleton.Instance.GetID(datatable, DateTime.Parse(row[2].ToString()));

                }
            }

            return ID;
            
        }

        protected virtual bool GeneralizedTestData(DataFile testData, string AssemblyLevel) 
        {
            bool success = true;           
            DataTable dt = null;
            int ID = -1;
            int LinkID;
            SerialNumber = testData.GetValue("SerialNumber", 1);
            dt = DBSingleton.Instance.SelectQuery(string.Format("SELECT TOP 1 P_Id FROM TMFlexLinkInfoState WHERE {0} = '{1}' ORDER BY TimeDate", AssemblyLevel, SerialNumber));
            if (dt.Rows.Count == 1)
            {
                LinkID = int.Parse(dt.Rows[0]["P_Id"].ToString());
            }
            else
            {
                DBSingleton.Instance.WriteLog(string.Format("SN {0} is missing from the Linktable.", SerialNumber));
                return false;
            }
            testDataRow[0] = LinkID.ToString();
            testDataRow[1] = SerialNumber;
            testDataRow[2] = testData.TestTimeStamp.ToString();
            testDataRow[3] = Dns.GetHostName();
            testDataRow[4] = testData.GetValue(testDataRowFields[4], testData.Rows - 1);
            testDataRow[5] = testData.TestName;
            if (testData.TestResult)
            {
                testDataRow[6] = "Pass";
            }
            else
            {
                testDataRow[6] = "Fail";
            }
            testDataRow[7] = testData.TotalTestTime.ToString();

            ID = UpdateDataRow(HistoryDataTable, testDataRow, testDataRowFields);

            if (ID != -1)
            {
                // Starting at 1 since the first line is a header.
                for (int i = 1; i <= testData.Rows - 1; i++)
                {
                    string[] dataRow = new string[testData.FieldCount + 4];
                    string[] dataFields = new string[testData.FieldCount + 4];
                    int counter = 0;
                    foreach (string s in testData.GetRow(i))
                    {
                        dataRow[counter] = s;
                        dataFields[counter] = testData.Fields[counter];
                        counter++;
                    }

                    dataFields[counter] = testDataRowFields[5];
                    dataRow[counter] = testData.TestName;
                    counter++;

                    Dictionary<string, object> spec = GetSpec(dataRow[3]);
                    if (spec.Count > 1)
                    {
                        dataFields[counter] = "SpecId";
                        dataRow[counter] = spec["P_Id"].ToString();
                        counter++;
                    }
                    else
                    {
                        DBSingleton.Instance.WriteLog(string.Format("Missing Spec Entry {0}", dataRow[3].ToString()));
                        return false;
                    }

                    dataFields[counter] = "TestId";
                    dataRow[counter] = ID.ToString();
                    counter++;

                    dataFields[counter] = "TimeDate";
                    dataRow[counter] = testData.TestTimeStamp.ToString();
                    counter++;


                    if (UpdateDataRow(TestDataTable, dataRow, dataFields) > -2)
                    {
                        success = success & true;
                    }
                    else
                    {
                        success = success & false;
                    }
                    
                }
            }
            return success;
        }

        public Dictionary<string, object> GetSpec(string SpecName)
        {
            Dictionary<string, object> specDictionary = new Dictionary<string, object>();
            DataTable dt = DBSingleton.Instance.SelectQuery(string.Format("SELECT TOP 1 * FROM TMFlexTestSpecs WHERE SpecName LIKE '{0}%' Order By TimeDate", SpecName));
            for (int i = 0; i <= dt.Columns.Count - 1; i++)
            {
                specDictionary.Add(dt.Columns[i].ToString(), dt.Rows[0][i]);
            }

            if (specDictionary.Count == 0)
            {
                DBSingleton.Instance.WriteLog("Missing Spec : " + SpecName);
            }

            return specDictionary;
        }

        private DataTable GetRow(string datatable)
        {
            return DBSingleton.Instance.SelectQuery(GetOneRow  + datatable);
        }

        private DataTable CreateDatatable(string datatable, string[] row, string[] columns)
        {
            DataTable dt = GetRow(datatable);
            DataRow dRow = dt.NewRow();
            dRow.BeginEdit();
            foreach (DataColumn column in dt.Columns)
            {
                string columnName = column.ColumnName;
                if (columns.Contains<string>(columnName))
                {                    
                    for (int i = 0; i <= columns.Count<string>() - 1; i++)
                    {                        
                        if (columns[i] == columnName)
                        {
                            string type = dt.Columns[columnName].DataType.ToString();
                            switch (type)
                            {
                                case "System.Int32":
                                    {
                                        dRow[columns[i]] = Int32.Parse(row[i]);
                                        break;
                                    }
                                case "System.DateTime":
                                    {
                                        dRow[columns[i]] = DateTime.Parse(row[i]);
                                        break;
                                    }
                                case "System.Double":
                                    {
                                        dRow[columns[i]] = Double.Parse(row[i]);
                                        break;
                                    }
                                case "System.Boolean":
                                    {
                                        if (row[i] == "1")
                                        {
                                            dRow[columns[i]] = true;
                                        }
                                        else if (row[i] == "0")
                                        {
                                            dRow[columns[i]] = false;
                                        }
                                        else if (row[i] == "false")
                                        {
                                            dRow[columns[i]] = false;
                                        }
                                        else if (row[i] == "true")
                                        {
                                            dRow[columns[i]] = true;
                                        }                                         
                                        break;
                                    }
                                default:
                                    {
                                        dRow[columns[i]] = row[i];
                                        break;
                                    }

                            }
                        }
                    }
                }
                dRow.EndEdit();
            }

            dt.Rows.Add(dRow);

            return dt;
        }
    }
}