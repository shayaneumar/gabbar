/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Container object that holds all of the request filters for
    /// AlarmGetResponseTextList
    /// </summary>
    [XmlRoot("AlarmGetResponseTextListRequest")]
    public class AlarmGetResponseTextListRequest 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmGetResponseTextListRequest"/> class.
        /// </summary>
        public AlarmGetResponseTextListRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmGetResponseTextListRequest"/> class.
        /// </summary>
        /// <param name="alarmResponseTextFilter">The alarm response filter object.</param>
        /// <param name="paging">The paging object</param>
        /// <param name="sortOrder">The sort order object.</param>
        public AlarmGetResponseTextListRequest(
            AlarmResponseTextFilter alarmResponseTextFilter,
            Paging paging,
            SortOrder sortOrder)
        {
            Filter = alarmResponseTextFilter;
            Paging = paging;
            SortOrder = sortOrder;
        }

        /// <summary>
        /// Gets or sets the <see cref="AlarmResponseTextFilter"/> filter.
        /// </summary>
        /// <value>
        /// The Request/Response filter
        /// </value>
        [XmlElement("AlarmResponseTextFilter")]
        public AlarmResponseTextFilter Filter { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Paging"/> object.
        /// </summary>
        /// <value>
        /// The paging object.
        /// </value>
        [XmlElement("Paging")]
        public Paging Paging { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SortOrder"/> object.
        /// </summary>
        /// <value>
        /// The sort order object.
        /// </value>
        [XmlElement("SortOrder")]
        public SortOrder SortOrder { get; set; }
    }
}
