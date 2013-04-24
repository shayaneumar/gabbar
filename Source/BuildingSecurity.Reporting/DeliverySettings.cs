/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using BuildingSecurity.Reporting.ReportingService;

namespace BuildingSecurity.Reporting
{
    public abstract class DeliverySettings
    {
        public abstract ExtensionSettings GetExtensionSettings();
        public abstract bool TryGetRenderFormatValue(out ReportOutputType renderFormatValue);
        public abstract new string ToString();

        public abstract ReportDestination ReportDestination { get; }

        public virtual EmailDeliverySettings ToEmailSettings()
        {
            return this as EmailDeliverySettings;
        }

        public virtual FileShareDeliverySettings ToFileShareSettings()
        {
            return this as FileShareDeliverySettings;
        }

        public static DeliverySettings CreateFromParameterValues(ParameterValueOrFieldReference[] objects)
        {
            if (objects == null) throw new ArgumentNullException();
            if (objects.Length == 0) throw new ArgumentException();

            if (!(objects[0] is ReportingService.ParameterValue)) throw new ArgumentException();
            List<ReportingService.ParameterValue> values = objects.Cast<ReportingService.ParameterValue>().ToList();

            // Logic to figure out file share or email
            if (values.FirstOrDefault(x => x.Name == "TO") != null)
            {
                return
                    new EmailDeliverySettings(new ExtensionSettings
                                                  {
                                                      ParameterValues = objects,
                                                      Extension = EmailDeliverySettings.ExtensionString
                                                  });
            }
            return new FileShareDeliverySettings(new ExtensionSettings
                                                     {
                                                         ParameterValues = objects, 
                                                         Extension = FileShareDeliverySettings.ExtensionString
                                                     });
        }
    }
}
