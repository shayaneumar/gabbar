/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BuildingSecurity.Web.App.Models;

namespace BuildingSecurity.Web
{
    public static class Reporting
    {
        #region Public methods for rendering Report Parameters

        public static MvcHtmlString HiddenParameters(this HtmlHelper htmlHelper, ReportParameters parameters)
        {
            if (htmlHelper != null && parameters != null)
            {            
                return htmlHelper.Hidden("Report.Parameters", Json.Encode(parameters.PersistedInfo));
            }

            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString LabeledParameterField(this HtmlHelper htmlHelper, ReportParameter parameter, bool disabled)
        {
            if (parameter == null) throw new ArgumentNullException("parameter");
            ReportControlType reportControlType = GetReportControlType(parameter);
            
            switch(reportControlType)
            {
                case ReportControlType.Hidden:      return Hidden(htmlHelper, parameter);
                case ReportControlType.CheckBox:    return LabeledCheckBox(htmlHelper, parameter, disabled);
                case ReportControlType.DatePicker:  return LabeledDatePicker(htmlHelper, parameter, disabled);
                case ReportControlType.ListBox:     return LabeledListBox(htmlHelper, parameter, disabled);
                case ReportControlType.DropDown:    return LabeledDropDown(htmlHelper, parameter, disabled);
                case ReportControlType.TextBox:     return LabeledTextBox(htmlHelper, parameter, disabled);
            }

            throw new ArgumentOutOfRangeException("parameter");
        }

        public static MvcHtmlString Hidden(this HtmlHelper htmlHelper, ReportParameter parameter)
        {
            if (htmlHelper != null && parameter != null)
            {
                return htmlHelper.Hidden(parameter.Id, parameter.CurrentValue);
            }

            return MvcHtmlString.Empty;
        }

        private static MvcHtmlString LabeledCheckBox(this HtmlHelper htmlHelper, ReportParameter parameter, bool disabled = false)
        {
            return LabeledControl(htmlHelper, parameter, CheckBoxForParameter(htmlHelper, parameter, disabled));

        }

        private static MvcHtmlString LabeledDropDown(this HtmlHelper htmlHelper, ReportParameter parameter, bool disabled = false)
        {
            if (!IsVisibleDropDown(parameter)) return MvcHtmlString.Empty;

            return LabeledControl(htmlHelper, parameter, IsDateRangeDropDown(parameter)
                                                              ? DateRangeDropDownForParameter(htmlHelper, parameter, disabled)
                                                              : DropDownForParameter(htmlHelper, parameter, disabled));

        }

        private static MvcHtmlString LabeledListBox(this HtmlHelper htmlHelper, ReportParameter parameter, bool disabled = false)
        {
            return LabeledControl(htmlHelper, parameter, ListBoxForParameter(htmlHelper, parameter, disabled));
        }

        private static MvcHtmlString LabeledTextBox(this HtmlHelper htmlHelper, ReportParameter parameter, bool disabled = false)
        {
            return LabeledControl(htmlHelper, parameter, TextBoxForParameter(htmlHelper, parameter, disabled));
        }

        private static MvcHtmlString LabeledDatePicker(this HtmlHelper htmlHelper, ReportParameter parameter, bool disabled = false)
        {
            if (!IsVisibleDateTime(parameter)) return MvcHtmlString.Empty;

            return LabeledControl(htmlHelper, parameter, DatePickerForParameter(htmlHelper, parameter, disabled));
        }
        
        #endregion

        #region Private methods to generate / return the input controls
        
        private static MvcHtmlString CheckBoxForParameter(HtmlHelper htmlHelper, ReportParameter reportParameter, bool disabled)
        {
            bool value;
            if (!bool.TryParse(reportParameter.CurrentValue, out value)) value = false;

            Dictionary<string, object> htmlAttributes = GetHtmlAttributes(reportParameter.Dependents, null, disabled);

            return htmlHelper.CheckBox(reportParameter.Id, value, htmlAttributes);
        }

        private static MvcHtmlString DropDownForParameter(HtmlHelper htmlHelper, ReportParameter reportParameter, bool disabled)
        {
            Dictionary<string, object> htmlAttributes = GetHtmlAttributes(reportParameter.Dependents, null, disabled);
            
            return htmlHelper.DropDownList(reportParameter.Id, reportParameter.Options, htmlAttributes);
        }

        private static MvcHtmlString ListBoxForParameter(HtmlHelper htmlHelper, ReportParameter reportParameter, bool disabled)
        {
            Dictionary<string, object> htmlAttributes = GetHtmlAttributes(reportParameter.Dependents, null, disabled);

            return htmlHelper.ListBox(reportParameter.Id, reportParameter.Options, htmlAttributes);
        }

        private static MvcHtmlString DateRangeDropDownForParameter(HtmlHelper htmlHelper, ReportParameter reportParameter, bool disabled)
        {
            Dictionary<string, object> htmlAttributes = GetHtmlAttributes(reportParameter.Dependents, null, disabled, "enableDependents");
            htmlAttributes.Add("data-value", ReportParameter.CustomDateRangeValue);

            return htmlHelper.DropDownList(reportParameter.Id, reportParameter.Options, htmlAttributes);
        }

        private static MvcHtmlString TextBoxForParameter(HtmlHelper htmlHelper, ReportParameter reportParameter, bool disabled)
        {
            Dictionary<string, object> htmlAttributes = GetHtmlAttributes(reportParameter.Dependents, reportParameter.Suggestions, disabled);

            return htmlHelper.TextBox(reportParameter.Id, reportParameter.CurrentValue, htmlAttributes);
        }

        private static MvcHtmlString DatePickerForParameter(HtmlHelper htmlHelper, ReportParameter reportParameter, bool disabled)
        {
            Dictionary<string, object> htmlAttributes = GetHtmlAttributes(reportParameter.Dependents, null, disabled);
            htmlAttributes.Add("class", "datepicker");

            return htmlHelper.TextBox(reportParameter.Id, reportParameter.CurrentValue, htmlAttributes);
        }
        
        #endregion

        #region Helper methods to prefix the input controls with labels and generate control attributes

        private static MvcHtmlString LabeledControl(this HtmlHelper htmlHelper, ReportParameter parameter, MvcHtmlString inputControl)
        {
            if (htmlHelper != null && parameter != null)
            {
                return MvcHtmlString.Create(string.Format(CultureInfo.InvariantCulture, "{1}{0}{2}", Environment.NewLine,
                                                          htmlHelper.Label(parameter.Id, parameter.Label),
                                                          inputControl));
            }

            return MvcHtmlString.Empty;
        }

        private static Dictionary<string, object> GetHtmlAttributes(string dependents, string suggestions, bool disabled, string changeEvent = "updateDependents")
        {
            var htmlAttributes = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(dependents))
            {
                htmlAttributes.Add("data-bind", "event : {change: " + changeEvent + "}");
                htmlAttributes.Add("data-dependents", dependents);
            }

            if (!string.IsNullOrEmpty(suggestions))
            {
                htmlAttributes.Add("data-provide", "typeahead");
                htmlAttributes.Add("data-items", "4");
                htmlAttributes.Add("data-source", suggestions);
            }

            if (disabled)
            {
                htmlAttributes.Add("disabled", "disabled");
            }

            return htmlAttributes;
        }
        
        #endregion

        #region Helper methods specific for some controls

        private static bool IsDateRangeDropDown(ReportParameter reportParameter)
        {
            return reportParameter.Options.Any(o => o.Value == ReportParameter.CustomDateRangeValue);
        }

        private static bool IsVisibleDateTime(ReportParameter reportParameter)
        {
            if (reportParameter.Scheduling)
            {
                switch (reportParameter.Id.ToUpperInvariant())
                {
                    case "DATETIMEFROM":
                    case "DATETIMETO":
                        return false;
                }
            }

            return true;
        }

        private static bool IsVisibleDropDown(ReportParameter reportParameter)
        {
            if (reportParameter.Scheduling)
            {
                return !IsDateRangeDropDown(reportParameter);
            }

            return true;
        }

        #endregion

        public static ReportControlType GetReportControlType(ReportParameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException("parameter");

            if (parameter.Hidden)
            {
                return ReportControlType.Hidden;
            }

            switch (parameter.ParameterType)
            {
                case ParameterType.Boolean:
                    return ReportControlType.CheckBox;

                case ParameterType.DateTime:
                    return ReportControlType.DatePicker;

                case ParameterType.Float:
                case ParameterType.Integer:
                case ParameterType.String:
                    if (parameter.Options.Any() || parameter.ItemParameter.ValidValuesQueryBased)
                    {
                        if (parameter.MultiValue)
                        {
                            return ReportControlType.ListBox;
                        }

                        return ReportControlType.DropDown;
                    }

                    return ReportControlType.TextBox;

                default:
                    return ReportControlType.Unsupported;
            }
        }

        public static int GetReportParameterHeight(ReportParameter reportParameter)
        {
            ReportControlType reportControlType = GetReportControlType(reportParameter);
            return GetReportControlTypeHeight(reportControlType);
        }

        private static int GetReportControlTypeHeight(ReportControlType reportControlType)
        {
            switch (reportControlType)
            {
                case ReportControlType.Hidden: return HeightHidden;
                case ReportControlType.CheckBox: return HeightCheckBox;
                case ReportControlType.DatePicker: return HeightDatePicker;
                case ReportControlType.ListBox: return HeightListBox;
                case ReportControlType.DropDown: return HeightDropDown;
                case ReportControlType.TextBox: return HeightTextBox;
            }

            return 0;
        }

        private const int HeightHidden = 0;
        private const int HeightCheckBox = 60;
        private const int HeightDatePicker = 60;
        private const int HeightListBox = 102;
        private const int HeightDropDown = 60;
        private const int HeightTextBox = 60;
    }
}
