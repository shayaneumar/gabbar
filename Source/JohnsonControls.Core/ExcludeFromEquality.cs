/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;

namespace JohnsonControls
{
    /// <summary>
    /// Class, properties and fields tagged with this attribute should be ignored
    /// when checking equality through the use of reflection.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class|AttributeTargets.Property|AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class ExcludeFromEqualityAttribute : Attribute
    {}
}
