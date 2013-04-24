/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Globalization;
using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Reply object returned from <see cref="ITypedAlarmService.AlarmGetResponseTextList(string,string,Services.SortOrder,Services.Paging)"/>.
    /// </summary>
    /// <remarks>
    /// Every property on this class is mutable to facilitate serialization. Because of this it is not 
    /// recommended you use it for anything other than serialization/deserialization. Since this class
    /// represents a reply object, the most useful usage scenario is deserializing an xml payload and then
    /// reading the properties.
    /// </remarks>
    public class AlarmGetResponseTextListReply
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="AlarmGetResponseTextListReply"/> class.
        /// All properties except ResponseTexts will have their C# default value.
        /// ResponseTexts will default to an empty list to match the XML deserializer behavior
        /// when deserializing and empty AlarmGetResponseTextListReply element.
        /// </summary>
        public AlarmGetResponseTextListReply()
        {
            ResponseTexts = new ResponseText[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmGetResponseTextListReply"/> class
        /// with the specified values for the properties.
        /// </summary>
        public AlarmGetResponseTextListReply(AlarmResponseTextFilter filter, Paging paging, 
            SortOrder sortOrder, params ResponseText[] responseTexts)
        {
            Filter = filter;
            Paging = paging;
            SortOrder = sortOrder;
            ResponseTexts = responseTexts;
        }

        /// <summary>
        /// Gets or sets a filter criteria for this instance.
        /// </summary>
        [XmlElement("AlarmResponseTextFilter")]
        public AlarmResponseTextFilter Filter { get; set; }

        /// <summary>
        /// Gets or sets paging information for this instance.
        /// </summary>
        public Paging Paging { get; set; }

        /// <summary>
        /// Gets or sets the SortOrder used for this instance.
        /// </summary>
        public SortOrder SortOrder { get; set; }

        /// <summary>
        /// Gets or sets a collection of ResponseText objects. This list
        /// represents the canned set of responses available to use
        /// to respond to an alarm.
        /// </summary>
        public ResponseText[] ResponseTexts { get; set; }
        
        /// <summary>
        /// Converts this instance to a string representation useful for debugging.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                                 "Filter: {0}, Paging: {1}, SortOrder: {2}, ResponseTexts: {3}", Filter, Paging,
                                 SortOrder, ResponseTexts.ConvertToString());
        }
    }
}
