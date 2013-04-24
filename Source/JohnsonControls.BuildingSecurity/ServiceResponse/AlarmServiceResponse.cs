/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.BuildingSecurity
{
    /// <summary>
    /// Class for Response Objects from the P2000 related to Alarm methods
    /// </summary>
    public class AlarmServiceResponse : ServiceResponse
    {
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">Response code</param>
        /// <param name="text">Response text</param>
        /// <param name="id">GUID of the Alarm</param>
        public AlarmServiceResponse(int code, string text, Guid id)
            : base(code, text)
        {
            Id = id;
        }
        #endregion

        #region Properties
        /// <summary>GUID of the Alarm</summary>
        public Guid Id { get; private set; }
        #endregion
    }
}
