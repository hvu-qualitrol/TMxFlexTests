using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLib.DataFiles
{
    public abstract class DataFile 
    {
        public string[] Fields = new string[1] {"Base"};
        public int FieldCount = 1;
        public string[] SeperatedData = new string[0];
        
        public string TestName { get; set; }        
        public int Rows { get; set; }
        public DateTime TestTimeStamp { get; set; }
        public string FileName { get; set; }
        public char Seperator = ' ';
        public string Text = "";

        public DataFile()
        {
        }

        public virtual void Init()
        {
            FieldCount = Fields.Count<string>();
            Rows = SeperatedData.Count<string>() / FieldCount;
            TestTimeStamp = DateTime.Parse(GetValue("TimeStamp", 1));
            
        }

        public DataFile(string data, string fileName)
        {
            FileName = fileName;
            Text = data;
            string[] tempData = data.Replace("\r\n", "^").Split('^');
            string[] tempTestName = fileName.Split('_');
            if (tempTestName.Count<string>() > 1)
            {
                TestName = tempTestName[1];
            }
            else
            {
                TestName = System.IO.Path.GetFileNameWithoutExtension(tempTestName[0]);
            }
            SeperatedData = new string[tempData.Count<string>()];
            tempData.CopyTo(SeperatedData, 0);            
        }       

        public string GetValue(string Field, int row)
        {
            string value = null;
            string[] sTemp = SeperatedData[row].Split(Seperator);      
            int fIndex = Utilities.Fields.FieldIndex(Fields, Field);
            if (fIndex >= 0)
            {
                value = sTemp[fIndex];
            }
            return value;
        }

        public string[] GetRow(int row)
        {
            string[] tempRow = new string[FieldCount];
            int count = 0;
            foreach (string field in Fields)
            {
                tempRow[count] = GetValue(field, row);
                count++;
            }

            return tempRow;
        }

        public Double TotalTestTime
        {
            get
            {
                return DateTime.Parse(GetValue("TimeStamp", Rows - 1)).Subtract(DateTime.Parse(GetValue("TimeStamp", 1))).TotalSeconds;
            }
        }

        public bool TestResult        
        {
            get
            {
                bool passFail = true;
                for (int i = 1; i <= Rows-1; i++)
                {
                    if (GetValue("PassFail", i) == "1" | GetValue("PassFail", i) == "true")
                    {
                        passFail = passFail & true;
                    }
                    else
                    {
                        passFail = passFail & false;
                    }
                    
                }

                return passFail;
            }
        }

    }
}
