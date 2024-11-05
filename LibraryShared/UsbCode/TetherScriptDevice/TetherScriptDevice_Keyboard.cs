using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInteropDll;

namespace LibraryUsb
{
    public partial class TetherScriptDevice
    {
        public void KeyPressSingle(KeysHid keyPress)
        {
            try
            {
                KeysPress(0, (byte)keyPress, 0, 0, 0, 0, 0);
                AVHighResDelay.Delay(50);
                KeysRelease();
            }
            catch { }
        }

        public void KeyPressCombo(KeysModifierHid keyMod, KeysHid keyPress)
        {
            try
            {
                KeysPress((byte)keyMod, (byte)keyPress, 0, 0, 0, 0, 0);
                AVHighResDelay.Delay(50);
                KeysRelease();
            }
            catch { }
        }

        private bool KeysPress(byte Modifier, byte Key0, byte Key1, byte Key2, byte Key3, byte Key4, byte Key5)
        {
            IntPtr featureIntPtr = IntPtr.Zero;
            try
            {
                //Set feature data
                SetFeatureKeyboard featureData = new SetFeatureKeyboard();
                featureData.ReportID = 1;
                featureData.CommandCode = 2;
                featureData.Timeout = 1000;
                featureData.Modifier = Modifier;
                featureData.Key0 = Key0;
                featureData.Key1 = Key1;
                featureData.Key2 = Key2;
                featureData.Key3 = Key3;
                featureData.Key4 = Key4;
                featureData.Key5 = Key5;

                //Convert to byte array
                int featureSize = Marshal.SizeOf(featureData);
                byte[] featureArray = new byte[featureSize];
                featureIntPtr = Marshal.AllocHGlobal(featureSize);
                Marshal.StructureToPtr(featureData, featureIntPtr, false);
                Marshal.Copy(featureIntPtr, featureArray, 0, featureSize);

                //Send byte array to driver
                return SetFeature(FileHandle, featureArray);
            }
            catch
            {
                Debug.WriteLine("Failed to press tether keys.");
                return false;
            }
            finally
            {
                SafeCloseMarshal(ref featureIntPtr);
            }
        }

        private bool KeysRelease()
        {
            IntPtr featureIntPtr = IntPtr.Zero;
            try
            {
                //Set feature data
                SetFeatureKeyboard featureData = new SetFeatureKeyboard();
                featureData.ReportID = 1;
                featureData.CommandCode = 2;
                featureData.Timeout = 1000;

                //Convert to byte array
                int featureSize = Marshal.SizeOf(featureData);
                byte[] featureArray = new byte[featureSize];
                featureIntPtr = Marshal.AllocHGlobal(featureSize);
                Marshal.StructureToPtr(featureData, featureIntPtr, false);
                Marshal.Copy(featureIntPtr, featureArray, 0, featureSize);

                //Send byte array to driver
                return SetFeature(FileHandle, featureArray);
            }
            catch
            {
                Debug.WriteLine("Failed to release tether keys.");
                return false;
            }
            finally
            {
                SafeCloseMarshal(ref featureIntPtr);
            }
        }
    }
}