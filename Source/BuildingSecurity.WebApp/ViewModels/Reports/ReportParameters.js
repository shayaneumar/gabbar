/*----------------------------------------------------------------------------

(C) Copyright 2012 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global $: false, jci: false, ko: false, window: false */

(function (jci) {
    "use strict";

    var reports = (function () {
        var disableAttributeName = "disabled",
            selectedAttributeName = "selected",
            valueAttributeName = "value",
            dataDependentsAttributeName = "data-dependents",
            dataValueAttributeName = "data-value",
            optionElement = "<option></option>",
            reportIdSelector = "#ReportId",
            dataSourceId = "DataSource",
            dataSourceSelector = jci.makeIdSelector(dataSourceId);

        function getParameterUpdate(parameterName, parameterUpdates) {
            var index, parameterUpdate;
            for (index = 0; index < parameterUpdates.length; index += 1) {
                parameterUpdate = parameterUpdates[index];
                if (parameterName === parameterUpdate.Name) {
                    return parameterUpdate;
                }
            }

            return jci.unDefined.value();
        }

        function enable($element) {
            $element.removeAttr(disableAttributeName);
        }

        function enableElementId(elementId) {
            enable($(jci.makeIdSelector(elementId)));
        }

        function disable($element) {
            $element.attr(disableAttributeName, "");
        }

        function disableElementId(elementId) {
            disable($(jci.makeIdSelector(elementId)));
        }

        function clearValue(elementId) {
            $(jci.makeIdSelector(elementId)).val("");
        }

        function select($element) {
            $element.attr(selectedAttributeName, "");
        }

        function addOption($select, key, value, selected) {
            var $option = $(optionElement).attr(valueAttributeName, key).text(value);
            if (selected) {
                select($option);
            }
            $select.append($option);
        }

        function onParameterUpdates($element, dependentSelects, parameterUpdates, useDefaults) {
            var index, $select, parameterUpdate, useDefault, selectedOptions, optionIndex, selected, key, value, hadSelection, foundSelection, validValueIndex;

            if (dependentSelects !== null && dependentSelects !== undefined) {
                for (index = 0; index < dependentSelects.length; index += 1) {
                    $select = $(jci.makeIdSelector(dependentSelects[index]));
                    parameterUpdate = getParameterUpdate(dependentSelects[index], parameterUpdates);
                    if (parameterUpdate !== jci.unDefined.value()) {
                        selectedOptions = {};
                        useDefault = true;
                        hadSelection = false;
                        foundSelection = false;

                        if (($select[0].options !== undefined) && (!useDefaults)) {
                            useDefault = useDefaults || ($select[0].options.length === 0);

                            for (optionIndex = 0; optionIndex < $select[0].options.length; optionIndex += 1) {
                                if ($select[0].options[optionIndex].selected) {
                                    selectedOptions[$select[0].options[optionIndex].value] = true;
                                    hadSelection = true;
                                }
                            }
                        }

                        $select.empty();

                        for (validValueIndex = 0; validValueIndex < parameterUpdate.Options.length; validValueIndex += 1) {
                            key = parameterUpdate.Options[validValueIndex].Key;
                            value = parameterUpdate.Options[validValueIndex].Value;

                            // If the selectedOptions object contains a property with a name = key,
                            // and the value of that property = true (that is the only value it would have been set to),
                            // then set selected = true
                            selected = Boolean(selectedOptions[key]);
                            if (selected) {
                                foundSelection = true;
                            }

                            // If useDefault = true (because $select[0].options = undefined, or $select[0].options.length = 0),
                            // and parameterUpdate.DefaultValues <> null and it contains at least 1 item,
                            // and the value of parameterUpdate.DefaultValues[0] = key,
                            // then set selected = true
                            if ((useDefault) && ((parameterUpdate.DefaultValues) && (parameterUpdate.DefaultValues.length > 0)) && (key === parameterUpdate.DefaultValues[0])) {
                                selected = true;
                            }

                            addOption($select, key, value, selected);
                        }

                        if (($select[0].type === "select-one") && (hadSelection) && (!foundSelection)) {
                            $select[0].selectedIndex = -1;
                        }
                    }

                    enable($select);
                }
            }

            enable($element);
        }

        function onError($element, dependentSelectIds) {
            var index;

            for (index = 0; index < dependentSelectIds.length; index += 1) {
                enableElementId(dependentSelectIds[index]);
            }

            enable($element);
        }

        function enableElementDependents(element) {
            var $element = $(element),
                dependents = $element.attr(dataDependentsAttributeName).split(","),
                enabled = $element.attr(dataValueAttributeName) === $element.val(),
                index;

            for (index = 0; index < dependents.length; index += 1) {
                if (enabled) {
                    enableElementId(dependents[index]);
                } else {
                    clearValue(dependents[index]);
                    disableElementId(dependents[index]);
                }
            }
        }

        function getParameters($sourceSelect, reportId, dataSource, parameterValues, dependentSelects, useDefaults) {
            var index,
                wasSubmitDisabled,
                submitButton;

            disable($sourceSelect);
            submitButton = $sourceSelect.closest('form').find(':submit');
            wasSubmitDisabled = submitButton.is(':disabled');
            disable(submitButton);
            if (dependentSelects !== null && dependentSelects !== undefined) {
                for (index = 0; index < dependentSelects.length; index += 1) {
                    disableElementId(dependentSelects[index]);
                }
            }

            // Retrieve updated selections
            jci.buildingSecurityClient.getDependentParameterValues(reportId, dataSource, parameterValues,
                function (parameterUpdates) {
                    if (!wasSubmitDisabled) { enable(submitButton); }
                    onParameterUpdates($sourceSelect, dependentSelects, parameterUpdates, useDefaults);
                },
                function () {
                    if (!wasSubmitDisabled) { enable(submitButton); }
                    onError($sourceSelect, dependentSelects);
                }
                );
        }

        return {
            // Handle the changing of a parameter that has dependent parameters
            // which should be enabled /disabled based on matching the trigger value
            enableDependents: function (unused, event) {
                enableElementDependents(event.target);
            },

            performInitialDependentBinding: function () {
                $.each($("[data-value]"), function (unused, value) {
                    enableElementDependents(value);
                });
            },

            // Handle the changing of a parameter that has dependent parameters
            updateDependents: function (unused, event) {
                var $sourceSelect = $(event.target),
                    reportId = $(reportIdSelector).val(),
                    dataSource = $(dataSourceSelector).val(),
                    dependentSelects = ($sourceSelect.attr(dataDependentsAttributeName) !== undefined) ? $sourceSelect.attr(dataDependentsAttributeName).split(",") : null,
                    name,
                    value,
                    index,
                    parameters = JSON.parse(event.target.form["Report.Parameters"].value),
                    parameterValues = [];

                for (index = 0; index < parameters.length; index += 1) {
                    name = parameters[index].Name;
                    value = $(jci.makeIdSelector(name)).val();

                    if ($.isArray(value)) {
                        value = value.join();
                    }

                    if (value === "") {
                        value = null;
                    }

                    parameterValues.push({ 'name': name, 'value': value });
                }

                getParameters($sourceSelect, reportId, dataSource, parameterValues, dependentSelects, false);
            },

            updateParameters: function (unused, event) {
                var $sourceSelect = $(dataSourceSelector),
                    reportId = $(reportIdSelector).val(),
                    dataSource = $sourceSelect.val(),
                    parameterValues = null,
                    dependentSelects = ($sourceSelect.attr(dataDependentsAttributeName) !== undefined) ? $sourceSelect.attr(dataDependentsAttributeName).split(",") : null,
                    useDefaults = true;

                if (event === null) {
                    // updateParameters() may be called via an event when the Data Source drop-down is changed (which includes an event object),
                    // or upon document.ready (which passed null for event)
                    useDefaults = false;
                }

// ReSharper disable ExpressionIsAlwaysConst; keep as named value for clarity
                getParameters($sourceSelect, reportId, dataSource, parameterValues, dependentSelects, useDefaults);
// ReSharper restore ExpressionIsAlwaysConst
            },

            updateDataSource: function (reportId, dataSource) {
                window.location.search = jci.getQueryString({ reportId: reportId, dataSource: dataSource });
            }
        };
    }());

    // Expose jci to the global object
    jci.reports = reports;
}(window.jci));