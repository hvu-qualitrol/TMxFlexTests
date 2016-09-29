using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiGasTanks
{
    public class MultiGas
    {
        public string[] Gases = new string[] { "CH4", "C2H6", "C2H4", "C2H2", "CO", "CO2", "H2O", "O2", "H2", "C3H8", "C3H6" };

        public List<float> GasConc = new List<float>   { 1000000, 200000, 250, 151, 7990, 20000, 12050, 8914, 60200, 50.1f, 5996, 20013, 1336 };
        public List<string> GasList = new List<string> { "N2", "CO", "C2H4", "C2H6", "C2H6", "CO", "C2H4", "C2H2", "CO2", "C2H2", "CO2", "CO", "CH4" };
        public List<int> Port = new List<int>          {   1,    2,    3,       4,      5,     6,     7,      2,     2,      6,     6,     6,     6 };

        public GasArray gasArray = new GasArray();

        public MultiGas()
        {
        }

        public int GasIndex(string[] gasArray, string GasName)
        {
            int g = 0;

            for (int i = 0; i <= gasArray.Length - 1; i++)
            {
                if (gasArray[i] == GasName)
                {
                    g = i;
                }
            }

            return g;
        }

        public string[] GetGasesForPort(int port)
        {
            string[] gasList = new string[20];
            int count = 0;
            foreach (int portInt in Port)
            {
                if (portInt == port)
                {
                    gasList[count] = GasList[portInt];
                    count++;
                }
            }

            return gasList;
        }

        /// <summary>
        /// Returns a list of the gases not found in the data.
        /// </summary>
        /// <param name="gasData">string of all the data</param>
        /// <returns>array of all the missing gases</returns>
        public string[] GetGasesNotFound(string gasData)
        {
            string[] gasList = new string[10];
            int count = 0;
            foreach (string gas in Gases)
            {
                if (!gasData.Contains(gas))
                {
                    gasList[count] = gas;
                    count++;
                }
            }

            return gasList;
        }

        public int GetPortForGas(string gas)
        {
            for (int i = 0; i < Gases.Length - 1; i++ )
            {
                if (PortContainsGas(i, gas))
                {
                    return i;
                }
            }
            return -1;
        }

        public string PrimaryGas(string gas, string gasData)
        {
            int port = GetPortForGas(gas);
            string foundGas = "";
            foreach (string portGas in GetGasesForPort(port))
            {
                if (gasData.Contains(portGas))
                {
                    foundGas = portGas;
                }
            }

            return foundGas;
        }

        public bool PortContainsGas(int port, string gas)
        {
            bool exists = false;
            for (int i = 0; i <= GasList.Count - 1; i++)
            {
                if (port == Port[i])
                {
                    if (GasList[i] == gas)
                    {
                        exists = true;
                    }
                }
            }

            return exists;
        }

        private float GetTankPPMValues(int port, string gasName)
        {
            float result = 0.0f;
            for (int i = 0; i <= GasList.Count - 1; i++)
            {
                if (port == Port[i])
                {
                    if (GasList[i] == gasName)
                    {
                        result = GasConc[i];
                    }
                }
            }

            return result;
        }

        public GasArray GetResults(string primaryGas, float gasConcentration, int port)
        {
            gasArray = new GasArray(); 
            for (int i = 0; i <= Gases.Length - 1; i++)
            {
                string currentGas = Gases[i];
                int gasIndex = GasIndex(Gases, currentGas);
                float primaryGasPPM = GetTankPPMValues(port, primaryGas);
                               
                if (PortContainsGas(port, currentGas))
                {
                    float ppmValue = GetTankPPMValues(port, currentGas);
                    if (primaryGas == currentGas)
                    {
                        gasArray.PPMResults(GasIndex(Gases, currentGas), gasConcentration);                        
                    }
                    else
                    {
                        gasArray.PPMResults(GasIndex(Gases, currentGas), (gasConcentration / primaryGasPPM) * ppmValue);    
                    }
                }
            }

            return gasArray;
        }
    }

    public class GasArray
    {
        public float CH4 { get; set; }
        public float C2H6 { get; set; }
        public float C2H4 { get; set; }
        public float C2H2 { get; set; }
        public float CO { get; set; }
        public float CO2 { get; set; }
        public float H2O { get; set; }
        public float O2 { get; set; }
        public float H2 { get; set; }
        public float C3H8 { get; set; }
        public float C3H6 { get; set; }

        public GasArray()
        {
            for (int i = 0; i <= 10; i++)
            {
                PPMResults(i, 0.0f);
            }
        }

        public void PPMResults(int index, float result)
        {
            switch (index)
            {
                case 0:
                    {
                        CH4 = result;
                        break;
                    }
                case 1:
                    {
                        C2H6 = result;
                        break;
                    }
                case 2:
                    {
                        C2H4 = result;
                        break;
                    }
                case 3:
                    {
                        C2H2 = result;
                        break;
                    }
                case 4:
                    {
                        CO = result;
                        break;
                    }
                case 5:
                    {
                        CO2 = result;
                        break;
                    }
                case 6:
                    {
                        H2O = result;
                        break;
                    }
                case 7:
                    {
                        O2 = result;
                        break;
                    }
                case 8:
                    {
                        H2 = result;
                        break;
                    }
                case 9:
                    {
                        C3H8 = result;
                        break;
                    }
                case 10:
                    {
                        C3H6 = result;
                        break;
                    }
            }
        }

       

    }
}
