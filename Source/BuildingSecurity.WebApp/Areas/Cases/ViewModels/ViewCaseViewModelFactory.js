/*----------------------------------------------------------------------------

(C) Copyright 2013 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global define: false, $: false, document: false*/

define('viewCaseViewModelFactory', ['ko', 'cases', 'logging', 'caseNoteViewModelFactory', 'jci'],
    function (ko, cases, log, caseNoteViewModelFactory, jci) {
        "use strict";
        var internalMessages = ko.observableArray(),
            closed = 'closed';

        return function (id) {
            var self = this;

            function updateCase(caseDetails) {
                var noteIndex,
                    notes = [];
                //If update is not for this case, ignore it.
                if (self.id() !== caseDetails.id) {
                    return;
                }

                self.title(caseDetails.title);
                self.createdBy(caseDetails.createdBy);
                self.createdDateTime(caseDetails.createdDateTimeString);
                self.owner(caseDetails.owner);
                self.status(caseDetails.status);

                for (noteIndex = 0; noteIndex < caseDetails.notes.length; noteIndex += 1) {
                    notes.push(new caseNoteViewModelFactory(caseDetails.notes[noteIndex]));
                }

                self.notes(notes);
            }

            function initCase(caseDetails) {
                //Set the case id, and then update every other field.
                self.id(caseDetails.id);
                updateCase(caseDetails);
            }

            function viewCaseSucceeded(caseDetails) {
                initCase(caseDetails);
                self.messages([]);
            }

            function viewCaseFailed(xhr) {
                log.error(JSON.parse(xhr.responseText).Message);
                self.messages([{ messageType: 'error', text: JSON.parse(xhr.responseText).Message }]);
            }

            function createNoteSucceeded(message) {
                self.enteredNote(jci.string.empty());
                var newNote = new caseNoteViewModelFactory(message);
                self.notes.splice(0, 0, newNote);
            }

            function createNoteFailed(xhr) {
                log.error('Failed to create case.');
                internalMessages.push({ messageType: 'error', text: JSON.parse(xhr.responseText).Message });
            }

            function replaceContainerBoundChildren(container, newChildren) {
                container.children().each(function () {
                    $(this).unbind();
                    ko.cleanNode(this);
                });

                container.empty();

                newChildren.hide().appendTo(container);
                container.children().each(function () {
                    ko.applyBindings(self, this);
                });

                newChildren.fadeIn('fast');
            }

            function inactivateEditableCaseTitle() {
                var $container = $('#case-title-container'),
                    $templates = $(document.createElement('div')).html($('#part-templates').text()),
                    $newChildren = $templates.find('#editable-case-title').children().clone(false);

                $container.removeClass('active');
                self.titleTooltip('Click to edit');

                replaceContainerBoundChildren($container, $newChildren);
            }

            function updateCaseTitleSucceeded(updatedCase) {
                inactivateEditableCaseTitle();
            }

            function updateCaseTitleFailed(xhr) {
                log.error('Failed to create case.');
                internalMessages.push({ messageType: 'error', text: JSON.parse(xhr.responseText).Message });
            }

            function updateStatusSucceeded(caseDetails) {
                updateCase(caseDetails);
                log.info('Case status updated successfully.');
            }

            function updateStatusFailed(xhr) {
                log.error('Failed to update status of the case.');
                internalMessages.push({ messageType: 'error', text: JSON.parse(xhr.responseText).Message });
            }

            this.id = ko.observable();
            this.title = ko.observable();
            this.editableTitle = ko.observable(jci.string.empty());
            this.editableTitle.subscribe(function (newValue) {
                self.isEditableTitleValid(jci.string.trim(newValue) !== jci.string.empty());
            });

            this.isEditableTitleValid = ko.observable(false);
            this.createdBy = ko.observable();
            this.createdDateTime = ko.observable();
            this.owner = ko.observable();
            this.status = ko.observable();
            this.notes = ko.observableArray();

            this.messages = internalMessages;
            this.status = ko.observable();

            this.enteredNote = ko.observable(jci.string.empty());
            this.isValidNote = ko.computed(function () {
                return (jci.string.trim(self.enteredNote()) !== jci.string.empty());
            });

            this.createNoteClick = function () {
                if (self.isValidNote()) {
                    this.messages.removeAll();
                    cases.createNote(self.id(), self.enteredNote(), createNoteSucceeded, createNoteFailed);
                }
            };

            this.isClosed = ko.computed(function () {
                return self.status() === closed;
            });

            this.titleTooltip = ko.observable('Click to edit');

            this.editCaseTitle = function (unused, event) {
                var $container = $(event.target).closest('.editable'),
                    $templates = $(document.createElement('div')).html($('#part-templates').text()),
                    $newChildren = $templates.find('#edit-case-title-form').children().clone(false);

                // Hack to deal with click being called when the toolbar buttons are pressed
                // Really would like to prevent this from bubbling
                if (!$container.hasClass('editable')) {
                    return;
                }

                // Ignore clicks when we are already active
                if ($container.hasClass('active')) {
                    return;
                }

                $container.addClass('active');
                replaceContainerBoundChildren($container, $newChildren);

                self.titleTooltip(jci.string.empty());
                self.editableTitle(self.title());

                $container.find('input[type="text"]').focus();
            };

            this.acceptEditCaseTitle = function () {
                if (self.isEditableTitleValid()) {
                    cases.updateCaseTitle(self.id(), self.editableTitle(), updateCaseTitleSucceeded, updateCaseTitleFailed);
                }
            };

            this.onTitleInputKeypress = function (unused, event) {
                if (event.which === jci.keys.enter()) {
                    $(event.target).change();
                    if (self.isEditableTitleValid()) {
                        cases.updateCaseTitle(self.id(), self.editableTitle(), updateCaseTitleSucceeded, updateCaseTitleFailed);
                    }
                    return false; // Prevent the default handler
                }

                return true; // Let the default handler execute
            };

            this.cancelEditCaseTitle = function () {
                inactivateEditableCaseTitle();
            };

            this.caseStatusUpdateClick = function (data, event) {
                this.messages.removeAll();
                var status = $(event.target).val();
                cases.updateStatus(self.id(), status, updateStatusSucceeded, updateStatusFailed);
            };

            cases.get(id, viewCaseSucceeded, viewCaseFailed);

            function onCasePushed(caseDetails) {
                updateCase(caseDetails);
            }

            jci.buildingSecurityClient.subscribeToCaseChannel(onCasePushed);
        };
    });