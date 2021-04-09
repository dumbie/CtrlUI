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
                //Temporary byte array
                IEnumerable<byte> tempByteArray = new List<byte>();

                //Convert each string into wide multi-byte and add NULL-terminator in between
                tempByteArray = stringArray.Aggregate(tempByteArray, (current, entry) => current.Concat(Encoding.Unicode.GetBytes(entry)).Concat(Encoding.Unicode.GetBytes(new[] { char.MinValue })));

                //Add another NULL-terminator to signal end of the list
                tempByteArray = tempByteArray.Concat(Encoding.Unicode.GetBytes(new[] { char.MinValue }));

                //Convert expression to array
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
                length = -1;
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