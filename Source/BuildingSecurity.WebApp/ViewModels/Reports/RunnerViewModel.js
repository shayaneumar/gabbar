/*----------------------------------------------------------------------------

(C) Copyright 2012 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global $: false, jci: false, ko: false, document: false */

//
// This is the Report Runner's view model. It is the view model for the entire page.
//
function RunnerViewModel() {
    "use strict";

    var self = this;

    // Behaviors

    // Handle the changing of a parameter that has dependent parameters
    // which should be enabled /disabled based on matching the trigger value
    self.enableDependents = jci.reports.enableDependents;

    // Handle the changing of a parameter that has dependent parameters
    self.updateDependents = jci.reports.updateDependents;

    // Handle the changing of the Data Source that may have many dependent parameters
    self.updateParameters = jci.reports.updateParameters;
}

//
// Initialize the page view model after the page is loaded and ready
///
$(document).ready(function () {
    "use strict";
    var viewModel = new RunnerViewModel();

    ko.applyBindings(viewModel);

    jci.reports.performInitialDependentBinding();
});