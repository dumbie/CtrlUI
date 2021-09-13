using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public class HidDeviceAttributes
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct HIDD_ATTRIBUTES
        {
            internal int Size;
            internal ushort VendorID;
            internal ushort ProductID;
            internal ushort VersionNumber;
        }

        public HidDeviceAttributes(HIDD_ATTRIBUTES attributes)
        {
            VendorId = attributes.VendorID;
            ProductId = attributes.ProductID;
            Version = attributes.VersionNumber;
            VendorHexId = "0x" + attributes.VendorID.ToString("X4");
            ProductHexId = "0x" + attributes.ProductID.ToString("X4");
        }

        public int VendorId { get; set; }
        public int ProductId { get; set; }
        public int Version { get; set; }
        public string VendorHexId { get; set; }
        public string ProductHexId { get; set; }
        public string VendorName { get; set; }
        public string ProductName { get; set; }
        public string SerialNumber { get; set; }
    }
}