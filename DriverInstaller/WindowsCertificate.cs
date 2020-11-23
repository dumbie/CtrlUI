using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace DriverInstaller
{
    public partial class WindowMain
    {
        void InstallCertificate(string filePath)
        {
            try
            {
                X509Certificate2 certificateFile = new X509Certificate2(filePath);
                X509Store certificateStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);

                certificateStore.Open(OpenFlags.ReadWrite);
                certificateStore.Add(certificateFile);
                certificateStore.Close();

                Debug.WriteLine("Registered certificate to store root.");
            }
            catch { }
        }

        void UninstallCertificate(string issuedTo)
        {
            try
            {
                X509Store certificateStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);

                certificateStore.Open(OpenFlags.ReadWrite);
                foreach (X509Certificate2 cert in certificateStore.Certificates.Cast<X509Certificate2>().Where(x => x.Issuer == issuedTo))
                {
                    certificateStore.Remove(cert);
                    Debug.WriteLine("Removed certificate from the store root.");
                }
                certificateStore.Close();
            }
            catch { }
        }
    }
}