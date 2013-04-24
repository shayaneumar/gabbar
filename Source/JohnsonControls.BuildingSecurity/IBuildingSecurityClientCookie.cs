/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
namespace JohnsonControls.BuildingSecurity
{
    /// <summary>
    /// Serves as an opaque marker type which allows instances of <see cref="IBuildingSecurityClient"/> to thread state
    /// between calls.
    /// </summary>
    public interface IBuildingSecurityClientCookie
    {
        /// <summary>
        /// A string which uniquely identifies this cookie.
        /// </summary>
        /// <remarks>This identifier is meaningless and should not be used for anything other
        /// than identifying a cookie.</remarks>
        string Id { get; }
    }
}
