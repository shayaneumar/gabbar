/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using BuildingSecurity.Web.Api.Models;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.Api.Controllers.Debugging
{
    [RequiredPermission(PermissionNames.CanControlSimulation)]
    public class SimulatorController : BaseApiController
    {
        private readonly ISimulatorClient _simulationClient;

        public SimulatorController(IBuildingSecuritySessionStore sessionStore, ISimulatorClient simulationClient) : base(sessionStore)
        {
             _simulationClient = simulationClient;
        }

        public void Post(SimulatorScript container)
        {
            if (container == null) throw new ArgumentNullException("container");
            if (_simulationClient != null) _simulationClient.Run(container.Script);
        }
    }
}
