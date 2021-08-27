using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace LibraryUsb
{
    public partial class VirtualHidDevice
    {
        //Device variables
        private IntPtr VirtualHidInstance = IntPtr.Zero;
        public bool Connected = false;

        //DD delegates
        public delegate int pDD_btn(int btn);
        public delegate int pDD_whl(int whl);
        public delegate int pDD_key(KeysDDCode ddcode, KeysStatusFlag flag);
        public delegate int pDD_mov(int x, int y);
        public delegate int pDD_movR(int dx, int dy);
        public delegate int pDD_str(string str);
        public delegate KeysDDCode pDD_todc(byte vkcode);

        //Key status flag
        public enum KeysStatusFlag : int
        {
            Press = 1,
            Release = 2
        }

        //Mouse button 
        //1=Left Button Press, 2=Left Button Release
        //4=Right Button Press, 8=Right Button Release
        //16=Middle Button Press, 32=Middle Button Release
        //64=Side4 button Press, 128=Side4 button Release
        //256=Side5 button Press, 512=Side5 button Release
        public pDD_btn btn;

        //Mouse wheel
        //1=Scroll up, 2=Scroll down
        public pDD_whl whl;

        //Mouse move absolute position
        public pDD_mov movAbs;

        //Mouse move relative position
        public pDD_movR movRel;

        //Keyboard key (DDCode)
        public pDD_key key;

        //Keyboard type string
        public pDD_str str;

        //Virtual key to DDCode
        public pDD_todc todc;

        public VirtualHidDevice()
        {
            try
            {
                VirtualHidInstance = LoadLibrary("Resources\\Drivers\\VirtualHid\\DD64.dll");
                if (VirtualHidInstance == IntPtr.Zero)
                {
                    Connected = false;
                }
                else
                {
                    Connected = true;

                    //Get virtual addresses
                    GetVirtualAddresses(VirtualHidInstance);

                    //Initialize virtual hid
                    btn(0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create virtual hid device: " + ex.Message);
            }
        }

        public bool CloseDevice()
        {
            try
            {
                if (VirtualHidInstance != IntPtr.Zero)
                {
                    FreeLibrary(VirtualHidInstance);
                }

                Connected = false;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close virtual hid device: " + ex.Message);
                return false;
            }
        }

        private bool GetVirtualAddresses(IntPtr hinst)
        {
            try
            {
                IntPtr ptr;
                ptr = GetProcAddress(hinst, "DD_btn");
                btn = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_btn)) as pDD_btn;

                ptr = GetProcAddress(hinst, "DD_whl");
                whl = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_whl)) as pDD_whl;

                ptr = GetProcAddress(hinst, "DD_mov");
                movAbs = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_mov)) as pDD_mov;

                ptr = GetProcAddress(hinst, "DD_movR");
                movRel = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_movR)) as pDD_movR;

                ptr = GetProcAddress(hinst, "DD_key");
                key = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_key)) as pDD_key;

                ptr = GetProcAddress(hinst, "DD_str");
                str = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_str)) as pDD_str;

                ptr = GetProcAddress(hinst, "DD_todc");
                todc = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_todc)) as pDD_todc;

                Debug.WriteLine("Loaded all virtual hid device addresses.");
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to get virtual hid device addresses.");
                return false;
            }
        }
    }
}