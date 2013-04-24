/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Client
{
    public class BuildingSecurityClientCookie : IBuildingSecurityClientCookie
    {
        public string UserName { get; private set; }
        public string SessionId { get; private set; }
        public IEnumerable<Partition> PartitionList { get; private set; }
        public IDictionary<string, bool> Permissions { get; private set; }
        public string FullName { get; private set; }

        public BuildingSecurityClientCookie(string userName, string sessionId, string fullName, IEnumerable<Partition> partitionList, IDictionary<string, bool> permissions)
        {
            UserName = userName;
            SessionId = sessionId;
            FullName = fullName;
            PartitionList = partitionList;
            Permissions = permissions;
        }

        public string Id
        {
            get { return SessionId; }
        }

    }
}
