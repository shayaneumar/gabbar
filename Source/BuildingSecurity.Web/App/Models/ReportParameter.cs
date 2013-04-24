/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using BuildingSecurity.Reporting;

namespace BuildingSecurity.Web.App.Models
{
    public class ReportParameter
    {
        private readonly ItemParameter _itemParameter;
        private readonly ItemParameter _itemParameterSuggestions;
        private List<SelectListItem> _options;
        private string _suggestions;
        private readonly string _dependents;
        private readonly bool _scheduling;

        public static readonly string CustomDateRangeValue = "C8BB0CA5-C3A3-4495-B30B-F6E9245F39B8";

        public ReportParameter(ItemParameter itemParameter, ItemParameter itemParameterSuggestions, IEnumerable<string> dependents, bool scheduling)
        {
            _itemParameter = itemParameter;
            _itemParameterSuggestions = itemParameterSuggestions;
            _dependents = string.Join(",", dependents);
            _scheduling = scheduling;
        }

        public ReportParameter(ItemParameter itemParameter)
            : this(itemParameter, null, new List<string>(), false)
        {
        }

        public ItemParameter ItemParameter
        {
            get { return _itemParameter; }
        }

        public string Id
        {
            get { return _itemParameter.Name; }
        }

        public string Label
        {
            get { return _itemParameter.Prompt; }
        }

        public string CurrentValue
        {
            get { return ((_itemParameter.DefaultValues != null) && (_itemParameter.DefaultValues.Length > 0)) ? string.Join(",", _itemParameter.DefaultValues) : null; }
        }

        public bool Scheduling
        {
            get { return _scheduling; }
        }

        // http://msdn.microsoft.com/en-us/library/reportservice2010.reportingservice2010.listparametertypes.aspx
        public ParameterType ParameterType
        {
            get
            {
                return ParameterTypeExtension.GetParameterType(_itemParameter.ParameterTypeName);
            }
        }

        public bool Hidden
        {
            get { return _itemParameter.Hidden; }
        }

        public bool MultiValue
        {
            get { return _itemParameter.MultiValue; }
        }

        public IEnumerable<SelectListItem> Options
        {
            get
            {
                if (_options == null)
                {
                    if (_itemParameter.ValidValues != null)
                    {
                        _options = _itemParameter.ValidValues
                            .Select(v => new SelectListItem { Text = v.Value, Value = v.Key, Selected = ((_itemParameter.DefaultValues != null) && (_itemParameter.DefaultValues.Any(dv => dv == v.Key))) })
                            .ToList();
                    }
                    else
                    {
                        _options = new List<SelectListItem>();
                    }
                }

                return _options;
            }
        }

        public string Suggestions
        {
            get
            {
                if (_suggestions == null)
                {
                    if (_itemParameterSuggestions != null && _itemParameterSuggestions.ValidValues != null)
                    {
                        _suggestions = Json.Encode(_itemParameterSuggestions.ValidValues.Select(v => v.Value));
                    }
                    else
                    {
                        _suggestions = string.Empty;
                    }
                }

                return _suggestions;
            }
        }

        public string Dependents
        {
            get
            {
                return _dependents;
            }
        }
    }
}
