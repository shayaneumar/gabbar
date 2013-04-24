/*----------------------------------------------------------------------------

(C) Copyright 2012 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global ko: false, jci: false */

function AlarmDisplayRange(serializedRange, previousRange, minLowerLimit, maxUpperLimit, isUpperFixed) {
    "use strict";

    var self = this,
        internalUpperLimit = ko.observable(serializedRange.upperLimit);

    self.prev = previousRange;
    self.isUpperLimitAdjustable = !isUpperFixed;
    self.isUpperLimitFixed = isUpperFixed;
    self.maxUpperLimit = maxUpperLimit;
    self.minLowerLimit = minLowerLimit;

    self.upperLimit = ko.computed({
        read: internalUpperLimit,
        write: function (value) {
            if (isNaN(value)) { return; } //Don't bother if it is not a number
            value = parseInt(value, 10);
            if (value >= minLowerLimit && value <= maxUpperLimit && !isUpperFixed) { //if value is valid an changeable
                if (self.prev !== jci.unDefined.value()) {
                    var previousUpper = parseInt(self.prev.upperLimit(), 10);
                    if (value <= previousUpper) {
                        self.prev.upperLimit(value - 1);
                    }
                }
                internalUpperLimit(value);
            }
        },
        owner: this
    });

    self.lowerLimit = ko.computed(function () {
        if (self.prev !== jci.unDefined.value()) {
            var limit = parseInt(self.prev.upperLimit(), 10) + 1;
            if (limit > self.upperLimit()) {
                self.upperLimit(limit);
            }
            return limit;
        }

        return serializedRange.lowerLimit;
    });

    self.color = ko.observable(serializedRange.color);

    self.audioAlertId = ko.observable(serializedRange.audioAlertId);


    self.postData = function () {
        return {
            id: serializedRange.id,
            lowerLimit: self.lowerLimit(),
            upperLimit: self.upperLimit(),
            color: self.color(),
            audioAlertId: self.audioAlertId()
        };
    };
}
