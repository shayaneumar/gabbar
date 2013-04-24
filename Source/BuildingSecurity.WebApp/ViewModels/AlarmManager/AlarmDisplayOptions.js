/*----------------------------------------------------------------------------

(C) Copyright 2012 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
//
// This is the Alarm Preferences view model, it encapsulates system and user preferences
// relevant to the alarm viewer.
///

/*global jci: false, ko: false, window: false*/
(function (jci) {
    "use strict";

    var alarmDisplayOptions = (function () {
        var priorityRangeDisplayOptions = [];

        function getOptionsForPriority(priority) {
            var index, priorityRangeOptions;
            for (index = 0; index < priorityRangeDisplayOptions.length; index += 1) {
                priorityRangeOptions = priorityRangeDisplayOptions[index];
                if (priorityRangeOptions.lowerLimit <= priority && priorityRangeOptions.upperLimit >= priority) {
                    return priorityRangeOptions;
                }
            }

            return jci.unDefined.value();
        }

        return {
            updateCount: window.ko.observable(0),

            getPriorityColor: function (priority) {
                var options = getOptionsForPriority(priority);
                if (options !== jci.unDefined.value()) {
                    return options.color;
                }

                return ""; //Default to transparent
            },

            getPriorityAudioAlertId: function (priority) {
                var options = getOptionsForPriority(priority);
                if (options !== jci.unDefined.value()) {
                    return options.audioAlertId;
                }

                return jci.unDefined.value();
            },

            update: function (updatedPriorityRangeDisplayOptions) {
                priorityRangeDisplayOptions = updatedPriorityRangeDisplayOptions;
                this.updateCount(this.updateCount() + 1);
            }
        };
    }());

    // Expose jci to the global object
    jci.alarmDisplayOptions = alarmDisplayOptions;
}(window.jci));
