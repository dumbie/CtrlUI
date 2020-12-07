using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public class HidDeviceCapabilities
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct HIDP_CAPS
        {
            internal short UsageGeneric;
            internal short UsagePage;
            internal short InputReportByteLength;
            internal short OutputReportByteLength;
            internal short FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            internal short[] Reserved;
            internal short NumberLinkCollectionNodes;
            internal short NumberInputButtonCaps;
            internal short NumberInputValueCaps;
            internal short NumberInputDataIndices;
            internal short NumberOutputButtonCaps;
            internal short NumberOutputValueCaps;
            internal short NumberOutputDataIndices;
            internal short NumberFeatureButtonCaps;
            internal short NumberFeatureValueCaps;
            internal short NumberFeatureDataIndices;
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

        public short UsageGeneric { get; set; }
        public short UsagePage { get; set; }
        public short InputReportByteLength { get; set; }
        public short OutputReportByteLength { get; set; }
        public short FeatureReportByteLength { get; set; }
        public short[] Reserved { get; set; }
        public short NumberLinkCollectionNodes { get; set; }
        public short NumberInputButtonCaps { get; set; }
        public short NumberInputValueCaps { get; set; }
        public short NumberInputDataIndices { get; set; }
        public short NumberOutputButtonCaps { get; set; }
        public short NumberOutputValueCaps { get; set; }
        public short NumberOutputDataIndices { get; set; }
        public short NumberFeatureButtonCaps { get; set; }
        public short NumberFeatureValueCaps { get; set; }
        public short NumberFeatureDataIndices { get; set; }
    }
}