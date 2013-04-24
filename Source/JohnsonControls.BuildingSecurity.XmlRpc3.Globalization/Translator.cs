/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Globalization
{
    /// <summary>
    /// Static class used to retrieve string representations of enumerations returned by the P2000 v3.x
    /// </summary>
    public static class Translator
    {
        #region Public Methods
        /// <summary>
        /// Returns the string representation of the specified value of the specified valueType, localized to the specified localeId
        /// </summary>
        /// <param name="valueType">Enumeration value of the type</param>
        /// <param name="value">Integer value to be translated</param>
        /// <param name="culture"></param>
        /// <returns>String representation of the specified value of the specified valueType, localized to the specified localeId</returns>
        public static string GetString(CategoryType valueType, int value, CultureInfo culture)
        {
            var localeId = culture.LCID;

            lock (Lock)
            {
                if (!P2000Enums.ContainsKey(localeId))
                {
                    LoadTranslations(localeId);
                }
            }

            var key = GenerateKey(valueType, value);
            
            return P2000Enums[localeId].ContainsKey(key) ? P2000Enums[localeId][key] : key;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Loads all enumerations based on the specified locale, into P2000Enums
        /// </summary>
        /// <param name="localeId">ID of the Locale of the enumerations to load</param>
        private static void LoadTranslations(int localeId)
        {
            // If the P2000 provides an API method to retrieve translations, implement and call that instead of using the ResX
            P2000Enums.Add(localeId, GetTranslationsResX());
        }

        /// <summary>
        /// Returns a Dictionary of all enumerations from a resource file based on the specified localeId
        /// </summary>
        /// <returns>Dictionary of all enumerations from a resource file based on the specified localeId</returns>
        private static Dictionary<string, string> GetTranslationsResX()
        {
            var resourceManager = new ResourceManager(typeof(P2000Enum));

            return resourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true).Cast<DictionaryEntry>().ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());
        }

        /// <summary>
        /// Translates the enumerated ValueType into the string prefixed used within the dictionary
        /// </summary>
        /// <param name="valueType">Enumeration value of the type</param>
        /// <returns>Enumerated ValueType into the string prefixed used within the dictionary</returns>
        private static string GetPrefix(CategoryType valueType)
        {
            switch (valueType)
            {
                case CategoryType.AlarmStates:          return AlarmStatesPrefix;
                case CategoryType.FaultCodes:           return FaultCodesPrefix;
                case CategoryType.BooleanTypes:         return BooleanTypesPrefix;
                case CategoryType.ConditionStates:      return ConditionStatePrefix;
                default: return InvalidPrefix;
            }
        }

        /// <summary>
        /// Generate and return a resource item key based on the specified valueType and value
        /// </summary>
        /// <param name="valueType">Category type to be converted</param>
        /// <param name="value">Value to be converted</param>
        /// <returns>Resource item key based on the specified valueType and value</returns>
        private static string GenerateKey(CategoryType valueType, int value)
        {
            return GetPrefix(valueType) + (value < 0 ? "N" : "") + Math.Abs(value).ToString(CultureInfo.InvariantCulture);
        }
        #endregion

        #region Fields
        /// <summary>Dictionary containing the translations based on a key, formatted at [ENUMERATION_TYPE_] + [Integer Value]</summary>
        private static readonly Dictionary<int, Dictionary<string, string>> P2000Enums = new Dictionary<int, Dictionary<string, string>>();
        private static readonly object Lock = new object();
        #endregion

        #region Constants
        private const string AlarmStatesPrefix =            "ALARM_STATE_";
        private const string FaultCodesPrefix =             "FAULT_CODE_";
        private const string InvalidPrefix =                "INVALID_";
        private const string BooleanTypesPrefix =           "BOOLEAN_TYPE_";
        private const string ConditionStatePrefix =         "CONDITION_STATE_";
        #endregion
    }
}
