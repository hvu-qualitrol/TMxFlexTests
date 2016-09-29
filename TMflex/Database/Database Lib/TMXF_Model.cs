using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class TMXF_Model
    {
        public DateTime Updated { get; set; }
        public FinalAssemblyModel FinalAssembly { get; set; }
        public QTMSAssemblyModel QTMSAssembly { get; set; }
        public PASAssemblyModel PASAssembly { get; set; }
        public OCAssemblyModel OCAssembly { get; set; }
        public GCAssemblyModel GCAssembly { get; set; }
        public ExtAssemblyModel ExtAssembly { get; set; }
        public HydrogenSensorModel HydrogenSensor { get; set; }
        public OxygenSensorModel OxygenSensor { get; set; }
        public ChemTrapModel ChemTrap { get; set; }
        public PowerSupplyModel PowerSupply { get; set; }
        public byte[] Blob { get; set; }

        public TMXF_Model()
        {
            Updated = DateTime.MinValue;
            FinalAssembly = new FinalAssemblyModel();
            QTMSAssembly = new QTMSAssemblyModel();
            PASAssembly = new PASAssemblyModel();
            OCAssembly = new OCAssemblyModel();
            GCAssembly = new GCAssemblyModel();
            ExtAssembly = new ExtAssemblyModel();
            HydrogenSensor = new HydrogenSensorModel();
            OxygenSensor = new OxygenSensorModel();
            ChemTrap = new ChemTrapModel();
            PowerSupply = new PowerSupplyModel();
            Blob = new byte[6000];
        }

        public string LinkString
        {
            get
            {
                return string.Format("QTMS: {0}, Final: {1}, PAS: {2}, GC: {3}, OC: {4}, Ext: {5}, H2Sens: {6}, O2Sens: {7}, Chem: {8}, PS: {9}",
                    QTMSAssembly.SerialNumber, FinalAssembly.SerialNumber, PASAssembly.SerialNumber, GCAssembly.SerialNumber, OCAssembly.SerialNumber,
                    ExtAssembly.SerialNumber, HydrogenSensor.SerialNumber, OxygenSensor.SerialNumber, ChemTrap.SerialNumber, PowerSupply.SerialNumber);
            }
        }

        public string GetNextSerialNumber(string serialNumber)
        {
            string nextSN = "";
            if (!EmptyOrNull(serialNumber))
            {
                string snType = serialNumber.Substring(0, 2);
                if (snType == "OC")
                {
                    if (!EmptyOrNull(GCAssembly.SerialNumber))
                        return GCAssembly.SerialNumber;
                    snType = "GC";
                }
                if (snType == "GC")
                {
                    if (!EmptyOrNull(HydrogenSensor.SerialNumber))
                        return HydrogenSensor.SerialNumber;
                    snType = "HS";
                }
                if (snType == "HS")
                {
                    if (!EmptyOrNull(OxygenSensor.SerialNumber))
                        return OxygenSensor.SerialNumber;
                    snType = "OS";
                }
                if (snType == "OS")
                {
                    if (!EmptyOrNull(ExtAssembly.SerialNumber))
                        return ExtAssembly.SerialNumber;
                    snType = "EX";
                }
                if (snType == "EX")
                {
                    if (!EmptyOrNull(ChemTrap.SerialNumber))
                        return ChemTrap.SerialNumber;
                    snType = "CT";
                }
                if (snType == "CT")
                {
                    if (!EmptyOrNull(PASAssembly.SerialNumber))
                        return PASAssembly.SerialNumber;
                    snType = "PA";
                }
                if (snType == "PA")
                {
                    if (!EmptyOrNull(PowerSupply.SerialNumber))
                        return PowerSupply.SerialNumber;
                    snType = "PS";
                }
                if (snType == "PS")
                {
                    if (!EmptyOrNull(QTMSAssembly.SerialNumber))
                        return QTMSAssembly.SerialNumber;
                    snType = "QT";
                }
                if (snType == "QT")
                {
                    if (!EmptyOrNull(FinalAssembly.SerialNumber))
                        return FinalAssembly.SerialNumber;
                    snType = "TM";
                }
            }
            else
            {
                nextSN = GetNextSerialNumber("OC000000");
            }
            return nextSN;
        }

        public bool EmptyOrNull(string serialNumber)
        {
            if (serialNumber == null | serialNumber == string.Empty)
            {
                return true;
            }

            return false;
        }

        public bool HasSerialNumber(string serialNumber)
        {            
            return !EmptyOrNull(GetSerialNumber(serialNumber));
        }

        /// <summary>
        /// Uses the 2 letter serial number indicator
        /// </summary>
        /// <param name="assemblyType"></param>
        public string GetSerialNumber(string assemblyType)
        {
            string result = string.Empty;
            SubAssemblyModel sAssem = GetSubAssembly(assemblyType);
            if (sAssem != null)
                return sAssem.SerialNumber;
            return "";
        }
        public SubAssemblyModel GetSubAssembly(string serialNumber)
        {
            SubAssemblyModel sAssem = null;
            if (!EmptyOrNull(serialNumber))
            {
                switch (serialNumber.Substring(0, 2))
                {
                    case "TM":
                        {
                            sAssem = FinalAssembly;
                            break;
                        }
                    case "QT":
                        {
                            sAssem = QTMSAssembly;
                            break;
                        }
                    case "PA":
                        {
                            sAssem = PASAssembly;
                            break;
                        }
                    case "OC":
                        {
                            sAssem = OCAssembly;
                            break;
                        }
                    case "GC":
                        {
                            sAssem = GCAssembly;
                            break;
                        }
                    case "HS":
                        {
                            sAssem = HydrogenSensor;
                            break;
                        }
                    case "OS":
                        {
                            sAssem = OxygenSensor;
                            break;
                        }
                    case "EX":
                        {
                            sAssem = ExtAssembly;
                            break;
                        }
                    case "CT":
                        {
                            sAssem = ChemTrap;
                            break;
                        }
                    case "PS":
                        {
                            sAssem = PowerSupply;
                            break;
                        }
                }
            }

            return sAssem;
        }

        public void SetSerialNumber(string serialNumber)
        {
            SubAssemblyModel sAssem = GetSubAssembly(serialNumber);
            if (sAssem != null)
            {
                sAssem.SerialNumber = serialNumber;
                sAssem.LinkChanged = false;
            }
        }

        public void AddEmptyLinks(Dictionary<string, string> linkage)
        {
            if (OCAssembly.LinkChanged)            
                modChangedLinkage(linkage, OCAssembly.OldLinkedSN);
            
            if (GCAssembly.LinkChanged)           
                modChangedLinkage(linkage, GCAssembly.OldLinkedSN);
            
            if (PowerSupply.LinkChanged)            
                modChangedLinkage(linkage, PowerSupply.OldLinkedSN);

            if (ChemTrap.LinkChanged)
                modChangedLinkage(linkage, ChemTrap.OldLinkedSN);
            
            if (PowerSupply.LinkChanged)
                modChangedLinkage(linkage, PowerSupply.OldLinkedSN);

            if (ExtAssembly.LinkChanged)
                modChangedLinkage(linkage, ExtAssembly.OldLinkedSN);

            if (OxygenSensor.LinkChanged)
                modChangedLinkage(linkage, OxygenSensor.OldLinkedSN);

            if (HydrogenSensor.LinkChanged)
                modChangedLinkage(linkage, HydrogenSensor.OldLinkedSN);

            if (PASAssembly.LinkChanged)
                modChangedLinkage(linkage, PASAssembly.OldLinkedSN);

            if (QTMSAssembly.LinkChanged)
                modChangedLinkage(linkage, QTMSAssembly.OldLinkedSN);

            if (FinalAssembly.LinkChanged)
                modChangedLinkage(linkage, FinalAssembly.OldLinkedSN);

        }

        private void modChangedLinkage(Dictionary<string, string> linkage, string serialNumber)
        {
            if (!linkage.Keys.Contains(serialNumber))
            {
                linkage.Add(serialNumber, "");
            }
        }

        public Dictionary<string, string> CreateLinkageDictionary()
        {
            // Setup the linkage
            Dictionary<string, string> linkage = new Dictionary<string, string>();
            string sn = OCAssembly.SerialNumber;
            bool loop = true;
            while (loop)
            {
                string foundSN = GetNextSerialNumber(sn);
                if (EmptyOrNull(foundSN))
                {
                    linkage.Add(sn, "");
                    loop = false;
                }
                else
                {
                    if (EmptyOrNull(sn))
                    {
                        sn = foundSN;
                    }
                    else
                    {
                        linkage.Add(sn, foundSN);
                        sn = foundSN;
                    }
                }
            }

            return linkage;
        }
    }

    public class SubAssemblyModel
    {
        public bool LinkChanged { get; set; }
        public string OldLinkedSN { get; set; }

        private string _sn = "";

        public string SerialNumber
        {
            get
            {
                return _sn;
            }
            set
            {
                if (_sn != value)
                {                    
                    if (_sn != "")
                    {
                        OldLinkedSN = _sn;
                        LinkChanged = true;
                    }
                    else
                    {
                        OldLinkedSN = value;
                    }
                    _sn = value;
                }
            }
        }
    }

    public class FinalAssemblyModel : SubAssemblyModel
    {
        
    }

    public class QTMSAssemblyModel : SubAssemblyModel
    {
    }

    public class PASAssemblyModel : SubAssemblyModel
    {
    }

    public class OCAssemblyModel : SubAssemblyModel
    {
    }

    public class GCAssemblyModel : SubAssemblyModel
    {
    }

    public class ExtAssemblyModel : SubAssemblyModel
    {
    }

    public class HydrogenSensorModel : SubAssemblyModel
    {
    }

    public class OxygenSensorModel : SubAssemblyModel
    {
    }

    public class ChemTrapModel : SubAssemblyModel
    {
    }

    public class PowerSupplyModel : SubAssemblyModel
    {
    }    
}
