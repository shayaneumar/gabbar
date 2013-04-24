/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Client
{
    public enum AlarmState
    {
        Unknown         = 0,
        Completed       = 1,
        Responding      = 2,
        Acknowledged    = 3,
        Pending         = 4
    }
}