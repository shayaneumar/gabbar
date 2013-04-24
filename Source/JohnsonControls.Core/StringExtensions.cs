/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

namespace JohnsonControls
{
    public static class StringExtensions
    {
        [Pure]
        public static SecureString ToSecureString(this string str)
        {
            var secureString = new SecureString();
            foreach (var c in str)
            {
                secureString.AppendChar(c);
            }
            secureString.MakeReadOnly();
            return secureString;
        }

        /// <summary>
        /// Removes leading and trailing whitespace from <see cref="str"/>;
        /// If <see cref="str"/> is null an empty string is returned.
        /// </summary>
        [Pure]
        public static string SafeTrim([CanBeNull]this string str)
        {
            return (str ?? "").Trim();
        }

        [Pure]
        public static string Sha1([NotNull]this string str, [NotNull]Encoding encoding)
        {
            if (str == null) throw new ArgumentNullException("str");
            if (encoding == null) throw new ArgumentNullException("encoding");
            using (var hasher = new SHA1CryptoServiceProvider())
            {
                var hash = hasher.ComputeHash(encoding.GetBytes(str));
                return hash.ToHexString();
            }
        }
    }
}
