/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using JohnsonControls.Serialization;
using JohnsonControls.Serialization.Xml;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using JohnsonControls.BuildingSecurity.XmlRpc3.Services;
using System.Collections.Generic;
using Moq;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Client
{
    /// <summary>
    ///This is a test class for BuildingSecurityClientTest and is intended
    ///to contain all BuildingSecurityClientTest Unit Tests
    ///</summary>
    [TestClass]
    public class BuildingSecurityClientTest
    {
        private static IBuildingSecurityClientCookie CreateCookie(string userName =null, string fullName = null, string sessionId = null,
            IEnumerable<Partition> partitions= null, bool canViewAlarms = false, bool canViewReports = false)
        {
            var permission = new Dictionary<string, bool>
                                 {
                                     {PermissionNames.CanViewAlarmManager, canViewAlarms},
                                     {PermissionNames.CanViewReports, canViewReports}
                                 }.ToDictionary(x => x.Key.ToUpperInvariant(), x => x.Value);
            return new BuildingSecurityClientCookie(userName: userName ?? "userName", fullName: fullName ?? "fullName", sessionId: sessionId??Guid.NewGuid().ToString(),
                               partitionList: partitions ?? Enumerable.Empty<Partition>(), permissions: permission);
        }

        private static BuildingSecurityClient GetBuildingSecurityClient(ITypedAlarmService alarmService = null,
                                                                        ITypedSessionManagement sessionService = null,
                                                                        ITypedSystemInformationService systemInformationService = null,
                                                                        ITypedApplicationPreference applicationPreferenceService = null,
                                                                        IDataSerializerFactory serializationFactory = null)
        {
            return new BuildingSecurityClient(alarmService ?? new Mock<ITypedAlarmService>().Object,
                                              sessionService ?? new Mock<ITypedSessionManagement>().Object,
                                              systemInformationService ?? new Mock<ITypedSystemInformationService>().Object,
                                              applicationPreferenceService ?? new Mock<ITypedApplicationPreference>().Object,
                                              serializationFactory ?? new DataSerializerFactory());
        }

        private static IEnumerable<AlarmMessage> CreateAlarmMessages()
        {
            var alarms = new List<AlarmMessage>();
            var date = new DateTime(2012, 3, 31, 1, 15, 30);

            for (int i = 0; i < 3; i++)
            {
                var rpcalarm = new AlarmMessage
                {
                    MessageBase = new MessageBase
                    {
                        BaseVersion = "301",
                        MessageType = "3",
                        MessageSubtype = "2",
                        SiteName = "MKESite",
                        PartitionName = "Partition Name" + i,
                        IsPublic = "0",
                        ItemName = "Item Name" + i,
                        QueryString = "",
                        Category = "P2000",
                        Escalation = "0",
                        Priority = "0",
                        OperatorName = "cardkey",
                        HistoryFilterKey = "2",
                        HistoryFilterName = "Alarm",
                        TimestampUtc = date.ToString(CultureInfo.InvariantCulture)
                    },
                    MessageDecode = new MessageDecode
                    {
                        MessageDateTime = "6/13/2012 3:31:06 PM",
                        MessageTypeText = "Alarm",
                        MessageText = "Acked, Set",
                        DetailsText = "Details" + i
                    },
                    MessageDetails = new AlarmMessageDetails
                    {
                        MessageVersion = "301",
                        AlarmGuid = Guid.NewGuid().ToString(),
                        AlarmId = "10",
                        AlarmType = "2",
                        AlarmOptionsGuid = Guid.NewGuid().ToString(),
                        AlarmTypeName = "Input Point",
                        AlarmTypeId = "21",
                        AcknowledgeRequired = "0",
                        ResponseRequired = "0",
                        InstructionText = "",
                        AlarmState = (i == 0 ? 3 : 4).ToString(CultureInfo.InvariantCulture), //4 is pending,
                        AlarmStateName = "Acked",
                        AlarmTimestamp = date.ToString(CultureInfo.InvariantCulture),
                        ConditionState = "1",
                        ConditionStateName = "Set",
                        ConditionSequenceNumber = i.ToString(CultureInfo.InvariantCulture),
                        ConditionCompletionState = "0",
                        ConditionCompletionStateName = "Secure",
                        ConditionTimestamp = date.ToString(CultureInfo.InvariantCulture),
                        ConditionTimestampUtc = date.ToString(CultureInfo.InvariantCulture),
                        Popup = "1",
                        Description = "Description" + i,
                        AlarmSiteName = "MKESite",
                        AlarmResponseText = "",
                        CanAcknowledge = "0",
                        CanRespond = "1",
                        CanComplete = "0",
                    }
                };

                alarms.Add(rpcalarm);
            }
            return alarms;
        }

        [TestClass]
        public class TheRetrieveActiveAlarmsMethod
        {
            private static AlarmGetListExReply GetAlarmGetListExReply(AlarmMessage[] messages =null, SortOrder sortOrder =null)
            {
                return new AlarmGetListExReply
                           {
                               AlarmMessages = messages??new AlarmMessage[0],
                               SortOrder = sortOrder?? new SortOrder(new[] {new Services.SortKey()})
                           };
            }

            [TestMethod]
            public void NullCookie_ArgumentException()
            {
                //Arrange
                var alarmServiceStub = new Mock<ITypedAlarmService>();
                var target = GetBuildingSecurityClient(alarmService:alarmServiceStub.Object);

                //Act+Assert
                ActionAssert.Throws<ArgumentException>(()=>target.RetrieveActiveAlarms(null, TimeZoneInfo.Local, CultureInfo.CurrentCulture), "cookie");
            }

            [TestMethod]
            public void ShouldNeverReturnNull()
            {
                //Arrange
                var alarmServiceStub = new Mock<ITypedAlarmService>();
                alarmServiceStub.Setup(x => x.AlarmGetListEx(It.IsAny<string>(), It.IsAny<string>(),
                                                             It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                                             It.IsAny<string>(),
                                                             It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                                                             It.IsAny<SortOrder>())).Returns(GetAlarmGetListExReply());

                var target = GetBuildingSecurityClient(alarmService: alarmServiceStub.Object);

                //Act
                var actual = target.RetrieveActiveAlarms(CreateCookie(), TimeZoneInfo.Local, CultureInfo.CurrentCulture).Data.ToList();

                //Assert
                CollectionAssert.AreEqual(new Alarm[0], actual);
            }

            [TestMethod]
            public void ShouldRetrieveAllActiveAlarmsNew()
            {
                //Arrange
                var alarmServiceStub = new Mock<ITypedAlarmService>();
                var alarmResponseFakes = CreateAlarmMessages().ToArray();
                alarmServiceStub.Setup(x => x.AlarmGetListEx(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SortOrder>()))
                    .Returns(GetAlarmGetListExReply(messages:alarmResponseFakes));

                var expected = alarmResponseFakes.Select(alarms => alarms.ConvertToAlarm(TimeZoneInfo.Local, CultureInfo.CurrentCulture)).Where(a => !a.IsCompleted).ToList();
                var target = GetBuildingSecurityClient(alarmService: alarmServiceStub.Object);

                //Act
                var actual = target.RetrieveActiveAlarms(CreateCookie(), TimeZoneInfo.Local, CultureInfo.CurrentCulture).Data.ToList();

                //Assert
                DtoAssert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void ShouldRetrieveOnlyActiveAlarmsNew()
            {
                //Arrange
                var alarmServiceStub = new Mock<ITypedAlarmService>();
                var alarmResponseFakes = CreateAlarmMessages().ToArray();

                alarmServiceStub.Setup(x => x.AlarmGetListEx(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SortOrder>()))
                    .Returns(GetAlarmGetListExReply(messages:alarmResponseFakes));

                var target = GetBuildingSecurityClient(alarmService:alarmServiceStub.Object);
                var expected = alarmResponseFakes.Select(alarms => alarms.ConvertToAlarm(TimeZoneInfo.Local, CultureInfo.CurrentCulture)).Where(a => !a.IsCompleted).ToList();

                //Act
                var actual = target.RetrieveActiveAlarms(CreateCookie(), TimeZoneInfo.Local, CultureInfo.CurrentCulture).Data.ToList();

                //TODO:Refractor to be more obvious what expected and actual are. Too much is being calculated by test case.
                //Assert
                DtoAssert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void ShouldNeverReturnNullNew()
            {
                //Arrange
                var alarmServiceStub = new Mock<ITypedAlarmService>();
                alarmServiceStub.Setup(x => x.AlarmGetListEx(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SortOrder>())).Returns(GetAlarmGetListExReply(messages:new AlarmMessage[0]));

                var target = GetBuildingSecurityClient(alarmService: alarmServiceStub.Object);

                //Act
                var actual = target.RetrieveActiveAlarms(CreateCookie(), TimeZoneInfo.Local, CultureInfo.CurrentCulture);

                //Assert
                Assert.IsFalse(actual.Data.Any());
            }
        }

        [TestClass]
        public class TheAcknowledgeAlarmMethod
        {
            [TestMethod]
            public void ShouldCallAlarmActionWithAlarmStateAcknowledged()
            {
                //Arrange
                var alarmServiceMock = new Mock<ITypedAlarmService>(MockBehavior.Strict);
                var mockReturn = new AlarmActionReply
                                     {
                                         AlarmActionFilter = new AlarmActionFilter {AlarmGuid = new MultipleCVAlarmGuidFilter {CurrentValues = new[] {Guid.Empty.ToString()}}},
                                         AlarmActionResponses = new[]{new AlarmActionResponse {AlarmActionStatus = "4105", AlarmGuid = Guid.Empty.ToString()}},
                                         Command = "3",
                                         Parameters =
                                             new Parameters {AlarmResponseText = string.Empty, ConditionSequenceNumber = "34"}
                                     };

                var sessionId = Guid.NewGuid().ToString();
                var cookie = CreateCookie(userName:"userName", sessionId:sessionId);
                alarmServiceMock.Setup(x => x.AlarmAction("userName", sessionId, It.IsAny<IEnumerable<Services.AlarmIdSequenceTuple>>(), 3, It.IsAny<string>())).Returns(mockReturn);

                var target = GetBuildingSecurityClient(alarmService: alarmServiceMock.Object);

                //Act
                target.AcknowledgeAlarm(cookie, new[] { new AlarmIdSequenceTuple(Guid.Empty, 0) });

                //Assert
                alarmServiceMock.VerifyAll();
            }

            [TestMethod]
            public void NullAlarmIds_ArgumentNullException()
            {
                //Arrange
                var target = GetBuildingSecurityClient();

                //Act + Assert

                ActionAssert.Throws<ArgumentNullException>(() => target.AcknowledgeAlarm(CreateCookie(), null));
            }

            [TestMethod]
            public void NullCookie_ArgumentNullException()
            {
                //Arrange
                var target = GetBuildingSecurityClient();

                //Act + Assert
                ActionAssert.Throws<ArgumentException>(() => target.AcknowledgeAlarm(null, Enumerable.Empty<AlarmIdSequenceTuple>()), "cookie");
            }
        }

        [TestClass]
        public class TheCompleteAlarmMethod
        {
            [TestMethod]
            public void ShouldCallAlarmActionWithAlarmStateCompleted()
            {
                //Arrange
                var alarmServiceMock = new Mock<ITypedAlarmService>(MockBehavior.Strict);
                var mockReturn = new AlarmActionReply
                {
                    AlarmActionFilter = new AlarmActionFilter { AlarmGuid = new MultipleCVAlarmGuidFilter { CurrentValues = new[] { string.Empty } } },
                    AlarmActionResponses = new[] { new AlarmActionResponse { AlarmActionStatus = "4105", AlarmGuid = Guid.Empty.ToString() } },
                    Command = "1",
                    Parameters =
                        new Parameters { AlarmResponseText = string.Empty, ConditionSequenceNumber = "34" }
                };
                var sessionId = Guid.NewGuid().ToString();
                var cookie = CreateCookie(userName: "userName", sessionId:sessionId);
                alarmServiceMock.Setup(x => x.AlarmAction("userName", sessionId, It.IsAny<IEnumerable<Services.AlarmIdSequenceTuple>>(), 1, It.IsAny<string>())).Returns(mockReturn);

                var target = GetBuildingSecurityClient(alarmService: alarmServiceMock.Object);

                //Act
                target.CompleteAlarm(cookie, new[] { new AlarmIdSequenceTuple(Guid.Empty, 0) });

                //Assert
                alarmServiceMock.VerifyAll();
            }

            [TestMethod]
            public void NullAlarmIds_ArgumentNullException()
            {
                //Arrange
                var target = GetBuildingSecurityClient();

                //Act + Assert
                ActionAssert.Throws<ArgumentNullException>(() => target.CompleteAlarm(CreateCookie(), null), "alarmIds");
            }

            [TestMethod]
            public void NullCookie_ArgumentException()
            {
                //Arrange
                var target = GetBuildingSecurityClient();

                //Act + Assert
                ActionAssert.Throws<ArgumentException>(() => target.CompleteAlarm(null, Enumerable.Empty<AlarmIdSequenceTuple>()), "cookie");
            }
        }

        [TestClass]
        public class TheRespondToAlarmMethod
        {
            [TestMethod]
            public void ShouldCallAlarmActionWithAlarmStateResponding()
            {
                //Arrange
                var alarmServiceMock = new Mock<ITypedAlarmService>(MockBehavior.Strict);
                var mockReturn = new AlarmActionReply
                {
                    AlarmActionFilter = new AlarmActionFilter { AlarmGuid = new MultipleCVAlarmGuidFilter { CurrentValues = new[] { string.Empty } } },
                    AlarmActionResponses = new[] { new AlarmActionResponse { AlarmActionStatus = "4105", AlarmGuid = Guid.Empty.ToString() } },
                    Command = "2",
                    Parameters =
                        new Parameters { AlarmResponseText = string.Empty, ConditionSequenceNumber = "34" }
                };

                var sessionId = Guid.NewGuid().ToString();
                var cookie = CreateCookie(userName: "userName", sessionId:sessionId);
                alarmServiceMock.Setup(x => x.AlarmAction("userName", sessionId, It.IsAny<IEnumerable<Services.AlarmIdSequenceTuple>>(), 2, "response")).Returns(mockReturn);

                var target = GetBuildingSecurityClient(alarmService:alarmServiceMock.Object);

                //Act
                target.RespondToAlarm(cookie, new[] { new AlarmIdSequenceTuple(Guid.Empty, 0) }, "response");

                //Assert
                alarmServiceMock.VerifyAll();
            }

            [TestMethod]
            public void NullAlarmIds_ArgumentNullException()
            {
                //Arrange
                var target = GetBuildingSecurityClient();

                //Act + Assert
                ActionAssert.Throws<ArgumentNullException>(() => target.RespondToAlarm(CreateCookie(), null, "response"), "alarmIds");
            }

            [TestMethod]
            public void NullCookie_ArgumentException()
            {
                //Arrange
                var target = GetBuildingSecurityClient();

                //Act + Assert
                ActionAssert.Throws<ArgumentException>(() => target.RespondToAlarm(null, Enumerable.Empty<AlarmIdSequenceTuple>(), "response"), "cookie");
            }

            [TestMethod]
            public void NullResponse_ArgumentNullException()
            {
                //Arrange
                var target = GetBuildingSecurityClient();

                //Act + Assert
                ActionAssert.Throws<ArgumentException>(() => target.RespondToAlarm(CreateCookie(), null, "response"));
            }
        }

        [TestClass]
        public class TheRetrieveResponseTextMethod
        {
            [TestMethod]
            public void ShouldReturnAllResponseTexts()
            {
                //Arrange
                var alarmServiceStub = new Mock<ITypedAlarmService>();
                alarmServiceStub.Setup(r => r.AlarmGetResponseTextList(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SortOrder>(), It.IsAny<Paging>())).Returns(
                    new AlarmGetResponseTextListReply(new AlarmResponseTextFilter(), new Paging(), new SortOrder( new []{new Services.SortKey("0","AlarmResponseText",null, null) }),
                                                      new ResponseText("Super User", 1, "Call Police",
                                                                       "Calling 911 and making a log of my call."),
                                                      new ResponseText("Super User", 1, "Propped Door",
                                                                       "Told source of alarm to stop propping the door open.")));

                var target = GetBuildingSecurityClient(alarmService: alarmServiceStub.Object);

                //Act
                var response = target.RetrieveResponseTexts(CreateCookie());

                // Assert
                Assert.AreEqual(2, response.Count());
            }

            [TestMethod]
            public void NullCookie_ArgumentException()
            {
                //Arrange
                var target = GetBuildingSecurityClient();

                //Act + Assert
                ActionAssert.Throws<ArgumentException>(() => target.RetrieveResponseTexts(null), "cookie");
            }
        }

        [TestClass]
        public class TheSignInMethod
        {
            [TestMethod]
            public void ThrowsExceptionIfUserNameIsNull()
            {
                // Arrange
                var target = GetBuildingSecurityClient();
                string errorMessage;
                IBuildingSecurityClientCookie cookie;
                // Act+Assert
                ActionAssert.Throws<ArgumentNullException>(() => target.TrySignIn(userName:null, password:"password", cookie:out cookie, errorMessage: out errorMessage), "userName");
            }

            [TestMethod]
            public void ThrowsExceptionIfUserNameIsEmpty()
            {
                // Arrange
                var target = GetBuildingSecurityClient();
                string errorMessage;
                IBuildingSecurityClientCookie cookie;

                // Act+Assert
                ActionAssert.Throws<ArgumentNullException>(() => target.TrySignIn(userName: String.Empty, password: "password", cookie: out cookie, errorMessage: out errorMessage), "userName");
            }

            [TestMethod]
            public void ThrowsExceptionIfPasswordIsNull()
            {
                // Arrange
                var target = GetBuildingSecurityClient();
                string errorMessage;
                IBuildingSecurityClientCookie cookie;
                // Act+Assert
                ActionAssert.Throws<ArgumentNullException>(() => target.TrySignIn(userName: "username", password: null, cookie: out cookie, errorMessage: out errorMessage), "password");
            }

            [TestMethod]
            public void ThrowsExceptionIfPasswordIsEmpty()
            {
                // Arrange
                var target = GetBuildingSecurityClient();
                string errorMessage;
                IBuildingSecurityClientCookie cookie;
                // Act+Assert
                ActionAssert.Throws<ArgumentNullException>(() => target.TrySignIn(userName: "username", password: String.Empty, cookie: out cookie, errorMessage: out errorMessage), "password");
            }
        }

        [TestClass]
        public class TheApplicationPreferenceSaveMethod
        {
            [TestMethod]
            public void ThrowsExceptionIfCookieIsNull()
            {
                //Arrange
                var target = GetBuildingSecurityClient();

                //Act + Assert
                ActionAssert.Throws<ArgumentException>(() => target.SaveApplicationPreference(null, "Key", "Value"), "cookie");
            }

            [TestMethod]
            public void ThrowsExceptionIfKeyIsEmpty()
            {
                // Arrange
                var target = GetBuildingSecurityClient();

                // Act+Assert
                ActionAssert.Throws<ArgumentException>(() => target.SaveApplicationPreference(CreateCookie(), string.Empty, "Value"), "settingName");
            }

            [TestMethod]
            public void ThrowsExceptionIfValueIsNull()
            {
                // Arrange
                var target = GetBuildingSecurityClient();

                // Act+Assert
                ActionAssert.Throws<ArgumentNullException>(() => target.SaveApplicationPreference<string>(CreateCookie(), "Key", null), "value");
            }

            [TestMethod]
            public void ShouldCallApplicationPreferenceSaveWithPreferenceTypeOfApplication()
            {
                //Arrange
                var applicationServiceMock = new Mock<ITypedApplicationPreference>(MockBehavior.Strict);

                applicationServiceMock.Setup(x => x.ApplicationPreferenceSave(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), PreferenceType.Application, It.IsAny<string>()));

                var target = GetBuildingSecurityClient(applicationPreferenceService: applicationServiceMock.Object);

                //Act
                target.SaveApplicationPreference(CreateCookie(), "key", "value");

                //Assert
                applicationServiceMock.VerifyAll();
            }
        }

        [TestClass]
        public class TheApplicationPreferenceReadMethod
        {
            [XmlRoot("string")]
            public class StringSetting
            {
                [XmlAttribute("value")]
                public string Value { get; set; }
            }

            [TestMethod]
            public void ThrowsExceptionIfCookieIsNull()
            {
                //Arrange
                var target = GetBuildingSecurityClient();
                List<string> value;
                //Act + Assert
                ActionAssert.Throws<ArgumentException>(() => target.TryReadApplicationPreference(null, "Key", out value),"cookie");
            }

            [TestMethod]
            public void ThrowsExceptionIfKeyIsEmpty()
            {
                // Arrange
                var target = GetBuildingSecurityClient();
                List<string> value;

                // Act+Assert
                ActionAssert.Throws<ArgumentException>(() => target.TryReadApplicationPreference(CreateCookie(), string.Empty, out value));
            }

            [TestMethod]
            public void ShouldCallApplicationPreferenceReadWithPreferenceTypeOfApplication()
            {
                //Arrange
                var applicationServiceMock = new Mock<ITypedApplicationPreference>();

                applicationServiceMock.Setup(x => x.ApplicationPreferenceRead(It.IsAny<string>(), It.IsAny<string>(), "fooValue", PreferenceType.Application)).Returns("<string value='hi'/>");

                var target = GetBuildingSecurityClient(applicationPreferenceService: applicationServiceMock.Object);

                StringSetting setting;

                //Act
                var result = target.TryReadApplicationPreference(CreateCookie(), "fooValue", out setting);

                //Assert
                Assert.IsTrue(result);
                Assert.AreEqual("hi", setting.Value);
            }
        }

        [TestClass]
        public class TheApplicationPreferenceDeleteMethod
        {
            [TestMethod]
            public void ThrowsExceptionIfCookieIsNull()
            {
                //Arrange
                var target = GetBuildingSecurityClient();

                //Act + Assert
                ActionAssert.Throws<ArgumentException>(() => target.DeleteApplicationPreference(null, "Key"), "cookie");
            }

            [TestMethod]
            public void ThrowsExceptionIfKeyIsEmpty()
            {
                // Arrange
                var target = GetBuildingSecurityClient();

                // Act+Assert
                ActionAssert.Throws<ArgumentException>(() => target.DeleteApplicationPreference(CreateCookie(), string.Empty), "settingName");
            }
            
            [TestMethod]
            public void ShouldCallApplicationPreferenceDeleteWithPreferenceTypeOfApplication()
            {
                //Arrange
                var applicationServiceMock = new Mock<ITypedApplicationPreference>(MockBehavior.Strict);

                applicationServiceMock.Setup(x => x.ApplicationPreferenceDelete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), PreferenceType.Application));

                var target = GetBuildingSecurityClient(applicationPreferenceService: applicationServiceMock.Object);

                //Act
                target.DeleteApplicationPreference(CreateCookie(), "key");

                //Assert
                applicationServiceMock.VerifyAll();
            }
        }
    }
}
