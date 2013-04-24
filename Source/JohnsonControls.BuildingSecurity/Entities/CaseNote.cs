/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace JohnsonControls.BuildingSecurity
{
    [DataContract]
    public class CaseNote
    {
        public CaseNote(Guid id, DateTimeOffset timestampUtc, string createdBy, string text)
        {
            Id = id;
            TimestampUtc = timestampUtc;
            CreatedBy = createdBy;
            Text = text;
        }

        [DataMember]
        public Guid Id { get; private set; }

        [DataMember]
        public DateTimeOffset TimestampUtc { get; private set; }

        [DataMember]
        public string CreatedBy { get; private set; }

        [DataMember]
        public string Text { get; private set; }

// ReSharper disable ValueParameterNotUsed
        //private sets are needed to make the .net serialization framework happy.
        [DataMember]
        public string TimestampDateString {
            get { return TimestampUtc.ToString("d", CultureInfo.CurrentCulture); }
            private set { }
        }

        [DataMember]
        public string TimestampTimeString {
            get { return TimestampUtc.ToString("t", CultureInfo.CurrentCulture); }
            private set { }
        }

        [DataMember]
        public string TimestampDateTimeString
        {
            get { return TimestampUtc.ToString("g", CultureInfo.CurrentCulture); }
            private set { }
        }

        [DataMember]
        public long TimestampUtcMilliseconds
        {
            get { return (long)(TimestampUtc - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds; }
            private set { }
        }
// ReSharper restore ValueParameterNotUsed
    }
}
