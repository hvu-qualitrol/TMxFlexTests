using DatabaseLib;
using FileLib;
using FileLib.DataFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {        
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {          
            timer1.Start();
        }

        protected override void OnStop()
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            ParseFiles();
        }

        /// <summary>
        /// This is where all of the work is done. It parses the files, updates the database, and moves them to a new location.
        /// </summary>
        private void ParseFiles()
        {
            List<string> files = new List<string>();
            List<string> destFiles = new List<string>();
            foreach (string fileStr in Directory.EnumerateFiles(DBSingleton.Instance.DataFilePath))
            {
                try
                {                    
                    StreamReader reader = new StreamReader(fileStr);
                    string contents = reader.ReadToEnd();
                    reader.Close();
                    DataFile data = Test_Data_Factory.Create_Test_Data(contents, Path.GetFileName(fileStr));
                    DBSingleton.Instance.WriteLog(string.Format("Pushing test data: {0}", fileStr));
                    DBSingleton.Instance.UpdateData(data);
                    files.Add(fileStr);
                    int lastIndex = data.GetType().ToString().LastIndexOf('.') + 1;
                    string type = data.GetType().ToString().Substring(lastIndex, data.GetType().ToString().Length - lastIndex);
                    string destStr = "C:\\Data\\" + type + "\\" + Path.GetFileName(fileStr);
                    destFiles.Add(destStr);
                }
                catch (Exception ex)
                {
                    DBSingleton.Instance.WriteLog("Error Message: " + ex.Message);
                }
            }
            for (int i = 0; i <= files.Count - 1; i++)
            {
                File.Move(files[i], destFiles[i]);
            }

        } 
    }
}
