/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using JohnsonControls.Serialization.Xml;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    [TestClass]
    public class P2000LoginReplyTest
    {
        [TestClass]
        public class XmlDeserializer
        {
            [TestMethod]
            public void WithValidP2000ReplyShouldSerializeCorrectly()
            {
                const string message =
                    "<P2000LoginReply><SessionInfo><SessionGuid>19C264EB-336A-4D71-9899-6DB7922E4AE1</SessionGuid></SessionInfo><UserDetails><UserName>Cardkey</UserName><UserType>2</UserType><UserFullName>UserFullName</UserFullName><ProfileName>ProfileName</ProfileName><UserId>1</UserId><UserGuid>F435F474-0C53-44BA-BE45-6C30A7FCBC15</UserGuid><Partitions><Partition><Key>C7D28B2F-4982-4D69-8E00-E0C7C699B802</Key><Name>Super User</Name></Partition><Partition><Key>F744BC8F-E3C6-4194-B6DA-370C8FDD0A78</Key><Name>Part A</Name></Partition><Partition><Key>A2F2750E-C547-4D1D-A486-52C4AE940E50</Key><Name>Part B</Name></Partition></Partitions><Permissions><Permission><ResourceKey>30001</ResourceKey><PermissionLevel>2048</PermissionLevel></Permission><Permission><ResourceKey>30002</ResourceKey><PermissionLevel>2048</PermissionLevel></Permission><Permission><ResourceKey>30003</ResourceKey><PermissionLevel>2048</PermissionLevel></Permission><Permission><ResourceKey>30006</ResourceKey><PermissionLevel>2048</PermissionLevel></Permission><Permission><ResourceKey>30007</ResourceKey><PermissionLevel>2048</PermissionLevel></Permission><Permission><ResourceKey>30008</ResourceKey><PermissionLevel>2048</PermissionLevel></Permission><Permission><ResourceKey>30009</ResourceKey><PermissionLevel>2048</PermissionLevel></Permission></Permissions></UserDetails></P2000LoginReply>";
                var serializer = new XmlSerializer<P2000LoginReply>();
                var expected = new P2000LoginReply
                                   {
                                       SessionInfo = new SessionInfo
                                                         {
                                                             SessionGuid = "19C264EB-336A-4D71-9899-6DB7922E4AE1"
                                                         },
                                       UserDetails = new UserDetails
                                                         {
                                                             ProfileName = "ProfileName",
                                                             UserName = "Cardkey",
                                                             UserFullName = "UserFullName",
                                                             UserGuid = "F435F474-0C53-44BA-BE45-6C30A7FCBC15",
                                                             UserId = "1",
                                                             UserType = 2,
                                                             Partitions = new[]
                                                                              {
                                                                                  new Partition
                                                                                      {
                                                                                          Key =
                                                                                              "C7D28B2F-4982-4D69-8E00-E0C7C699B802",
                                                                                          Name = "Super User"
                                                                                      },
                                                                                  new Partition
                                                                                      {
                                                                                          Key =
                                                                                              "F744BC8F-E3C6-4194-B6DA-370C8FDD0A78",
                                                                                          Name = "Part A"
                                                                                      },
                                                                                  new Partition
                                                                                      {
                                                                                          Key =
                                                                                              "A2F2750E-C547-4D1D-A486-52C4AE940E50",
                                                                                          Name = "Part B"
                                                                                      }
                                                                              },
                                                             Permissions = new[]
                                                                               {
                                                                                   new Permission
                                                                                       {
                                                                                           PermissionLevel = "2048",
                                                                                           ResourceKey = "30001"
                                                                                       },
                                                                                   new Permission
                                                                                       {
                                                                                           PermissionLevel = "2048",
                                                                                           ResourceKey = "30002"
                                                                                       },
                                                                                   new Permission
                                                                                       {
                                                                                           PermissionLevel = "2048",
                                                                                           ResourceKey = "30003"
                                                                                       },
                                                                                   new Permission
                                                                                       {
                                                                                           PermissionLevel = "2048",
                                                                                           ResourceKey = "30006"
                                                                                       },
                                                                                   new Permission
                                                                                       {
                                                                                           PermissionLevel = "2048",
                                                                                           ResourceKey = "30007"
                                                                                       },
                                                                                   new Permission
                                                                                       {
                                                                                           PermissionLevel = "2048",
                                                                                           ResourceKey = "30008"
                                                                                       },
                                                                                   new Permission
                                                                                       {
                                                                                           PermissionLevel = "2048",
                                                                                           ResourceKey = "30009"
                                                                                       }
                                                                               }
                                                         }
                                   };


                // Act
                var actual = serializer.Deserialize(message);

                // Assert
                DtoAssert.AreEqual(expected, actual);

            }
        }

        [TestClass]
        public class CanViewAlarmManager
        {
            [TestMethod]
            public void WithNullUserDetailsExpectFalse()
            {
                // Arrange
                var p2000LoginReply = new P2000LoginReply();

                // Act
                var isSet = p2000LoginReply.CanViewAlarmManager;

                // Assert
                Assert.IsFalse(isSet);
            }

            [TestMethod]
            public void WithoutPermissionSetExpectFalse()
            {
                // Arrange
                var p2000LoginReply = new P2000LoginReply
                                          {UserDetails = new UserDetails {Permissions = new Permission[0]}};

                // Act
                var isSet = p2000LoginReply.CanViewAlarmManager;

                // Assert
                Assert.IsFalse(isSet);
            }
        }

        [TestClass]
        public class CanViewReports
        {
            [TestMethod]
            public void WithNullUserDetailsExpectFalse()
            {
                // Arrange
                var p2000LoginReply = new P2000LoginReply();

                // Act
                var isSet = p2000LoginReply.CanViewReports;

                // Assert
                Assert.IsFalse(isSet);
            }

            [TestMethod]
            public void WithoutPermissionSetExpectFalse()
            {
                // Arrange
                var p2000LoginReply = new P2000LoginReply { UserDetails = new UserDetails { Permissions = new Permission[0] } };

                // Act
                var isSet = p2000LoginReply.CanViewReports;

                // Assert
                Assert.IsFalse(isSet);
            }
        }
    }
}
