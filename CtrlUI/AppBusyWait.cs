using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CtrlUI
{
    public class AppBusyWait
    {
        //Wait for busy bool
        ///<param name="boolGetter">() => busyBool</param>
        public static async Task<bool> WaitForBusyBoolCancel(Func<bool> boolGetter, bool waitBool)
        {
            try
            {
                if (boolGetter())
                {
                    if (waitBool)
                    {
                        Debug.WriteLine("Waiting for busy bool...");
                        while (boolGetter())
                        {
                            await Task.Delay(1);
                        }
                    }
                    else
                    {
                        //Debug.WriteLine("Busy bool, cancel.");
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }
    }
}