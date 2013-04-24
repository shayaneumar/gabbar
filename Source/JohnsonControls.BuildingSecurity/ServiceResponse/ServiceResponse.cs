/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity
{
    /// <summary>
    /// Base class for Response Objects from the P2000
    /// </summary>
    public class ServiceResponse
    {
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">Response code</param>
        /// <param name="text">Response text</param>
        public ServiceResponse(int code, string text)
        {
            Code = code;
            Text = text;
        }
        #endregion

        #region Properties
        /// <summary>Response code</summary>
        private int Code { get; set; }
        
        /// <summary>
        /// True if the response code returned by the RPC method call indicates Success (equals 0), otherwise false
        /// </summary>
        public bool Success { get { return Code == 0; } }

        /// <summary>Response text</summary>
        public string Text { get; private set; }
        #endregion
    }
}
