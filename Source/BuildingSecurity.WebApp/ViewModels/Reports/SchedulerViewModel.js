/*----------------------------------------------------------------------------

(C) Copyright 2012 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global $: false, jci: false, ko: false, document: false, window: false */

//
// This is the Report Scheduler's view model. It is the view model for the entire page.
//
function SchedulerViewModel() {
    "use strict";

    var self = this;

    self.endDateSpecified = ko.observable(window.endDateSpecified);
    self.reportFreqency = ko.observable(window.reportFrequency);
    self.reportDestination = ko.observable(window.reportDestination);
    
    self.enableEndDate = ko.computed(function () {
        return self.endDateSpecified();
    });

    self.isDailySelected = ko.computed(function () {
        return self.reportFreqency() === "Daily";
    });

    self.isWeekySelected = ko.computed(function () {
        return !self.isDailySelected();
    });

    self.isShareSelected = ko.computed(function () {
        return self.reportDestination() === "FileShare";
    });

    self.isEmailSelected = ko.computed(function () {
        return !self.isShareSelected();
    });
    
    // Behaviors

    // Handle the changing of a parameter that has dependent parameters
    // which should be enabled /disabled based on matching the trigger value
    self.enableDependents = jci.reports.enableDependents;

    // Handle the changing of a parameter that has dependent parameters
    self.updateDependents = jci.reports.updateDependents;
}

//
// Initialize the page view model after the page is loaded and ready
///
$(document).ready(function () {
    "use strict";

    var viewModel = new SchedulerViewModel();
    ko.applyBindings(viewModel);

    jci.reports.performInitialDependentBinding();
});