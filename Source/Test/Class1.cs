using System;
using System.Net;
using JohnsonControls.BuildingSecurity.XmlRpc3.Services;
using JohnsonControls.XmlRpc;

namespace Test
{
    public class Configuration { private readonly Uri _pegasysUrl; private readonly string _validSuperUserName; private readonly string _validSuperUserPassword; private readonly string _validPartition; private readonly Guid _validPartitionGuid; public Configuration() { _pegasysUrl = new Uri("http://10.40.205.133:41023"); _validSuperUserName = "cardkey"; _validSuperUserPassword = "master"; _validPartition = "Super User"; _validPartitionGuid = new Guid("E3863EE3-45A9-4788-A25B-62E6322A481A"); } public string ValidSuperUserName { get { return _validSuperUserName; } } public Uri PegasysUrl { get { return _pegasysUrl; } } public string ValidSuperUserPassword { get { return _validSuperUserPassword; } } public string ValidPartition { get { return _validPartition; } } public Guid ValidPartitionGuid { get { return _validPartitionGuid; } } }

    public class Class1
    {
        static readonly Config.Configuration Config = new Config.Configuration();

        static int Main()
        {
            ITypedSessionManagement target = null;
            P2000LoginReply p2000LoginReply = null;

            try
            {
                target = new SessionManagementService(Config.PegasysUrl);
                p2000LoginReply = target.P2000Login(Config.ValidSuperUserName, Config.ValidSuperUserPassword);

                TestRtl(p2000LoginReply.SessionInfo);
//                TestAlarmGetListEx(p2000LoginReply.SessionInfo);
            }
            catch(Exception e)
            {
                Console.WriteLine("EXCEPTION: " + e.Message);
            }

            if (target != null && p2000LoginReply != null)
            {
                target.P2000Logout(Config.ValidSuperUserName, p2000LoginReply.SessionInfo.SessionGuid);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();

            return 0;
        }

        private static void TestRtl(SessionInfo sessionInfo)
        {
            XmlMessageClient xmlMessageClient = new XmlMessageClient(new IPAddress(0x85CD280A), 4502, Config.ValidSuperUserName, sessionInfo.SessionGuid);
            new MessageProcessingService(xmlMessageClient);

            Console.WriteLine("Press ESC to stop...");
            do
            {
                while (!Console.KeyAvailable)
                {
                    // Do something 
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape); 

            xmlMessageClient.Stop();
        }

/*
        private static void TestAlarmGetListEx(SessionInfo sessionInfo)
        {
            ITypedAlarmService alarmService = new AlarmService(Config.PegasysUrl);

//            var alarms = alarmService.AlarmGetList(Config.ValidSuperUserName, Config.ValidSuperUserPassword, Config.ValidPartitionGuid.ToString());

            const string partition = "Super User";
            const string alarmGuid = "235337E0-B192-4259-82AE-C0D10C64C831"; // "*"; //"alarms[0].ItemGuid; //"235337E0-B192-4259-82AE-C0D10C64C831";
            const string alarmSiteName = "Site 1";
            const string alarmTypeName = "Alarm Type 1";
            const string itemName = "Item 1";
            const string operatorName = "Operator 1";

//                AlarmDetailsReply p2000AlarmDetailsReply = alarmService.AlarmDetails(config.ValidSuperUserName, p2000LoginReply.SessionInfo.SessionGuid, alarmGuid);

            AlarmGetListExReply p2000AlarmGetListExReply = alarmService.AlarmGetListEx(
                Config.ValidSuperUserName, sessionInfo.SessionGuid,
                partition, alarmGuid, alarmSiteName, alarmTypeName, itemName, operatorName, 50, new SortOrder(new[] { new SortKey("1", "Column1", null, null) }, "DESC"));

            Console.WriteLine(p2000AlarmGetListExReply.ToString());    
        }
*/
    }
}
