using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInteropDll;

namespace LibraryUsb
{
    public partial class TetherScriptDevice
    {
        public bool MouseMoveRel(sbyte moveX, sbyte moveY)
        {
            IntPtr featureIntPtr = IntPtr.Zero;
            try
            {
                //Set feature data
                SetFeatureMouseRel featureData = new SetFeatureMouseRel();
                featureData.ReportID = 1;
                featureData.CommandCode = 2;
                featureData.X = moveX;
                featureData.Y = moveY;

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
                Debug.WriteLine("Failed to move tether mouse.");
                return false;
            }
            finally
            {
                SafeCloseMarshal(ref featureIntPtr);
            }
        }

        public bool MousePressRel(MouseHidButtons mouseButtons)
        {
            IntPtr featureIntPtr = IntPtr.Zero;
            try
            {
                //Set feature data
                SetFeatureMouseRel featureData = new SetFeatureMouseRel();
                featureData.ReportID = 1;
                featureData.CommandCode = 2;
                featureData.Buttons = (byte)mouseButtons;

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
                Debug.WriteLine("Failed to press tether mouse.");
                return false;
            }
            finally
            {
                SafeCloseMarshal(ref featureIntPtr);
            }
        }

        public bool MouseScrollRel(int scrollUp, int scrollDown)
        {
            IntPtr featureIntPtr = IntPtr.Zero;
            try
            {
                //Set feature data
                SetFeatureMouseRel featureData = new SetFeatureMouseRel();
                featureData.ReportID = 1;
                featureData.CommandCode = 2;
                featureData.VWheelPosition = (byte)scrollUp;
                featureData.HWheelPosition = (byte)scrollDown;

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
                Debug.WriteLine("Failed to scroll tether mouse.");
                return false;
            }
            finally
            {
                SafeCloseMarshal(ref featureIntPtr);
            }
        }

        public bool MouseReleaseRel()
        {
            IntPtr featureIntPtr = IntPtr.Zero;
            try
            {
                //Set feature data
                SetFeatureMouseRel featureData = new SetFeatureMouseRel();
                featureData.ReportID = 1;
                featureData.CommandCode = 2;

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
                Debug.WriteLine("Failed to release tether mouse.");
                return false;
            }
            finally
            {
                SafeCloseMarshal(ref featureIntPtr);
            }
        }
    }
}