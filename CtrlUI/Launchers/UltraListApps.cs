using System;
using System.Diagnostics;

namespace CtrlUI
{
    partial class WindowMain
    {
        void UltraScanAddLibrary()
        {
            try
            {
                //Fix find way to run games through launcher using ultra://
                //Fix get applications from Roaming\ultra\gameLibraryStorage.json
                //Decode gameLibraryStorage file with key jWnZr4u7x!A%D*G-KaPdRgUkXp2s5v8y
                //Decode persistent-storage file with key 5KPNxJ3k5i7JGJDB5wjkffaXNNwBPQqtfChWAj
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Ultra library: " + ex.Message);
            }
        }
    }
}