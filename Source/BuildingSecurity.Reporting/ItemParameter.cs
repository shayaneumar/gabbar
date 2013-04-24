/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;

namespace BuildingSecurity.Reporting
{
    public class ItemParameter
    {
        public ItemParameter(string name, string parameterTypeName, bool nullable
            , bool nullableSpecified, bool allowBlank, bool allowBlankSpecified, bool multiValue
            , bool multiValueSpecified, bool queryParameter, bool queryParameterSpecified
            , string prompt, bool promptUser, bool promptUserSpecified, string[] dependencies
            , bool validValuesQueryBased, bool validValuesQueryBasedSpecified, IDictionary<string, string> validValues
            , KeyValuePair<string, string>[] options
            , bool defaultValuesQueryBased, bool defaultValuesQueryBasedSpecified, string[] defaultValues
            , string parameterStateName, string errorMessage, bool hidden)
        {
            Name = name;
            ParameterTypeName = parameterTypeName;
            Nullable = nullable;
            NullableSpecified = nullableSpecified;
            AllowBlank = allowBlank;
            AllowBlankSpecified = allowBlankSpecified;
            MultiValue = multiValue;
            MultiValueSpecified = multiValueSpecified;
            QueryParameter = queryParameter;
            QueryParameterSpecified = queryParameterSpecified;
            Prompt = prompt;
            PromptUser = promptUser;
            PromptUserSpecified = promptUserSpecified;
            Dependencies = dependencies;
            ValidValuesQueryBased = validValuesQueryBased;
            ValidValuesQueryBasedSpecified = validValuesQueryBasedSpecified;
            ValidValues = validValues;
            Options = options;
            DefaultValuesQueryBased = defaultValuesQueryBased;
            DefaultValuesQueryBasedSpecified = defaultValuesQueryBasedSpecified;
            DefaultValues = defaultValues;
            ParameterStateName = parameterStateName;
            ErrorMessage = errorMessage;
            Hidden = hidden;
        }

        public static ItemParameter CreateItemParameter(ItemParameter itemParameter, string prompt)
        {
            return new ItemParameter(
                itemParameter.Name,
                itemParameter.ParameterTypeName,
                itemParameter.Nullable,
                itemParameter.NullableSpecified,
                itemParameter.AllowBlank,
                itemParameter.AllowBlankSpecified,
                itemParameter.MultiValue,
                itemParameter.MultiValueSpecified,
                itemParameter.QueryParameter,
                itemParameter.QueryParameterSpecified,
                string.IsNullOrEmpty(prompt) ? itemParameter.Prompt : prompt,
                itemParameter.PromptUser,
                itemParameter.PromptUserSpecified,
                itemParameter.Dependencies,
                itemParameter.ValidValuesQueryBased,
                itemParameter.ValidValuesQueryBasedSpecified,
                itemParameter.ValidValues,
                itemParameter.Options,
                itemParameter.DefaultValuesQueryBased,
                itemParameter.DefaultValuesQueryBasedSpecified,
                itemParameter.DefaultValues,
                itemParameter.ParameterStateName,
                itemParameter.ErrorMessage,
                (itemParameter.Prompt == "")
                );
        }

        public void SetDefaultValues(string[] defaultValues)
        {
            DefaultValues = defaultValues;
        }

        public string Name { get; private set; }
        public string ParameterTypeName { get; private set; }

        public bool NullableSpecified { get; private set; }
        public bool Nullable { get; private set; }

        public bool AllowBlankSpecified { get; private set; }
        public bool AllowBlank { get; private set; }

        public bool MultiValueSpecified { get; private set; }
        public bool MultiValue { get; private set; }

        public bool QueryParameterSpecified { get; private set; }
        public bool QueryParameter { get; private set; }

        public bool PromptUserSpecified { get; private set; }
        public bool PromptUser { get; private set; }
        public string Prompt { get; private set; }

        public string[] Dependencies { get; private set; }

        public bool ValidValuesQueryBasedSpecified { get; private set; }
        public bool ValidValuesQueryBased { get; private set; }
        //TODO: Replace all usages of ValidValues with Options
        public IDictionary<string, string> ValidValues { get; private set; }
        public KeyValuePair<string, string>[] Options { get; private set; }

        public bool DefaultValuesQueryBasedSpecified { get; private set; }
        public bool DefaultValuesQueryBased { get; private set; }
        public string[] DefaultValues { get; private set; }

        public string ParameterStateName { get; private set; }
        public string ErrorMessage { get; private set; }

        public bool Hidden { get; private set; }
    }
}
