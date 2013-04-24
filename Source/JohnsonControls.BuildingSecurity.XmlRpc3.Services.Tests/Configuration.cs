/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
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
        public string SessionGuid { get; set; }
        private readonly string _realTimeServiceAddress;

        public Configuration()
        {
            _pegasysUrl = new Uri("http://10.40.205.133:41023");
            _validSuperUserName = "cardkey";
            _validSuperUserPassword = "master";
            _validPartition = "Super User";
            _validPartitionGuid = new Guid("E3863EE3-45A9-4788-A25B-62E6322A481A");
            _realTimeServiceAddress = "10.40.205.133";
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

        public string RealTimeServiceAddress
        {
            get { return _realTimeServiceAddress; }
        }
    }
}
