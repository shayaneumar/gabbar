/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

// These "filter" classes don't match the names in the XML. This is intentional.
// In some cases doing so would clash with another C# class. In others, we just
// want to be consistent that all filters are handled the same.

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    public abstract class CurrentValueFilter
    {
// ReSharper disable InconsistentNaming
        public string CV { get; set; }
// ReSharper restore InconsistentNaming
    }
}
