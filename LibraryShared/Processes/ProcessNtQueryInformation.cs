using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LibraryShared
{
    class ProcessNtQueryInformation
    {
        public static string GetProcessParameterstring(int ProcessId, USER_PROCESS_PARAMETERS RequestedProcessParameter)
        {
            string Parameterstring = string.Empty;
            try
            {
                //Open the process for reading
                IntPtr openProcessHandle = OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryRead, false, ProcessId);
                if (openProcessHandle == IntPtr.Zero)
                {
                    //Debug.WriteLine("Failed to open the process.");
                    return Parameterstring;
                }

                //Check if Windows is 64 bit
                bool Windows64bits = IntPtr.Size > 4;

                //Set the parameter offset
                long userParameterOffset = 0;
                long processParametersOffset = Windows64bits ? 0x20 : 0x10;
                if (RequestedProcessParameter == USER_PROCESS_PARAMETERS.CurrentDirectoryPath) { userParameterOffset = Windows64bits ? 0x38 : 0x24; }
                else if (RequestedProcessParameter == USER_PROCESS_PARAMETERS.ImagePathName) { userParameterOffset = Windows64bits ? 0x60 : 0x38; }
                else if (RequestedProcessParameter == USER_PROCESS_PARAMETERS.CommandLine) { userParameterOffset = Windows64bits ? 0x70 : 0x40; }

                //Read information from process
                PROCESS_BASIC_INFORMATION process_basic_information = new PROCESS_BASIC_INFORMATION();
                int ntQuery = NtQueryInformationProcess(openProcessHandle, PROCESSINFOCLASS.ProcessBasicInformation, ref process_basic_information, process_basic_information.Size, IntPtr.Zero);
                if (ntQuery != 0)
                {
                    Debug.WriteLine("Failed to query information.");
                    return Parameterstring;
                }

                IntPtr process_parameter = new IntPtr();
                long pebBaseAddress = process_basic_information.PebBaseAddress.ToInt64();
                if (!ReadProcessMemory(openProcessHandle, new IntPtr(pebBaseAddress + processParametersOffset), ref process_parameter, new IntPtr(Marshal.SizeOf(process_parameter)), IntPtr.Zero))
                {
                    Debug.WriteLine("Failed to read parameter address.");
                    return Parameterstring;
                }

                UNICODE_string unicode_string = new UNICODE_string();
                if (!ReadProcessMemory(openProcessHandle, new IntPtr(process_parameter.ToInt64() + userParameterOffset), ref unicode_string, new IntPtr(unicode_string.Size), IntPtr.Zero))
                {
                    Debug.WriteLine("Failed to read parameter unicode.");
                    return Parameterstring;
                }

                string converted_string = new string(' ', unicode_string.Length / 2);
                if (!ReadProcessMemory(openProcessHandle, unicode_string.Buffer, converted_string, new IntPtr(unicode_string.Length), IntPtr.Zero))
                {
                    Debug.WriteLine("Failed to read parameter string.");
                    return Parameterstring;
                }

                Parameterstring = converted_string;
                CloseHandle(openProcessHandle);
            }
            catch { }
            return Parameterstring;
        }

        //DllImports enums
        public enum USER_PROCESS_PARAMETERS
        {
            CurrentDirectoryPath,
            ImagePathName,
            CommandLine
        };
        private enum PROCESSINFOCLASS : int
        {
            ProcessBasicInformation = 0
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr ExitStatus;
            public IntPtr PebBaseAddress;
            public IntPtr AffinityMask;
            public IntPtr BasePriority;
            public UIntPtr UniqueProcessId;
            public IntPtr InheritedFromUniqueProcessId;
            public int Size
            {
                get { return Marshal.SizeOf(typeof(PROCESS_BASIC_INFORMATION)); }
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct UNICODE_string
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;
            public int Size
            {
                get { return Marshal.SizeOf(typeof(UNICODE_string)); }
            }
        }

        //Application DllImports
        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr hProcess, PROCESSINFOCLASS pic, ref PROCESS_BASIC_INFORMATION pbi, int cb, IntPtr pSize);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref IntPtr lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref UNICODE_string lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [MarshalAs(UnmanagedType.LPWStr)] string lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);

        //Open and close process
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hHandle);
        private enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }
    }
}