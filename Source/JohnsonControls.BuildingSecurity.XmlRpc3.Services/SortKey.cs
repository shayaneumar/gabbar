/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Used by the P2000 to specify the sort order of a response.
    /// </summary>
    /// <remarks>Every property on this class is mutable to facilitate serialization.</remarks>
    public class SortKey
    {
        /// <summary>
        /// Creates a default instance of <see cref="SortKey"/>
        /// </summary>
        public SortKey()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="SortKey"/> class
        /// </summary>
        /// <param name="sequenceNumber">Priority of the SortKey within an array containing multiple SortKeys.</param>
        /// <param name="name">The name of the data field to sort by.</param>
        /// <param name="startKey">Will return the first set of records with a value greater that this.</param>
        /// <param name="lastKey">Value of the respective data field from the last record included in a response sorted by this SortKey.</param>
        public SortKey(string sequenceNumber, string name, string startKey, string lastKey)
        {
            SequenceNumber = sequenceNumber;
            Name = name;
            StartKey = startKey;
            LastKey = lastKey;
        }

        /// <summary>Priority of the SortKey within an array containing multiple SortKeys</summary>
        [XmlAttribute]
        public string SequenceNumber { get; set; }

        /// <summary>Name of the Property to be sorted</summary>
        public string Name { get; set; }

        /// <summary>Will return the first set of records with a value greater that this</summary>
        public string StartKey { get; set; }

        /// <summary>Value of the respective data field from the last record included in a response sorted by this SortKey</summary>
        public string LastKey { get; set; }
    }
}