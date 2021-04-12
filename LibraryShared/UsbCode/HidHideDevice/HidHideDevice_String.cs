using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibraryUsb
{
    public partial class HidHideDevice
    {
        public IntPtr StringArrayToMultiSzPointer(IEnumerable<string> stringArray, out int length)
        {
            try
            {
                //Check string array
                if (!stringArray.Any())
                {
                    //Return empty buffer pointer
                    length = 0;
                    return IntPtr.Zero;
                }

                //Temporary byte list
                IEnumerable<byte> tempByteArray = new List<byte>();
                byte[] minValue = Encoding.Unicode.GetBytes(new[] { char.MinValue });

                //Convert each string into wide multi-byte and add NULL-terminator in between
                foreach (string stringEntry in stringArray)
                {
                    tempByteArray = tempByteArray.Concat(Encoding.Unicode.GetBytes(stringEntry)).Concat(minValue);
                }

                //Add another NULL-terminator to signal end of the list
                tempByteArray = tempByteArray.Concat(minValue);

                //Convert list to array
                byte[] multiSzArray = tempByteArray.ToArray();

                //Convert array to managed native buffer
                IntPtr bufferIntPtr = Marshal.AllocHGlobal(multiSzArray.Length);
                Marshal.Copy(multiSzArray, 0, bufferIntPtr, multiSzArray.Length);

                //Return usable buffer pointer
                length = multiSzArray.Length;
                return bufferIntPtr;
            }
            catch
            {
                //Return empty buffer pointer
                length = 0;
                return IntPtr.Zero;
            }
        }

        public IEnumerable<string> MultiSzPointerToStringArray(IntPtr buffer, int length)
        {
            try
            {
                //Temporary byte array
                byte[] tempByteArray = new byte[length];

                //Grab data from buffer
                Marshal.Copy(buffer, tempByteArray, 0, length);

                //Trims away potential redundant NULL-characters and splits at NULL-terminator
                return Encoding.Unicode.GetString(tempByteArray).TrimEnd(char.MinValue).Split(char.MinValue);
            }
            catch { }
            return null;
        }
    }
}