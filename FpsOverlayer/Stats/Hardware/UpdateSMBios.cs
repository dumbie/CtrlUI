using LibreHardwareMonitor.Hardware;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowStats
    {
        void UpdateSMBiosInformation(SMBios smBios)
        {
            try
            {
                //Set motherboard name
                vHardwareMotherboardName = smBios.Board.ManufacturerName + " " + smBios.Board.ProductName;

                //Filter motherboard manufacturer
                vHardwareMotherboardName = vHardwareMotherboardName.Replace("To be filled by O.E.M.", "O.E.M.");
                vHardwareMotherboardName = vHardwareMotherboardName.Replace(" Technology", string.Empty);
                vHardwareMotherboardName = vHardwareMotherboardName.Replace(" Ltd.", string.Empty);
                vHardwareMotherboardName = vHardwareMotherboardName.Replace(" Ltd", string.Empty);
                vHardwareMotherboardName = vHardwareMotherboardName.Replace(" Co.,", string.Empty);
                vHardwareMotherboardName = vHardwareMotherboardName.Replace(" Co.", string.Empty);

                //Set memory details
                int memoryCount = 0;
                foreach (MemoryDevice memoryDevice in smBios.MemoryDevices)
                {
                    if (memoryDevice.Size > 0)
                    {
                        memoryCount++;
                        vHardwareMemoryName = memoryDevice.ManufacturerName + " " + memoryDevice.PartNumber + " (" + memoryCount + "X) " + memoryDevice.Type;
                        vHardwareMemorySpeed = memoryDevice.ConfiguredSpeed + "MTs";
                        vHardwareMemoryVoltage = (memoryDevice.ConfiguredVoltage / 1000F).ToString("0.000") + "V";
                    }
                }
            }
            catch { }
        }
    }
}