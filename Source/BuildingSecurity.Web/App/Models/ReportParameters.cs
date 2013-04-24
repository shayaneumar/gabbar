/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using BuildingSecurity.Reporting;
using BuildingSecurity.Web.App.Controllers;

namespace BuildingSecurity.Web.App.Models
{
    [Serializable]
    public class ReportParameterColumnsDictionary : Dictionary<ReportParameterContainer, List<ReportParameter>>
    {
        public ReportParameterColumnsDictionary()
        {
        }

        protected ReportParameterColumnsDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    public class ReportParameters : DynamicObject
    {
        private readonly IEnumerable<ItemParameter> _parameters;
        private ReportParameterColumnsDictionary _reportParameterContainers;
        private readonly bool _scheduling;

        public ReportParameters(IEnumerable<ItemParameter> parameters, bool scheduling)
            : this(parameters, scheduling, null)
        {
        }

        public ReportParameters(IEnumerable<ItemParameter> parameters, bool scheduling, IEnumerable<ParameterValue> parameterValues)
        {
            _parameters = new List<ItemParameter>();
            _scheduling = scheduling;

            if (parameters == null) return;
            List<ItemParameter> itemParameterList = parameters.ToList();
            List<ParameterValue> parameterValueList = parameterValues != null
                                                          ? parameterValues.ToList()
                                                          : new List<ParameterValue>(0);

            foreach (ItemParameter itemParameter in itemParameterList)
            {
                ParameterValue parameterValue = parameterValueList.FirstOrDefault(pv => pv.Name == itemParameter.Name);
                if (parameterValue != null)
                {
                    itemParameter.SetDefaultValues(parameterValue.Value == null ? null : itemParameter.MultiValue ? parameterValue.Value.Split(',') : new [] { parameterValue.Value });
                }

                ((List<ItemParameter>) _parameters).Add(itemParameter);
            }
        }
        
        public IEnumerable<PersistedParameterInfo> PersistedInfo {
            get { return _parameters.Select(p => new PersistedParameterInfo(p.Name, ParameterTypeExtension.GetParameterType(p.ParameterTypeName), p.Nullable, ((p.DefaultValues != null) && (p.DefaultValues.Length > 0)) ? p.DefaultValues[0]: null)); }
        }

        private ReportParameter ConvertItemParamterToReportParameter(ItemParameter itemParameter)
        {
            ItemParameter suggestions = _parameters.FirstOrDefault(m => m.Name == itemParameter.Name + "Suggestions");
            IEnumerable<string> dependents = GetDependents(itemParameter);
            var reportParameter = new ReportParameter(itemParameter, suggestions, dependents, _scheduling);

            return reportParameter;
        }

        public ReportParameterColumnsDictionary ReportParameterContainers
        {
            get
            {
                if (_reportParameterContainers == null) InitializeReportParameterContainers();

                return _reportParameterContainers;
            }
        }

        private void InitializeReportParameterContainers()
        {
            _reportParameterContainers = new ReportParameterColumnsDictionary
                                         {
                                             {ReportParameterContainer.Column1, new List<ReportParameter>()},
                                             {ReportParameterContainer.Column2, new List<ReportParameter>()}
                                         };

            int totalHeight = GetTotalHeight();
            int runningHeight = 0;

            foreach (ItemParameter itemParameter in _parameters)
            {
                int controlHeight = GetItemParameterHeight(itemParameter);

                ReportParameterContainer reportParameterContainer = (runningHeight < totalHeight/2.0)
                                                                        ? ReportParameterContainer.Column1
                                                                        : ReportParameterContainer.Column2;

                _reportParameterContainers[reportParameterContainer].Add(ConvertItemParamterToReportParameter(itemParameter));
                runningHeight += controlHeight;
            }
        }

        private int GetItemParameterHeight(ItemParameter itemParameter)
        {
            ReportParameter reportParameter = ConvertItemParamterToReportParameter(itemParameter);
            return Reporting.GetReportParameterHeight(reportParameter);
        }

        private int GetTotalHeight()
        {
            return _parameters.Sum(itemParameter => GetItemParameterHeight(itemParameter));
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            ItemParameter itemParameter = _parameters.FirstOrDefault(m => m.Name == binder.Name);
            if (itemParameter != null)
            {
                result = ConvertItemParamterToReportParameter(itemParameter);
                return true;
            }

            result = null;
            return false;
        }

        private IEnumerable<string> GetDependents(ItemParameter parameter)
        {
            if (parameter.Name == "LanguageSelectSingle")
            {
                // Ignore any dependents on Language, parameters are always configured in site language.
                return Enumerable.Empty<string>();
            }

            if (parameter.Name == "DateTimeRangeTypeSelectSingle")
            {
                // This is a UI inspired dependency, we hide these fields if Custom is not
                // selected in the DateTimeRangeTypeSelectSingle drop down
                return _parameters.Where(IsDateTimeRangeTypeSelectSingleDependent).Select(p => p.Name);
            }

            return _parameters.Where(p => (p.Dependencies != null && p.Dependencies.Any(d => d == parameter.Name))).Select(p => p.Name);
        }

        private static bool IsDateTimeRangeTypeSelectSingleDependent(ItemParameter parameter)
        {
            switch (parameter.Name.ToUpperInvariant())
            {
                case "DATETIMEFROM":
                case "DATETIMETO":
                    return true;
            }

            return false;
        }
    }
}
