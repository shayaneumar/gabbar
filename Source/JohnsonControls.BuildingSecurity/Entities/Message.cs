/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace JohnsonControls.BuildingSecurity
{
    /// <summary>
    /// Base class for all events
    /// </summary>
    [DataContract]
    public class Message
    {
        /// <summary>
        /// Creates an immutable message object
        /// </summary>
        /// <param name="id">Message id</param>
        /// <param name="description">Message description</param>
        /// <param name="partitionId">The GUID of the partition the message occurred on</param>
        /// <param name="partitionName">The name of the partition the message occurred on</param>
        /// <param name="isPublic">True if the message is public, otherwise false</param>
        /// <param name="messageDateTime">The Date and Time the message occurred.</param>
        public Message(Guid id, string description, Guid partitionId, string partitionName, bool isPublic, DateTimeOffset messageDateTime)
        {
            Id = id;
            Description = description;
            PartitionId = partitionId;
            PartitionName = partitionName;
            IsPublic = isPublic;
            MessageDateTime = messageDateTime;
            MessageDateString = MessageDateTime.ToString("d", CultureInfo.CurrentCulture);
            MessageTimeString = MessageDateTime.ToString("t", CultureInfo.CurrentCulture);
            MessageDateTimeString = MessageDateTime.ToString("G", CultureInfo.CurrentCulture);
            MessageDateTimeSortableString = MessageDateTime.ToString("s", CultureInfo.InvariantCulture);
            MessageUtcDateTime = (long) (MessageDateTime.UtcDateTime - new DateTime(1970,1,1,0,0,0,0,DateTimeKind.Utc)).TotalMilliseconds;
        }

        /// <summary>
        /// Gets the unique identifier of this message.
        /// </summary>
        [DataMember]
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the description of this message.
        /// </summary>
        [DataMember]
        public string Description { get; private set; }

        /// <summary>
        /// Gets the GUID of partition which owns this alarm
        /// </summary>
        public Guid PartitionId { get; private set; }
       
        /// <summary>
        /// Gets the name of the partition which owns this alarm.
        /// </summary>
        public string PartitionName { get; private set; }
        
        /// <summary>
        /// True if the alarm is public, otherwise false.
        /// </summary>
        public bool IsPublic { get; private set; }

        /// <summary>
        /// Gets the <see cref="DateTimeOffset"/> the message occurred
        /// </summary>
        /// <remarks>
        /// This value is the time the event occurred, represented in the local time of the current machine. i.e.: ("2012-04-30T13:34:46-05:00").
        /// </remarks>
        public DateTimeOffset MessageDateTime { get; private set; }

        /// <summary>
        /// Gets the date portion of the <see cref="MessageDateTime"/> as a short date string formatted for the current culture
        /// </summary>
        [DataMember]
        public string MessageDateString { get; private set; }

        /// <summary>
        /// Gets the time portion of the <see cref="MessageDateTime"/> as a short time string formatted for the current culture
        /// </summary>
        [DataMember]
        public string MessageTimeString { get; private set; }

        /// <summary>
        /// Gets the <see cref="MessageDateTime"/> as a general date time (long time) string formatted for the current culture
        /// </summary>
        [DataMember]
        public string MessageDateTimeString { get; private set; }

        /// <summary>
        /// Gets the <see cref="MessageDateTime"/> in utc as a unix date. (milliseconds since 1/1/1970)
        /// </summary>
        [DataMember]
        public long MessageUtcDateTime { get; private set; }

        /// <summary>
        /// Gets the <see cref="MessageDateTime"/> as a sortable date time string
        /// </summary>
        [DataMember]
        public string MessageDateTimeSortableString { get; private set; }
    }
}
