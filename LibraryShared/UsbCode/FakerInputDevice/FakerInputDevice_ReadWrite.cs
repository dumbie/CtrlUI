using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace LibraryUsb
{
    public partial class FakerInputDevice
    {
        private byte[] ConvertToByteArray(object targetObject)
        {
            IntPtr marshalPtr = IntPtr.Zero;
            try
            {
                int arraySize = Marshal.SizeOf(targetObject);
                byte[] byteArray = new byte[arraySize];
                marshalPtr = Marshal.AllocHGlobal(arraySize);
                Marshal.StructureToPtr(targetObject, marshalPtr, false);
                Marshal.Copy(marshalPtr, byteArray, 0, arraySize);
                return byteArray;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to convert object to byte array: " + ex.Message);
                return null;
            }
            finally
            {
                SafeCloseMarshal(marshalPtr);
            }
        }

        private byte[] MergeHeaderInputByteArray(int arraySize, byte[] firstArray, byte[] secondArray)
        {
            try
            {
                byte[] byteArray = new byte[arraySize];
                Buffer.BlockCopy(firstArray, 0, byteArray, 0, firstArray.Length);
                Buffer.BlockCopy(secondArray, 0, byteArray, firstArray.Length, secondArray.Length);
                return byteArray;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to merge input byte array: " + ex.Message);
                return null;
            }
        }

        private bool WriteBytesFile(byte[] outputBuffer)
        {
            try
            {
                if (!Connected) { return false; }
                FileStream.Write(outputBuffer, 0, outputBuffer.Length);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to write file bytes: " + ex.Message);
                return false;
            }
        }
    }
}