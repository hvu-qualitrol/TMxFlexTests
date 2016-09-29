using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLib.DataFiles
{
    public class FA_Test_Data : DataFile
    {
        public FA_Test_Data(string dataFile, string fileName)
            : base(dataFile, fileName)
        {
            Fields = new string[] {"SerialNumber","TestName","TimeStamp","SpecName","ResultValue",
                "SpecMin","SpecMax","ResultText","SpecEqual","SpecUnit","PassFail","UserName"};
            Init();
        }
    }
}
