/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace JohnsonControls.BuildingSecurity
{
    [DataContract]
    public class ReportServerConfiguration
    {
        private static readonly byte[] AppSalt = new byte[]
                                       {
                                           0xeb, 0x60, 0xb5, 0x75, 0x86, 0xd8, 0x4c, 0xc6, 0xaa, 0x8b, 0x37, 0x1b, 0x8e,
                                           0x95
                                           , 0xb5, 0x6d, 0xce, 0x1d, 0x5c, 0x2b, 0xfc, 0x51, 0x41, 0x41, 0x81, 0x3d,
                                           0x54,
                                           0x65, 0xd1, 0x4e, 0xe9, 0xb7
                                       };
        
        private string _serviceUrl;
        public const string ObfuscatedPassword = "••••••••••••••••"; //Yes a user 'COULD' type this....but they won't

        [DataMember]
        public string ServiceUrl
        {
            get { return _serviceUrl.SafeTrim(); }
            private set { _serviceUrl = value; }
        }

        private string _domain;

        [DataMember]
        public string Domain
        {
            get { return _domain.SafeTrim(); }
            private set { _domain = value; }
        }

        private string _userName;

        [DataMember]
        public string UserName
        {
            get { return _userName.SafeTrim(); }
            private set { _userName = value; }
        }

        private string _encryptedPassword;

        [DataMember]
        public string EncryptedPassword
        {
            get { return _encryptedPassword??""; }
            private set { _encryptedPassword = value; }
        }

        private SecureString _password;

        /// <summary>
        /// Returns the password as a secure string. If the password cannot be decrypted or
        /// has not yet been set, then this returns an empty secure string.
        /// </summary>
        ///<remarks>Disposal is guaranteed for SecureStrings, even if the thread terminates abnormally,
        /// as it is a CriticalFinalizerObject. Thus it does not need to be disposed manually.</remarks>>
        [IgnoreDataMember]
        public SecureString Password
        {
            //DataContract deserializer does not call a constructor so we have to derive this after construction
            get { return _password ?? (_password = DecryptPassword().ToSecureString()); }
        }

        public ReportServerConfiguration(string serviceUrl, string domain, string userName, string encryptedPassword)
        {
            ServiceUrl = serviceUrl;
            Domain = domain;
            UserName = userName;
            EncryptedPassword = encryptedPassword;
        }

        public ReportServerConfiguration(bool useSsl, string server, string webServiceVirtualDirectory, string domain, string userName, string password, string currentEncryptedPassword)
        {
            if (webServiceVirtualDirectory == null) throw new ArgumentNullException("webServiceVirtualDirectory");

            string scheme = useSsl ? "https://" : "http://";
            string serviceUrl = scheme + server + "/" + webServiceVirtualDirectory;

            ServiceUrl = serviceUrl;
            Domain = domain;
            UserName = userName;

            // do not change the password unless the user has actually specified a password (or specified no password).
            EncryptedPassword = (password != ObfuscatedPassword)
                                    ? EncryptPassword(password)
                                    : currentEncryptedPassword;
        }

        public ReportServerConfiguration CloneWithNewPassword(string rawPassword)
        {
            return new ReportServerConfiguration(ServiceUrl, Domain, UserName, EncryptPassword(rawPassword));
        }

        private static string EncryptPassword(string rawPassword)
        {
            var encryptedPasswordBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(rawPassword ?? ""), AppSalt
                                  , DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedPasswordBytes);
        }

        /// <summary>
        /// Decrypts the <see cref="EncryptedPassword"/> and returns it.
        /// If the decryption fails, this returns an empty string.
        /// </summary>
        /// <returns></returns>
        private string DecryptPassword()
        {
            if (string.IsNullOrWhiteSpace(EncryptedPassword)) return "";
            try
            {
                return
                    Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(EncryptedPassword), AppSalt,
                                                                    DataProtectionScope.CurrentUser));
            }
            catch(CryptographicException)
            {
                return "";
            }
        }
    }
}
