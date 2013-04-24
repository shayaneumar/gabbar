/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;

namespace BuildingSecurity.Web.Api.Models
{
    [DataContract]
    public class AudioAlert
    {
        public AudioAlert(Guid id, decimal duration, string name, string source)
        {
            Id = id;
            Duration = duration;
            Name = name;
            Source = source;
        }

        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "duration")]
        public decimal Duration { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }
    }
}
