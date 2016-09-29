using FileLib.DataFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLib
{
    public class Test_Data_Factory
    {
        public enum AssemblyTypes { GC, FA, OC, PAS, QTMS, Xtr };
       // private static const string[] AssemblyTypes = new string[] { "GC", "FA", "OC", "PAS", "QTMS", "Xtr" };
        public static DataFile Create_Test_Data(string Contents_Data, string FileName)
        {
            foreach (AssemblyTypes assemblyType in Enum.GetValues(typeof(AssemblyTypes)))
            {
                string type = assemblyType.ToString();
                if (Contents_Data.Contains(type))
                {
                    return CreateStructure(assemblyType, Contents_Data, FileName);
                }
            }

            return null;
        }

        private static DataFile CreateStructure(AssemblyTypes assemblyType, string Contents_Data, string FileName)
        {
            DataFile file = null;
            switch (assemblyType)
            {
                case AssemblyTypes.FA:
                    {
                        file = new FA_Test_Data(Contents_Data, FileName);
                        break;
                    }
                case AssemblyTypes.GC:
                    {
                        file = new GC_Test_Data(Contents_Data, FileName);
                        break;
                    }
                case AssemblyTypes.OC:
                    {
                        file = new Oil_Test_Data(Contents_Data, FileName);
                        break;
                    }
                case AssemblyTypes.PAS:
                    {
                        if (Path.GetExtension(FileName) == ".txt")
                        {
                            if (Contents_Data.Contains("#GASID="))
                            {
                                file = new PAS_Calibration_Data(Contents_Data, FileName);
                            }
                            else
                            {
                                file = new PAS_Test_Data(Contents_Data, FileName);
                            }
                        }
                        break;
                    }
                case AssemblyTypes.QTMS:
                    {
                        file = new QTMS_Test_Data(Contents_Data, FileName);
                        break;
                    }
                case AssemblyTypes.Xtr:
                    {
                        file = new Xtr_Test_Data(Contents_Data, FileName);
                        break;
                    }
            }

            return file;
        }
    }
}
