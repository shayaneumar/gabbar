/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using JohnsonControls.BuildingSecurity.XmlRpc3.Services;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Client
{
    /// <summary>
    ///This is a test class for P2KClientExtensionsTest and is intended
    ///to contain all P2KClientExtensionsTest Unit Tests
    ///</summary>
    [TestClass]
    public class AlarmMessageExtensionTest
    {
        /// <summary>
        ///A test for ConvertToAlarm
        ///</summary>
        [TestMethod]
        public void ConvertToAlarmTest()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var itemState = new DateTime(2012, 3, 31, 1, 15, 30);
            var alarmState = new DateTime(2012, 1, 31, 3, 15, 30);
            var localTime = TimeZoneInfo.Local;

            var rpcalarm = new AlarmMessage
            {
                MessageBase = new MessageBase
                {
                    PartitionName = "Partition Name",
                    IsPublic = "0",
                    Category = "TestCategory",
                    Priority = "5",
                    SiteName = "JCI",
                    Escalation = "4",
                    TimestampUtc = new DateTimeOffset(alarmState, localTime.GetUtcOffset(alarmState)).ToString(CultureInfo.InvariantCulture),
                },
                MessageDecode = new MessageDecode(),
                MessageDetails = new AlarmMessageDetails
                {
                    AlarmGuid = guid.ToString(),
                    Description = "Item Name",
                    ConditionTimestampUtc = new DateTimeOffset(itemState, localTime.GetUtcOffset(itemState)).ToString(CultureInfo.InvariantCulture),
                    AlarmType = "3", // "Area"
                    AlarmTypeName = "Area",
                    ResponseRequired = "1",
                    AcknowledgeRequired = "1",
                    AlarmState = "4", // "Pending"
                    AlarmStateName = "Pending",
                    ConditionSequenceNumber = "1",
                    ConditionState = "1", // Alarm
                    ConditionStateName = "Alarm",
                    InstructionText = "Instructions or Alarm",
                    CanComplete = "0",
                    CanRespond = "1"
                    
                }
            };

            var expected = new Alarm(
                guid,
                "Item Name",
                Guid.Empty,
                "Partition Name",
                false,
                new DateTimeOffset(itemState, localTime.GetUtcOffset(itemState)),  //since my pc is set to central, after the conversion to local, 1/31 will be -5 (DST), not offset (-7)
                "Area",
                "TestCategory",
                5,
                true,
                true,
                4,
                "Pending",
                new DateTimeOffset(alarmState, localTime.GetUtcOffset(alarmState)),     //since my pc is set to central, after the conversion to local, 1/31 will be -6, not offset (-7)
                1,
                             "JCI",
                             1,
                             "Alarm",
                             4,
                             "Instructions or Alarm",
                             "No",
                             true,
                             false,
                             true,
                             false,
                             false
                );  // has everything including enumerations converted to our domain objects

            //Act
            var actual = rpcalarm.ConvertToAlarm(TimeZoneInfo.Local, CultureInfo.CurrentCulture);

            //Assert
            DtoAssert.AreEqual(expected, actual);      //test against expected object
        }

        [TestMethod]
        public void ConvertToAlarmNullTest()
        {
            // Act + Assert
            ActionAssert.Throws<ArgumentNullException>(
                () => AlarmMessageExtensions.ConvertToAlarm(null, TimeZoneInfo.Local, CultureInfo.CurrentCulture), "alarmMessage");
        }
    }
}
