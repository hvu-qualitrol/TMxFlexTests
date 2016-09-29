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
    public class FA_Update_Test_Data : Abstract_Update_Test_Data
    {

        public FA_Update_Test_Data()
            : base()
        {
            HistoryDataTable = "TMflexGcHistory";
            TestDataTable = "TMflexGcTestData";
            testDataRowFields[0] = "Link_Id";
            testDataRowFields[1] = "SerialNumber";
            testDataRowFields[2] = "TimeDate";
            testDataRowFields[3] = "Station";
            testDataRowFields[4] = "UserName";
            testDataRowFields[5] = "TestName";
            testDataRowFields[6] = "TestResult";
            testDataRowFields[7] = "TestTime";
        }

        public override bool UpdateTestData(DataFile testData)
        {
            return GeneralizedTestData(testData, "FaAssembly");
        }       
    }
}
