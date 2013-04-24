/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Diagnostics;
using SignalR;
using SignalR.Hubs;

namespace BuildingSecurity.Web.Api.Models
{
    /// <summary>
    /// PushServices makes a collection of channels available to Building Security clients. The client
    /// can register for one or more of the available channels. For as long as the client is registered,
    /// it will be pushed all items that arrive for that channel.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class PushServices : Hub, IDisconnect
    {
        private readonly string _currentUserName;
        private readonly string _clientId;
        private readonly IBuildingSecuritySessionStore _sessionStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="PushServices"/> with default values and services.
        /// </summary>
        public PushServices()
            : this(null, null, DependencyResolver.Current.GetService<IBuildingSecuritySessionStore>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PushServices"/> with the specified values and services.
        /// </summary>
        /// <param name="currentUserName">The name of the user this registration should be associated with.</param>
        /// <param name="clientId">The id of the calling signalr client.
        /// uses to add / remove channel callbacks.</param>
        /// <param name="sessionStore"> </param>
        public PushServices(string currentUserName, string clientId, IBuildingSecuritySessionStore sessionStore)
        {
            _currentUserName = currentUserName;
            _clientId = clientId;
            _sessionStore = sessionStore;
        }

        public void SubscribeToChannel(string channelName)
        {
            IUser currentUser;
            if (_sessionStore.TryRetrieveUser(CurrentUserName, out currentUser))
            {
                currentUser.AddStreamCallbackForClient(channelName, ClientId, (clientId, update) => PushUpdateToClient(channelName, clientId, update));
            }
        }

        public void UnsubscribeFromChannel(string channelName)
        {
            IUser currentUser;
            if (_sessionStore.TryRetrieveUser(CurrentUserName, out currentUser))
            {
                currentUser.RemoveStreamCallbackForClient(channelName, ClientId);
            }
        }
        private string CurrentUserName
        {
            get { return _currentUserName ?? Context.User.Identity.Name; }
        }

        private string ClientId
        {
            get { return _clientId ?? Context.ConnectionId; }
        }

        private static void PushUpdateToClient(string channel, string clientId, object update)
        {
            HubContext.Clients[clientId].pushUpdate(channel, update);
        }


        private static IHubContext HubContext
        {
            get
            {
                return GlobalHost.ConnectionManager.GetHubContext<PushServices>();
            }
        }

        public Task Disconnect()
        {
            Log.Information("Client disconnected without warning.");
            return Clients.leave(ClientId);
        }
    }
}
