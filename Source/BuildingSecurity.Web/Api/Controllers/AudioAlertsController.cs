/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using BuildingSecurity.Globalization;
using BuildingSecurity.Web.Api.Models;

namespace BuildingSecurity.Web.Api.Controllers
{
    /// <summary>
    /// The <see cref="AudioAlertsController"/> provides the ability for a client application to
    /// retrieve the list of audio alert uniform resource identifiers for a specified audio encoding
    /// in a negotiated format, particularly JSON or XML, over the standard http protocol.
    /// </summary>
    public class AudioAlertsController : ApiController
    {
        /// <summary>
        /// Retrieve the complete list of audio alert uniform resource identifiers for the specified audio encoding.
        /// </summary>
        /// <param name="id">The specified encoding of the audio alerts needed, i.e. mp3, wav.</param>
        /// <returns>A list of uniform resource identifiers to the audio alerts in the specified audio encoding.</returns>
        public IEnumerable<AudioAlert> Get(string id)
        {
            string encoding = string.IsNullOrWhiteSpace(id) ? "unknown" : id.ToUpperInvariant();
            switch (encoding)
            {
                case "MP3":
                    return AudioAlertsForEncoding("mp3");
                case "WAV":
                    return AudioAlertsForEncoding("wav");

                default:
                    throw new HttpResponseException(HttpResponses.NotFoundMessage);
            }
        }

        private IEnumerable<AudioAlert> AudioAlertsForEncoding(string encoding)
        {
            return new []
                       {
                           new AudioAlert(new Guid("64755EEE-379A-461F-B57B-F44F9DC61E40"), new decimal(1.45), Resources.AudioAlertChime, ResourcePath("Audio/AlarmPending", encoding)),
                           new AudioAlert(new Guid("6D4861A6-18B3-45B8-A47A-A5D3DC4E8982"), new decimal(0.95), Resources.AudioAlertDrum, ResourcePath("Audio/Drum", encoding)),
                           new AudioAlert(new Guid("CEFFB4B3-07FD-4A3A-BB90-232F1D9B8073"), new decimal(3.9), Resources.AudioAlertEvacuation, ResourcePath("Audio/Evacuation", encoding)),
                           new AudioAlert(new Guid("8191A7DE-CCA4-447F-AC2E-C3101438AEE3"), new decimal(0.7), Resources.AudioAlertSiren, ResourcePath("Audio/Siren", encoding)),
                           new AudioAlert(new Guid("B76FFE24-C2B2-46F9-AE01-BCEC1C568D14"), new decimal(1), Resources.AudioAlertPager, ResourcePath("Audio/Pager", encoding)),
                           new AudioAlert(new Guid("2C3A3FC2-6259-469F-A822-B0EE84E35F75"), new decimal(3.1), Resources.AudioAlertFireAlarm, ResourcePath("Audio/FireAlarm", encoding))
                       };
        }

        private string ResourcePath(string resource, string encoding)
        {
            return string.Format(CultureInfo.InvariantCulture,
                Configuration.VirtualPathRoot.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? "{0}{1}.{2}" : "{0}/{1}.{2}",
                Configuration.VirtualPathRoot, resource, encoding);
        }
    }
}