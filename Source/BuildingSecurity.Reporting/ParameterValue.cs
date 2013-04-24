/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Runtime.Serialization;

namespace BuildingSecurity.Reporting
{
    [DataContract]
    public class ParameterValue
    {
        public ParameterValue(string name, string value)
        {
            Name = name;
            Value = value;
        }

        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "value")]
        public string Value { get; private set; }
    }
}
