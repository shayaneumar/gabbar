/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// P2000Request container class for AlarmDetailsRequest
    /// </summary>
    /// <remarks>Every property on this class is mutable to facilitate serialization.</remarks>
    [XmlRoot("P2000Request")]
    public class P2000AlarmDetailsRequestWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="P2000AlarmDetailsRequestWrapper"/> class.
        /// </summary>
        public P2000AlarmDetailsRequestWrapper()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="P2000AlarmDetailsRequestWrapper"/> class,
        /// with an AlarmDetailsRequest
        /// </summary>
        public P2000AlarmDetailsRequestWrapper(AlarmDetailsRequest alarmDetailsRequest)
        {
            AlarmDetailsRequest = alarmDetailsRequest;
        }

        /// <summary>
        /// Container object that holds all of the request filters and results configuration for
        /// <see cref="ITypedAlarmService.AlarmDetails(string,string,string,int,SortOrder)"/>
        /// </summary>
        public AlarmDetailsRequest AlarmDetailsRequest { get; set; }
    }
}