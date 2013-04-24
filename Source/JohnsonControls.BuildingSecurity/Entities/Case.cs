/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace JohnsonControls.BuildingSecurity
{
    /// <summary>
    /// Represents a Case in P2000.
    /// </summary>
    [DataContract]
    public class Case
    {
        /// <summary>
        /// Creates an immutable Case object
        /// </summary>        
        /// <param name="id">Globally unique identifier for the case</param>
        /// <param name="title">Title of the case</param>
        /// <param name="createdBy">User who created the case</param>
        /// <param name="createdDateTime">Time and Date that the case was created</param>
        /// <param name="owner">User who owns the case</param>
        /// <param name="notes">The list of notes on the case</param>
        /// <param name="status">Status of the case</param>
        public Case(string id, string title, string createdBy, DateTimeOffset createdDateTime, string owner, IEnumerable<CaseNote> notes, CaseStatus status)
        {
            Id = id;
            Title = title;
            CreatedBy = createdBy;
            CreatedDateTime = createdDateTime;
            CreatedDateString = CreatedDateTime.ToString("d", CultureInfo.CurrentCulture);
            CreatedTimeString = CreatedDateTime.ToString("t", CultureInfo.CurrentCulture);
            CreatedDateTimeString = CreatedDateTime.ToString("g", CultureInfo.CurrentCulture);
            CreatedDateTimeSortableString = CreatedDateTime.ToString("s", CultureInfo.InvariantCulture);
            Owner = owner;
            Notes = notes.ToList();
            StatusEnum = status;
        }

        /// <summary>
        /// Globally unique identifier for the case.
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; private set; }

        /// <summary>
        /// Title of the case.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; private set; }

        /// <summary>
        /// Case status of the case.
        /// </summary>
        [DataMember(Name="status")]
        public string Status
        {
            get
            {
                switch (StatusEnum)
                {
                    case CaseStatus.Closed:
                        return @"closed";
                    default://Default to open
                        return @"open";
                }
            }
            private set
            {
                CaseStatus status;
                if (Enum.TryParse(value, ignoreCase: true, result: out status))
                {
                    StatusEnum = status;
                }
            }
        }

        [IgnoreDataMember]
        public CaseStatus StatusEnum { get; private set; }

        /// <summary>
        /// User who created the case.
        /// </summary>
        [DataMember(Name = "createdBy")]
        public string CreatedBy { get; private set; }

        /// <summary>
        /// Time and Date that the case was created.
        /// </summary>
        public DateTimeOffset CreatedDateTime { get; private set; }

        /// <summary>
        /// Gets the date portion of the <see cref="CreatedDateTime"/> as a short date string formatted for the current culture
        /// </summary>
        [DataMember(Name = "createdDateString")]
        public string CreatedDateString { get; private set; }

        /// <summary>
        /// Gets the time portion of the <see cref="CreatedDateTime"/> as a short time string formatted for the current culture
        /// </summary>
        [DataMember(Name = "createdTimeString")]
        public string CreatedTimeString { get; private set; }

        /// <summary>
        /// Returns the <see cref="CreatedDateTime"/> as a general date time (long time) string formatted for the current culture
        /// </summary>
        [DataMember(Name = "createdDateTimeString")]
        public string CreatedDateTimeString { get; private set; }

        /// <summary>
        /// Returns the <see cref="CreatedDateTime"/> as a sortable date time string
        /// </summary>
        [DataMember(Name = "createdDateTimeSortableString")]
        public string CreatedDateTimeSortableString { get; private set; }

        /// <summary>
        /// User who owns the case.
        /// </summary>
        [DataMember(Name = "owner")]
        public string Owner { get; private set; }

        /// <summary>
        /// The collection of notes that are part of the case..
        /// </summary>
        [DataMember(Name = "notes")]
        public IEnumerable<CaseNote> Notes { get; private set; }

        public Case CloneWith(string id = null, string title = null, CaseStatus? status = null, string createdBy = null, DateTimeOffset? createdDateTime = null, string owner = null, IEnumerable<CaseNote> notes = null)
        {
            return new Case(
                id:id??Id,
                title:title??Title,
                status: status??StatusEnum,
                createdBy:createdBy??CreatedBy,
                createdDateTime:createdDateTime??CreatedDateTime,
                owner:owner??Owner,
                notes:notes??Notes
                );
        }
    }
}
