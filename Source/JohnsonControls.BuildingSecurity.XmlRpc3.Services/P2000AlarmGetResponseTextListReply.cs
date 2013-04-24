/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// This class is simply used to wrap an AlarmGetResponseTextListReply
    /// in a P2000Reply element for XML serialization purposes (per the 3.12
    /// XML RPC Documentation)
    /// </summary>
    [XmlRoot("P2000Reply")]
    public class P2000AlarmGetResponseTextListReply
    {
        /// <summary>
        /// Creates a default instance of <see cref="P2000AlarmGetResponseTextListReply"/>.
        /// Every property will have its default C# value.
        /// </summary>
        public P2000AlarmGetResponseTextListReply()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="P2000AlarmGetResponseTextListReply"/>
        /// </summary>
        /// <param name="reply"></param>
        public P2000AlarmGetResponseTextListReply(AlarmGetResponseTextListReply reply)
        {
            if (reply == null) throw new ArgumentNullException("reply");
            AlarmGetResponseTextListReply = reply;
        }

        /// <summary>
        /// Gets or sets the actual reply object.
        /// </summary>
        public AlarmGetResponseTextListReply AlarmGetResponseTextListReply { get; set; }

        /// <summary>
        /// Converts this instance into a string representation useful for
        /// debugging purposes.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "AlarmGetResponseTextListReply: {0}", AlarmGetResponseTextListReply);
        }
    }
}
