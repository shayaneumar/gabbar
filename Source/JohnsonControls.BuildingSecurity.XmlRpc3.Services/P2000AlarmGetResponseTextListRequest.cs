/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// The outer-most container object Containing the request filter parameters
    /// for the P2000's AlarmGetResponseTextList method.
    /// </summary>
    [XmlRoot("P2000Request")]
    public class P2000AlarmGetResponseTextListRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="P2000AlarmGetResponseTextListRequest"/> class.
        /// </summary>
        public P2000AlarmGetResponseTextListRequest()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="P2000AlarmGetResponseTextListRequest"/> class.
        /// </summary>
        /// <param name="request">The request parameter object.</param>
        public P2000AlarmGetResponseTextListRequest(AlarmGetResponseTextListRequest request)
        {
            AlarmGetResponseTextListRequest = request;
        }

        /// <summary>
        /// Gets or sets the request's filter parameters.
        /// </summary>
        /// <value>
        /// The get response text parameters.
        /// </value>
        public AlarmGetResponseTextListRequest AlarmGetResponseTextListRequest { get; set; }
    }
}
