/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.BuildingSecurity
{
    public class Session
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; }

        public Session(Guid id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }
    }
}
