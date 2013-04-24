/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client.Scripting.JVent
{
    public class JsonEvent
    {
        public TimeSpan At { get; private set; }
        public string Value { get; private set; }
        public string Name { get; private set; }

        public JsonEvent(TimeSpan at, string name, string value)
        {
            At = at;
            Name = name;
            Value = value;
        }


    }
}