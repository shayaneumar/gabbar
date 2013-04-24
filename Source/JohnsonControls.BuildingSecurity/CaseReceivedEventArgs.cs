/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/


namespace JohnsonControls.BuildingSecurity
{
    public class CaseReceivedEventArgs : ChannelUpdateEventArgs
    {
        public CaseReceivedEventArgs(Case updatedCase)
            : base("caseChannelPushed", updatedCase)
        {}
    }
}
