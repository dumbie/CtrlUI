using System.Diagnostics;
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
                //Fix some clients seem to open 4 separate udp endpoints / check timeout / check ports
                //Debug.WriteLine("Gyro dsu client connected: " + endPoint.Address + ":" + endPoint.Port);
                if (vController0.GyroDsuClientEndPoint == null)
                {
                    vController0.GyroDsuClientEndPoint = endPoint;
                    vController1.GyroDsuClientEndPoint = endPoint;
                    vController2.GyroDsuClientEndPoint = endPoint;
                    vController3.GyroDsuClientEndPoint = endPoint;
                }
                else if (endPoint.Port < vController0.GyroDsuClientEndPoint.Port)
                {
                    Debug.WriteLine("Overwriting gyro dsu client endpoint: " + vController0.GyroDsuClientEndPoint.Port + " with " + endPoint.Port);
                    vController0.GyroDsuClientEndPoint = endPoint;
                    vController1.GyroDsuClientEndPoint = endPoint;
                    vController2.GyroDsuClientEndPoint = endPoint;
                    vController3.GyroDsuClientEndPoint = endPoint;
                }
                return true;
            }
            catch { }
            return false;
        }
    }
}