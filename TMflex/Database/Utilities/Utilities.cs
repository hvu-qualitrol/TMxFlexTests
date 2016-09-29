using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class Fields
    {
        public static int FieldIndex(string[] Fields, string Field)
        {
            for (int i = 0; i <= Fields.Count(); i++)
            {
                if (Fields[i] == Field)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
