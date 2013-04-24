/*----------------------------------------------------------------------------

(C) Copyright 2013 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global define: false*/

define('createCaseViewModelFactory', ['ko', 'cases', 'rivets', 'jci'],
    function (ko, cases, rivets, jci) {
        'use strict';

        return function () {
            var self = this,
                caseCreationInProgress = ko.observable(false);

            function createCaseFailed(errorMessage) {
                caseCreationInProgress(false);
                self.messages.push(errorMessage);
            }

            function createCaseSucceeded(caseId) {
                rivets.softlink(rivets.getLinkablePath('Cases/' + caseId));
            }

            function beginCreateCase() {
                if (self.canCreateCase()) {
                    caseCreationInProgress(true);
                    self.messages([]);
                    cases.create(self.caseTitle(), createCaseSucceeded, createCaseFailed);
                }
            }

            this.messages = ko.observableArray();

            this.caseTitle = ko.observable(jci.string.empty());
            this.isCaseTitleValid = ko.computed(function () {
                return (jci.string.trim(self.caseTitle()) !== jci.string.empty());
            });

            this.createCaseClick = function () {
                beginCreateCase();
            };

            this.canCreateCase = ko.computed(function () {
                return (!caseCreationInProgress() && self.isCaseTitleValid());
            });

            this.onTitleInputKeypress = function (unused, event) {
                if (event.which === jci.keys.enter()) {
                    beginCreateCase();
                    return false; // Prevent the default handler
                }

                return true; // Let the default handler execute
            };
        };
    });