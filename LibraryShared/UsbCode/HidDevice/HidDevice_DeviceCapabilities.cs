using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public class HidDeviceCapabilities
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct HIDP_CAPS
        {
            internal ushort UsageGeneric;
            internal ushort UsagePage;
            internal ushort InputReportByteLength;
            internal ushort OutputReportByteLength;
            internal ushort FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            internal ushort[] Reserved;
            internal ushort NumberLinkCollectionNodes;
            internal ushort NumberInputButtonCaps;
            internal ushort NumberInputValueCaps;
            internal ushort NumberInputDataIndices;
            internal ushort NumberOutputButtonCaps;
            internal ushort NumberOutputValueCaps;
            internal ushort NumberOutputDataIndices;
            internal ushort NumberFeatureButtonCaps;
            internal ushort NumberFeatureValueCaps;
            internal ushort NumberFeatureDataIndices;
        }

        public HidDeviceCapabilities(HIDP_CAPS capabilities)
        {
            UsageGeneric = capabilities.UsageGeneric;
            UsagePage = capabilities.UsagePage;
            InputReportByteLength = capabilities.InputReportByteLength;
            OutputReportByteLength = capabilities.OutputReportByteLength;
            FeatureReportByteLength = capabilities.FeatureReportByteLength;
            Reserved = capabilities.Reserved;
            NumberLinkCollectionNodes = capabilities.NumberLinkCollectionNodes;
            NumberInputButtonCaps = capabilities.NumberInputButtonCaps;
            NumberInputValueCaps = capabilities.NumberInputValueCaps;
            NumberInputDataIndices = capabilities.NumberInputDataIndices;
            NumberOutputButtonCaps = capabilities.NumberOutputButtonCaps;
            NumberOutputValueCaps = capabilities.NumberOutputValueCaps;
            NumberOutputDataIndices = capabilities.NumberOutputDataIndices;
            NumberFeatureButtonCaps = capabilities.NumberFeatureButtonCaps;
            NumberFeatureValueCaps = capabilities.NumberFeatureValueCaps;
            NumberFeatureDataIndices = capabilities.NumberFeatureDataIndices;
        }

        public ushort UsageGeneric { get; set; }
        public ushort UsagePage { get; set; }
        public ushort InputReportByteLength { get; set; }
        public ushort OutputReportByteLength { get; set; }
        public ushort FeatureReportByteLength { get; set; }
        public ushort[] Reserved { get; set; }
        public ushort NumberLinkCollectionNodes { get; set; }
        public ushort NumberInputButtonCaps { get; set; }
        public ushort NumberInputValueCaps { get; set; }
        public ushort NumberInputDataIndices { get; set; }
        public ushort NumberOutputButtonCaps { get; set; }
        public ushort NumberOutputValueCaps { get; set; }
        public ushort NumberOutputDataIndices { get; set; }
        public ushort NumberFeatureButtonCaps { get; set; }
        public ushort NumberFeatureValueCaps { get; set; }
        public ushort NumberFeatureDataIndices { get; set; }
    }
}