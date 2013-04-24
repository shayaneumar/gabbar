/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Web.Administration;

namespace IisActions
{
    public class CustomAction
    {
        [CustomAction]
        public static ActionResult DeployCertificate(Session session)
        {
            session.Log("Beginning DeploySelfSignedCertificate.");
            var hostname = GetFullHostname();
            session.Log("Determined hostname of computer to be: {0}.  This name will be used for the certificate, and should be used when accessing the website.", hostname);
            session.Log("Enumerating CustomActionData.");
            //print out all the session data we have access to (helps when troubleshooting)
            foreach (var v in session.CustomActionData)
            {
                session.Log("{0}:{1}", v.Key, v.Value);
            }

            var installPath = session.CustomActionData["INSTALLLOCATION"];

            string certExportPath;
            session.CustomActionData.TryGetValue("CERTEXPORTPATH", out certExportPath);

            session.Log("Searching for existing certificates.");
            X509Certificate2 certificate;
            if (!TryRetrieveCertificate(out certificate, hostname))
            {
                session.Log("No usable certificates found, beginning generation of new self-signed certificate.");
                certificate = GenerateSelfSignCertificate(hostname);
                session.Log("Successful generation of certificate.");
            }

            session.Log("Beginning configuration of https binding.");
            AddHttpsBinding(session, installPath, certificate, certExportPath);
            session.Log("Configuration of https binding successful.");

            session.Log("DeploySelfSignedCertificate finished successfully.");
            return ActionResult.Success;
        }

        private static void AddHttpsBinding(Session session, string installPath, X509Certificate2 cert, string certExportPath)
        {
            using (var mgr = new ServerManager())
            {
                session.Log("Searching for site in IIS.");
                var site = mgr.Sites.FirstOrDefault(s => s.Applications.Any(app => app.VirtualDirectories.Any(x => AreSameDirectory(x.PhysicalPath, installPath))));
                if(site == null)
                {
                    session.Log("Site not found.  This could be caused by the presence of IIS Express.");
                    throw new Exception("Could not find site.  This could be caused by the presence of IIS Express.");
                }

                session.Log("Site found ({0}), checking for https bindings.", site.Name);

                var httpsBinding = site.Bindings.FirstOrDefault(x => x.Protocol == "https" && x.BindingInformation.StartsWith("*:443"));
                if (httpsBinding != null)
                {
                    session.Log("Binding already present ({0}), it will not be changed.", httpsBinding.BindingInformation);
                    session.Log("Certificate sha1 fingerprint={0};", ToHex(httpsBinding.CertificateHash));
                    ExportPublicKey(session, certExportPath, httpsBinding.CertificateHash, httpsBinding.CertificateStoreName);
                    return;
                }

                session.Log("Usable https binding was not found, adding new one.");
                var storeName = new X509Store(StoreName.My, StoreLocation.LocalMachine).Name;
                site.Bindings.Add("*:443:", cert.GetCertHash(), storeName);
                ExportPublicKey(session, certExportPath, cert.GetCertHash(), storeName);
                session.Log("Certificate sha1 fingerprint={0};", ToHex(cert.GetCertHash()));
                mgr.CommitChanges();
                session.Log("Binding added.");
            }
        }

        private static void ExportPublicKey(Session session, string certExportPath, byte[] certFingerprint, string storeName)
        {
            if (string.IsNullOrWhiteSpace(certExportPath))
            {
                session.Log("certExportPath was not set, ssl certificate's public key will not be exported.");
                return;
            }

            var certStore = new X509Store(storeName, StoreLocation.LocalMachine);
            certStore.Open(OpenFlags.ReadOnly);
            var selectedCert = certStore.Certificates.Find(X509FindType.FindByThumbprint, ToHex(certFingerprint), false).Cast<X509Certificate2>().SingleOrDefault();
            certStore.Close();
            if (selectedCert == null) throw new InvalidOperationException("Failed to find certificate in key store");
            var formatedCert = selectedCert.Export(X509ContentType.Cert);
            if (formatedCert == null) throw new InvalidOperationException("Failed to export certificate's public key");
            string path = Path.GetDirectoryName(certExportPath);
            if (path == null)
            {
                session.Log("The directory specified to export the certificate was invalid.");
                throw new Exception("The directory specified to export the certificate was invalid.");
            }

            Directory.CreateDirectory(path);
            File.WriteAllBytes(certExportPath, formatedCert);
            session.Log("Certificate exported to:{0}", certExportPath);
        }

        private static string ToHex(Byte[] data)
        {
            var sb = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Checks to see if 2 paths resolve to the same directory
        /// </summary>
        private static bool AreSameDirectory(string path1, string path2)
        {
            return string.Equals( Path.GetFullPath(path1).TrimEnd('\\'), Path.GetFullPath(path2).TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase);
        }

        private static X509Certificate2 GenerateSelfSignCertificate(string hostname)
        {
            var pinfo = new ProcessStartInfo
                            {
                                FileName = "makecert.exe",
                                Arguments = "-r -pe -n CN=\"" + hostname + "\" -b 01/10/2012 -e 01/01/2029 -a sha256 -eku 1.3.6.1.5.5.7.3.1 -ss my -sr localmachine -sky exchange -sp \"Microsoft RSA SChannel Cryptographic Provider\" -sy 12 -len 2048",
                                WindowStyle = ProcessWindowStyle.Hidden
                            };
            var certGen = Process.Start(pinfo);
            certGen.WaitForExit();
            if(certGen.ExitCode != 0) throw new Exception("Certification generation failed");
            X509Certificate2 result;

            if (TryRetrieveCertificate(out result, hostname))//Since certificate was created out of process we can not get it directly
            {
                return result;
            }
            throw new Exception("Certificate generation seems to have been successful, but was unable to retrieve generated certificate from certificate store.");
        }

        private static bool TryRetrieveCertificate(out X509Certificate2 certificate, string hostname)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var possibleCerts = store.Certificates.Find(X509FindType.FindByApplicationPolicy, "1.3.6.1.5.5.7.3.1", false).Cast<X509Certificate2>()
                .Where(x => x.Subject.StartsWith("CN=" + hostname, StringComparison.CurrentCultureIgnoreCase) && x.NotAfter > DateTime.Now && x.NotBefore < DateTime.Now).ToList();
            store.Close();

            //Bias result towards valid certificates (this is not the optimal way to do this, but was the cleanest syntax I could come up with)
            var orderedCerts = possibleCerts.Where(x => x.Verify()).Concat(possibleCerts.Where(x => !x.Verify()));

            certificate = orderedCerts.FirstOrDefault();
            return certificate!=null;
        }

        private static string GetFullHostname()
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            return string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName).Trim().TrimEnd('.');
        }

    }
}
