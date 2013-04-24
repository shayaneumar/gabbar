/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Object containing one response text entry from the P2000
    /// </summary>
    /// <remarks>
    /// A response text entry represents one "canned response" that a user
    /// can choose to select in the user interface when responding to an alarm.
    /// This is in addition to just typing a response message from scratch.
    /// </remarks>
    public class ResponseText
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="ResponseText"/> class.
        /// All properties will have their C# default value.
        /// </summary>
        public ResponseText()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseText"/> class.
        /// </summary>
        public ResponseText(string partitionName, int isPublic, 
            string alarmResponseName, string alarmResponseText)
        {
            PartitionName = partitionName;
            IsPublic = isPublic;
            AlarmResponseName = alarmResponseName;
            AlarmResponseText = alarmResponseText;
        }

        /// <summary>
        /// Gets or sets the name of the partition to which the ResponseText
        /// is associated with.
        /// </summary>
        /// <value>
        /// The name of the partition.
        /// </value>
        public string PartitionName { get; set; }

        /// <summary>
        /// Gets or sets whether this response text is public.
        /// </summary>
        /// <value>
        /// 0 = non-public, 1 = public
        /// </value>
        [XmlElement("Public")]
        public int IsPublic { get; set; }

        /// <summary>
        /// Gets or sets the name of the alarm response item.
        /// </summary>
        /// <value>
        /// The name of the alarm response item.
        /// </value>
        public string AlarmResponseName { get; set; }

        /// <summary>
        /// Gets or sets the alarm response text.
        /// </summary>
        /// <value>
        /// The alarm response text.
        /// </value>
        public string AlarmResponseText { get; set; }
    }
}
