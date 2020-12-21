using System.Net;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Check incoming gyro dsu client
        bool GyroDsuClientHandler(IPEndPoint endPoint, byte[] incomingBytes)
        {
            try
            {
                //Check gyro dsuc header
                if (incomingBytes[0] != 'D' && incomingBytes[1] != 'S' && incomingBytes[2] != 'U' && incomingBytes[3] != 'C')
                {
                    return false;
                }

                //Update gyro dsu client endpoint
                //Debug.WriteLine("Gyro dsu client connected: " + endPoint.Address + ":" + endPoint.Port);
                vGyroDsuClientEndPoint = endPoint;
                return true;
            }
            catch { }
            return false;
        }
    }
}