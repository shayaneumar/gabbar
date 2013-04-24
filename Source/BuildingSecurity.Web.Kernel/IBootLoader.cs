/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using Ninject;

namespace BuildingSecurity.Web
{
    public interface IBootLoader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
            Justification = "MGW: We expect that the injection code WILL have lots of dependencies.")]
        IKernel CreateKernel(Uri serviceUrl, string realTimeServiceAddress);
    }
}
