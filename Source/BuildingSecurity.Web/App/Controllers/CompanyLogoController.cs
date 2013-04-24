/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Security;
using System.Web.Mvc;

namespace BuildingSecurity.Web.App.Controllers
{
    public class CompanyLogoController : Controller
    {
        private static readonly byte[] EmptyPng = new byte[]
                                                      {
                                                          0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48,
                                                          0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x08, 0x00, 0x00, 0x00,
                                                          0x00, 0x3A, 0x7E, 0x9B, 0x55, 0x00, 0x00, 0x00, 0x04, 0x67, 0x41, 0x4D, 0x41, 0x00,
                                                          0x00, 0xB1, 0x8F, 0x0B, 0xFC, 0x61, 0x05, 0x00, 0x00, 0x00, 0x02, 0x74, 0x52, 0x4E,
                                                          0x53, 0x00, 0x00, 0x76, 0x93, 0xCD, 0x38, 0x00, 0x00, 0x00, 0x09, 0x70, 0x48, 0x59,
                                                          0x73, 0x00, 0x00, 0x0E, 0xC2, 0x00, 0x00, 0x0E, 0xC2, 0x01, 0x15, 0x28, 0x4A, 0x80,
                                                          0x00, 0x00, 0x00, 0x0A, 0x49, 0x44, 0x41, 0x54, 0x08, 0x1D, 0x63, 0x60, 0x00, 0x00,
                                                          0x00, 0x02, 0x00, 0x01, 0xCF, 0xC8, 0x35, 0xE5, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45,
                                                          0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
                                                      };
        [AllowAnonymous]
        public ActionResult CurrentLogo()
        {
            byte[] logo = null;
            try
            {
                string customLogo = Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\Johnson Controls\P2000WebUiData\logo.png");
                string customLogoX86 = Environment.ExpandEnvironmentVariables(@"%PROGRAMFILES(X86)%\Johnson Controls\P2000WebUiData\logo.png");
                
                if (System.IO.File.Exists(customLogo))
                {
                    logo = System.IO.File.ReadAllBytes(customLogo);
                }
                else if (System.IO.File.Exists(customLogoX86))
                {
                    logo = System.IO.File.ReadAllBytes(customLogoX86);
                }
            }
            catch (IOException)// Don't care just return an empty img
            {}
            catch(SecurityException)
            {}
            catch(UnauthorizedAccessException)
            {}

            return File(logo??EmptyPng, @"image/png");
        }
    }
}
