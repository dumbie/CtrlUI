using System.Text;

namespace LibraryUsb
{
    public static class Extensions
    {
        public static string ToUTF8String(this byte[] buffer)
        {
            string value = Encoding.UTF8.GetString(buffer);
            return value.Remove(value.IndexOf((char)0));
        }

        public static string ToUTF16String(this byte[] buffer)
        {
            string value = Encoding.Unicode.GetString(buffer);
            return value.Remove(value.IndexOf((char)0));
        }
    }
}