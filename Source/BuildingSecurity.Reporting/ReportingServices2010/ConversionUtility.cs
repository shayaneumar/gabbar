/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingSecurity.Reporting.ReportingServices2010
{
    internal static class ConversionUtility
    {
        public static ItemParameter ToDomain(this ReportingService.ItemParameter itemParameter)
        {
            if(itemParameter == null) throw new ArgumentNullException("itemParameter");

            return new ItemParameter(errorMessage: itemParameter.ErrorMessage
                                       , parameterStateName: itemParameter.ParameterStateName
                                       , defaultValues: itemParameter.DefaultValues
                                       , name: itemParameter.Name
                                       , parameterTypeName: itemParameter.ParameterTypeName
                                       , nullable: itemParameter.Nullable
                                       , nullableSpecified: itemParameter.NullableSpecified
                                       , allowBlank: itemParameter.AllowBlank
                                       , allowBlankSpecified: itemParameter.AllowBlankSpecified
                                       , multiValue: itemParameter.MultiValue
                                       , multiValueSpecified: itemParameter.MultiValueSpecified
                                       , queryParameter: itemParameter.QueryParameter
                                       , queryParameterSpecified: itemParameter.QueryParameterSpecified
                                       , prompt: itemParameter.Prompt
                                       , promptUser: itemParameter.PromptUser
                                       , promptUserSpecified: itemParameter.PromptUserSpecified
                                       , dependencies: itemParameter.Dependencies
                                       , validValuesQueryBased: itemParameter.ValidValuesQueryBased
                                       , validValuesQueryBasedSpecified: itemParameter.ValidValuesQueryBasedSpecified
                                       , validValues: itemParameter.ValidValues.ToDomain()
                                       , options: itemParameter.ValidValues.ToKeyValuePair()
                                       , defaultValuesQueryBased: itemParameter.DefaultValuesQueryBased
                                       , defaultValuesQueryBasedSpecified: itemParameter.DefaultValuesQueryBasedSpecified
                                       , hidden: itemParameter.Prompt == "");
        }

        public static ItemParameter[] ToDomain(this ReportingService.ItemParameter[] itemParameters)
        {
            if (itemParameters == null) return new ItemParameter[0];
            return itemParameters.Select(ToDomain).ToArray();
        }

        public static IDictionary<string, string> ToDomain(this ReportingService.ValidValue[] validValues)
        {
            var result = new Dictionary<string, string>();
            if (validValues == null) return result;
            foreach (var validValue in validValues)
            {
                //The validValue.Value are not guaranteed to be unique so we can not use linq.  This will
                //cause us to just use the last validValue.Value we see.
                result[validValue.Value] = validValue.Label;
            }
            return result;
        }

        public static KeyValuePair<string, string>[] ToKeyValuePair(this ReportingService.ValidValue[] validValues)
        {
            if (validValues == null) return new KeyValuePair<string, string>[0];
            return validValues.Select(v => new KeyValuePair<string, string>(v.Value, v.Label)).ToArray();
        }

        public static ReportingService.ParameterValue ToSsrsObject(this ParameterValue parameterValue)
        {
            if (parameterValue == null) throw new ArgumentNullException("parameterValue");
            return new ReportingService.ParameterValue{Name = parameterValue.Name, Value = parameterValue.Value};
        }

        public static ReportingService.ParameterValue[] ToSsrsObject(this ParameterValue[] parameterValues)
        {
            //TODO: need to determine if null has a special meaning to SSRS
            if (parameterValues == null) return null;
            return parameterValues.Select(ToSsrsObject).ToArray();
        }

        public static ReportingService.ParameterValue[] ToSsrsObject(this IEnumerable<ParameterValue> parameterValues)
        {
            //TODO: need to determine if null has a special meaning to SSRS
            if (parameterValues == null) return null;
            return parameterValues.Select(ToSsrsObject).ToArray();
        }

        public static ParameterValue ToDomain(this ReportingService.ParameterValue parameterValue)
        {
            if (parameterValue == null) throw new ArgumentNullException("parameterValue");
            return new ParameterValue(parameterValue.Name, parameterValue.Value);
        }

        public static ParameterValue[] ToDomain(this ReportingService.ParameterValue[] parameterValues)
        {
            if (parameterValues == null) return new ParameterValue[0];
            return parameterValues.Select(ToDomain).ToArray();
        }
    }
}
