/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Mvc = System.Web.Mvc;

namespace BuildingSecurity.Web
{
    public sealed class WebApiResolverAdapter : IDependencyResolver
    {
        private readonly Mvc.IDependencyResolver _dependencyResolver;

        public WebApiResolverAdapter(Mvc.IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            return _dependencyResolver.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _dependencyResolver.GetServices(serviceType);
        }

        public void Dispose()
        {
        }
    }
}