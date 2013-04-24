/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Text;
using JetBrains.Annotations;

namespace JohnsonControls
{
    public static class ByteExtensions
    {
        [Pure]
        [NotNull]
        public static string ToHexString([NotNull]this byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
