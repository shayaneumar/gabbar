/*----------------------------------------------------------------------------

(C) Copyright 2013 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global define: false, $: false*/

define('caseListViewModelFactory', ['ko', 'cases', 'rivets', 'jci'],
    function (ko, cases, rivets, jci) {
        "use strict";

        var Case = function (id, title, status) {
            this.id = id;
            this.status = ko.observable(status);
            this.title = ko.observable(title);

            this.viewCase = rivets.getLinkablePath('Cases/' + this.id);

            this.viewCaseSoftLink = function () {
                rivets.softlink(this.viewCase);
            };

            this.update = function (caseUpdate) {
                //http://ecma-international.org/ecma-262/5.1/
                //JSlint is wrong, in this case we are using in correctly.
                //We are checking if the provided object has defined a value,
                //even if the defined value is null.
                if ('title' in caseUpdate) {
                    this.title(caseUpdate.title);
                }
                if ('status' in caseUpdate) {
                    this.status(caseUpdate.status);
                }
            };
        };

        return function () {
            var self = this,
                allCases = ko.observableArray(),
                intializeCaseListCallback = function (caseList) {
                    caseList.Data.forEach(function (caseDetails) {
                        allCases.push(new Case(caseDetails.id, caseDetails.title, caseDetails.status));
                    });
                },
                isOpen = function (caseDetails) {
                    // TODO: Apply actual filter once the status property of a case is available
                    return caseDetails.status() === 'open';
                },
                i;

            self.filterToOpen = ko.observable(true);
            self.cases = ko.computed(function () {
                return ko.utils.arrayFilter(allCases(), function (caseDetails) { return !self.filterToOpen() || isOpen(caseDetails); });
            }).extend({ throttle: 250 });

            cases.getAll(intializeCaseListCallback);

            self.onCasePushed = function (update) {
                for (i = 0; i < allCases().length; i = i + 1) {
                    if (allCases()[i].id === update.id) {
                        allCases()[i].update(update);
                        return;
                    }
                }
                allCases.push(new Case(update.id, update.title, update.status));
            };

            jci.buildingSecurityClient.subscribeToCaseChannel(self.onCasePushed);
        };
    });