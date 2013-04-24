using System;

namespace Test.Config
{
    /// <summary>
    /// Contains a collection of property values that are useful in integration tests.
    /// </summary>
    public class Configuration
    {
        private readonly Uri _pegasysUrl;
        private readonly string _validSuperUserName;
        private readonly string _validSuperUserPassword;
        private readonly string _validPartition;
        private readonly Guid _validPartitionGuid;

        public Configuration()
        {
            _pegasysUrl = new Uri("http://10.40.205.133:41023");
            _validSuperUserName = "cardkey";
            _validSuperUserPassword = "master";
            _validPartition = "Super User";
            _validPartitionGuid = new Guid("C7D28B2F-4982-4D69-8E00-E0C7C699B802"); //new Guid("E3863EE3-45A9-4788-A25B-62E6322A481A");
        }

        public string ValidSuperUserName
        {
            get { return _validSuperUserName; }
        }

        public Uri PegasysUrl
        {
            get { return _pegasysUrl; }
        }

        public string ValidSuperUserPassword
        {
            get { return _validSuperUserPassword; }
        }

        public string ValidPartition
        {
            get { return _validPartition; }
        }

        public Guid ValidPartitionGuid
        {
            get { return _validPartitionGuid; }
        }
    }
}
