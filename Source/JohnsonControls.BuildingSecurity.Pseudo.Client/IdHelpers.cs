/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client
{
    public class IdHelpers
    {
        private static Guid HashToGuid(string s)
        {
            using (var hasher = new SHA1CryptoServiceProvider())
            {
                var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(s));
                return new Guid(hash.Take(16).ToArray());
            }
        }

        public static Guid GetGuid(string stringId)
        {
            Guid guid = Guid.TryParse(stringId, out guid) ? guid : Guid.Empty;
            if (guid != Guid.Empty) return guid;

            if (stringId != null) return HashToGuid(stringId);
            return Guid.NewGuid();
        }

        public static string GetId(string stringId = null)
        {
            return string.IsNullOrWhiteSpace(stringId) ? Guid.NewGuid().ToString() : stringId;
        }
    }
}
