/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BuildingSecurity.Web.Api.Models
{
    [DataContract]
    public class AlarmDisplayOptions
    {
        private IEnumerable<AlarmDisplayRange> _displayRanges;

        public AlarmDisplayOptions(IEnumerable<AlarmDisplayRange> displayRanges)
        {
            DisplayRanges = displayRanges;
        }

        [DataMember(Name = "displayRanges")]
        public IEnumerable<AlarmDisplayRange> DisplayRanges
        {
            get { return _displayRanges ?? Enumerable.Empty<AlarmDisplayRange>(); }
            private set { _displayRanges = value; }
        }
    }
}
