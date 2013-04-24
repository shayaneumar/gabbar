/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace BuildingSecurity.Web.App.Models
{
    public enum ParameterType
    {
        Unsupported,
        Boolean,
        DateTime,
        Integer,
        Float,
        String
    }

    public static class ParameterTypeExtension
    {
        public static ParameterType GetParameterType(string parameterTypeName)
        {
            switch (parameterTypeName)
            {
                case "Boolean":     return ParameterType.Boolean;
                case "DateTime":    return ParameterType.DateTime;
                case "Float":       return ParameterType.Float;
                case "Integer":     return ParameterType.Integer;
                case "String":      return ParameterType.String;
                default:            return ParameterType.Unsupported;
            }
        }
    }
}
