using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace LibraryShared
{
    public partial class Classes
    {
        [Guid("5842A140-FF9F-4166-8F5C-62F5B7B0C781"), ComImport]
        public class AppxFactory { }

        [Guid("BEB94909-E451-438B-B5A7-D79E767B75D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAppxFactory
        {
            void _VtblGap0_2();
            IAppxManifestReader CreateManifestReader(IStream inputStream);
        }

        [Guid("4E1BD148-55A0-4480-A3D1-15544710637C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAppxManifestReader
        {
            IAppxManifestPackageId GetPackageId();
            IAppxManifestProperties GetProperties();
            void _VtblGap1_5();
            IAppxManifestApplicationsEnumerator GetApplications();
        }

        [Guid("283CE2d7-7153-4A91-9649-7A0F7240945F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]

        public interface IAppxManifestPackageId : IDisposable
        {
            [return: MarshalAs(UnmanagedType.LPWStr)]
            string GetName();

            APPX_PACKAGE_ARCHITECTURE GetArchitecture();

            [return: MarshalAs(UnmanagedType.LPWStr)]
            string GetPublisher();

            ulong GetVersion();

            [return: MarshalAs(UnmanagedType.LPWStr)]
            string GetResourceId();

            bool ComparePublisher([In, MarshalAs(UnmanagedType.LPWStr)] string otherPublisher);

            [return: MarshalAs(UnmanagedType.LPWStr)]
            string GetPackageFullName();

            [return: MarshalAs(UnmanagedType.LPWStr)]
            string GetPackageFamilyName();
        }

        [Guid("03FAF64D-F26F-4B2C-AAF7-8FE7789B8BCA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAppxManifestProperties : IDisposable
        {
            [PreserveSig]
            bool GetBoolValue([In, MarshalAs(UnmanagedType.LPWStr)] string propertyName);
            [PreserveSig]
            void GetStringValue([In, MarshalAs(UnmanagedType.LPWStr)] string propertyName, [Out, MarshalAs(UnmanagedType.LPWStr)] out string value);
        }

        [Guid("9EB8A55A-F04B-4D0D-808D-686185D4847A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAppxManifestApplicationsEnumerator
        {
            IAppxManifestApplication GetCurrent();
            bool HasCurrent();
            bool MoveNext();
        }

        [Guid("5DA89BF4-3773-46BE-B650-7E744863B7E8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAppxManifestApplication : IDisposable
        {
            [PreserveSig]
            void GetStringValue([In, MarshalAs(UnmanagedType.LPWStr)] string propertyName, [Out, MarshalAs(UnmanagedType.LPWStr)] out string value);
        }

        public enum APPX_PACKAGE_ARCHITECTURE
        {
            APPX_PACKAGE_ARCHITECTURE_X86 = 0,
            APPX_PACKAGE_ARCHITECTURE_ARM = 5,
            APPX_PACKAGE_ARCHITECTURE_X64 = 9,
            APPX_PACKAGE_ARCHITECTURE_NEUTRAL = 11,
            APPX_PACKAGE_ARCHITECTURE_ARM64 = 12
        }
    }
}