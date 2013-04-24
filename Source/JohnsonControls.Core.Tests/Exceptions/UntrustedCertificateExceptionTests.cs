/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.Exceptions
{
    [TestClass]
    public class UntrustedCertificateExceptionTests
    {
        [TestMethod]
        public void DefaultConstructorDoesNotThrowException()
        {
            //Act+Assert
            ActionAssert.DoesNotThrow<Exception>(() => new UntrustedCertificateException());
        }

        [TestMethod]
        public void ImplementsStandardConstructors()
        {
            //Act+Assert
            CustomExceptionAssert.HasStandardConstructors<UntrustedCertificateException>();
        }
    }
}
