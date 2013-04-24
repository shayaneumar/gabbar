/*----------------------------------------------------------------------------

(C) Copyright 2012 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global ko: false, $: false, jci: false, setTimeout: false, clearTimeout: false,
    window: false, document: false, Alarm: false, Modernizr: false, Audio: false */

//
// This is the active view's view model. It is the view model for the entire page.
//
function AlarmManagerViewModel(settings, handlers) {
    "use strict";

    var self = this,
        audioAlerts = null,
        audioLoadedCount = 0,
        audioTimeout = null,
        modalShownEvent = "shown",
        modalShowCommand = "show",
        modalHideCommand = "hide",
        typeAheadDataKey = "typeahead",
        scrollEvent = "scroll",
        expandedAlarms = [],
        queuedAlarms = [],
        minimumDisplayAlarms = 30,
        displayAlarmChunkSize = 10, // Changing this requires a change to the addAlarmsToDisplay function
        sorting = ko.observable({
            primarySortOrder: jci.direction.Descending,
            primarySortKey: jci.alarmManagerColumns.OccurredDateTime,
            secondarySortOrder: jci.direction.Ascending,
            secondarySortKey: jci.alarmManagerColumns.Priority
        });

    function getAlarmsFor(predicate) {
        return self.selectedAlarms().filter(predicate)
            .map(function (alarm) { return alarm.actionParams(); });
    }

    function getSelectedPendingAlarms() {
        return getAlarmsFor(function (alarm) { return alarm.isPending(); });
    }

    function getSelectedCompletableAlarms() {
        return getAlarmsFor(function (alarm) { return alarm.isCompletable(); });
    }

    function getSelectedRespondableAlarms() {
        return getAlarmsFor(function (alarm) { return alarm.isRespondable(); });
    }

    // Method returns -1 if the values (determined based on the current sortKeys) are less for alarmA than alarmB; 1 if alarmB less than AlarmA; or 0 by default if equal
    function compareAlarms(alarmA, alarmB) {
        var alarmAPrimarySortValue = alarmA.getSortableValue(sorting().primarySortKey),
            alarmBPrimarySortValue = alarmB.getSortableValue(sorting().primarySortKey),
            alarmASecondarySortValue,
            alarmBSecondarySortValue;

        if (alarmAPrimarySortValue < alarmBPrimarySortValue) {
            return -sorting().primarySortOrder;
        }

        if (alarmAPrimarySortValue > alarmBPrimarySortValue) {
            return sorting().primarySortOrder;
        }

        alarmASecondarySortValue = alarmA.getSortableValue(sorting().secondarySortKey);
        alarmBSecondarySortValue = alarmB.getSortableValue(sorting().secondarySortKey);
        if (alarmASecondarySortValue < alarmBSecondarySortValue) {
            return -sorting().secondarySortOrder;
        }

        if (alarmASecondarySortValue > alarmBSecondarySortValue) {
            return sorting().secondarySortOrder;
        }

        return 0;
    }

    function resort() {
        self.activeAlarms.sort(compareAlarms);
        self.displayedAlarms(self.activeAlarms.slice(0, minimumDisplayAlarms));
        // Scroll back to the top of the list
        $(self.alarmListSelector).scrollTop(0);
    }

    // Private methods
    function internalSortByPending() {
        sorting({
            primarySortOrder: jci.direction.Descending,
            primarySortKey: jci.alarmManagerColumns.Importance,
            secondarySortOrder: jci.direction.Descending,
            secondarySortKey: jci.alarmManagerColumns.OccurredDateTime
        });
        resort();
    }

    function internalSortByTime() {
        sorting({
            primarySortOrder: jci.direction.Descending,
            primarySortKey: jci.alarmManagerColumns.OccurredDateTime,
            secondarySortOrder: jci.direction.Ascending,
            secondarySortKey: jci.alarmManagerColumns.Priority
        });
        resort();
    }

    // This implementation will try to push equal items as high in the list as possible
    // i.e. it assumes things arriving later are newer if the dates match
    function getSortedIndex(alarm, startIndex, endIndex) {
        var middleIndex = Math.floor((startIndex + endIndex) / 2);

        // The startIndex is our insert index
        if (startIndex > endIndex) {
            return startIndex;
        }

        // The last compare to determine before or after
        if (startIndex === endIndex) {
            if (self.activeAlarms().length > startIndex) {
                if (compareAlarms(alarm, self.activeAlarms()[startIndex]) > 0) {
                    return startIndex + 1;
                }
            }

            return startIndex;
        }

        // Keep looking in the top half...
        if (compareAlarms(alarm, self.activeAlarms()[middleIndex]) > 0) {
            return getSortedIndex(alarm, middleIndex + 1, endIndex);
        }

        // or the bottom half of the list
        return getSortedIndex(alarm, startIndex, middleIndex - 1);
    }

    function addAlarmsToDisplay(start, end) {
        var index, chunk = self.activeAlarms.slice(start, end);

        if (chunk.length === displayAlarmChunkSize) {
            self.displayedAlarms.push(chunk[0], chunk[1], chunk[2],
                chunk[3], chunk[4], chunk[5], chunk[6], chunk[7],
                chunk[8], chunk[9]);
        } else {
            for (index = 0; index < chunk.length; index += 1) {
                self.displayedAlarms.push(chunk[index]);
            }
        }
    }

    function ensureMinimumAlarmsDisplayed() {
        var start, end;

        if (self.displayedAlarms().length < minimumDisplayAlarms &&
                self.displayedAlarms().length < self.activeAlarms().length) {

            start = self.displayedAlarms().length;
            end = start + (minimumDisplayAlarms - start);
            addAlarmsToDisplay(start, end);
        }
    }

    function insertAlarmAtIndex(alarm, index) {
        self.activeAlarms.splice(index, 0, alarm);

        if (index < self.displayedAlarms().length) {
            self.displayedAlarms.splice(index, 0, alarm);
        }
    }

    // Inserts an alarm, at the index based on the current sorting rules
    function insertAlarm(alarm) {
        insertAlarmAtIndex(alarm, getSortedIndex(alarm, 0, self.activeAlarms().length - 1));
    }

    //Finds the index of a existing alarm with a specified Id. If alarm is not found a
    //a negative index will be returned.
    function indexOfAlarm(alarmId) {
        var index, alarm;
        for (index = 0; index < self.activeAlarms().length; index += 1) {
            alarm = self.activeAlarms()[index];
            if (alarm.id === alarmId) {
                return index;
            }
        }
        return -1;
    }

    //Finds the index of a existing alarm with a specified Id. If alarm is not found a
    //a negative index will be returned.
    function indexOfExpandedAlarm(alarmId) {
        var index, alarm;
        for (index = 0; index < expandedAlarms.length; index += 1) {
            alarm = expandedAlarms[index];
            if (alarm.id === alarmId) {
                return index;
            }
        }
        return -1;
    }

    function findActiveAlarm(alarmId) {
        var alarmIndex;

        alarmIndex = indexOfAlarm(alarmId);
        if (alarmIndex >= 0) {
            return self.activeAlarms()[alarmIndex];
        }

        return jci.unDefined.value();
    }

    function removeAlarmAtIndex(index) {
        self.activeAlarms.splice(index, 1);

        if (index < self.displayedAlarms().length) {
            self.displayedAlarms.splice(index, 1);
        }
    }

    function removeExistingAlarm(alarmId) {
        var alarmIndex, alarm;

        alarmIndex = indexOfAlarm(alarmId);
        if (alarmIndex >= 0) {
            alarm = self.activeAlarms()[alarmIndex];
            removeAlarmAtIndex(alarmIndex);
            return alarm;
        }

        return jci.unDefined.value();
    }

    function removeExistingQueuedAlarm(alarmId) {
        var index, alarm;

        for (index = 0; index < queuedAlarms.length; index += 1) {
            alarm = queuedAlarms[index];
            if (alarm.Id === alarmId) {
                queuedAlarms.splice(index, 1);
                return alarm;
            }
        }

        return jci.unDefined.value();
    }

    function queuePushedAlarm(serializedAlarm) {
        // Last update goes into the queue
        var existingAlarm = removeExistingQueuedAlarm(serializedAlarm.Id);

        if (existingAlarm) { // Were we already in the queue?
            // If we are not pending anymore, reduce pending count
            if (existingAlarm.isPending && !serializedAlarm.IsPending) {
                self.queuedPendingAlarmCount(self.queuedPendingAlarmCount() - 1);
            }
        } else {
            // Are we in the list
            existingAlarm = findActiveAlarm(serializedAlarm.Id);
            if (existingAlarm) { // We are in the list
                // If we are not pending anymore, reduce pending count
                if (existingAlarm.isPending() && !serializedAlarm.IsPending) {
                    self.queuedPendingAlarmCount(self.queuedPendingAlarmCount() - 1);
                }
            } else {
                // Increase the new alarm count
                self.queuedNewAlarmCount(self.queuedNewAlarmCount() + 1);

                // Increase the pending count if the alarm is pending
                if (serializedAlarm.IsPending) {
                    self.queuedPendingAlarmCount(self.queuedPendingAlarmCount() + 1);
                }
            }
        }

        queuedAlarms.push(serializedAlarm);
    }

    function processPushedAlarm(serializedAlarm) {
        var existingAlarm = removeExistingAlarm(serializedAlarm.Id);

        if (existingAlarm) {
            // Process changes and insert in correct position
            existingAlarm.processChanges(serializedAlarm);
            insertAlarm(existingAlarm);
        } else {
            // Don't insert completed alarms, only perform updates
            if (!serializedAlarm.IsCompleted) {
                // Add new alarm in correct position
                insertAlarm(new Alarm(serializedAlarm, { autoExpand: settings.autoExpand }));
                if ((handlers !== undefined) && (handlers.totalChanged !== undefined)) {
                    handlers.totalChanged(1);
                }
            }
        }
    }

    function onAlarmPushed(serializedAlarm) {
        var expandedAlarmIndex,
            alarmIndex = indexOfAlarm(serializedAlarm.Id),
            height = 0;

        if (expandedAlarms.lendth > 0) {
            expandedAlarmIndex = indexOfExpandedAlarm(serializedAlarm.Id);
            if (expandedAlarmIndex >= 0) {
                expandedAlarms[expandedAlarmIndex].processChanges(serializedAlarm);
            }

            if (settings.freezeWhenExpanded) {
                queuePushedAlarm(serializedAlarm);
            } else {
                processPushedAlarm(serializedAlarm);
                ensureMinimumAlarmsDisplayed();

                /* If inserting a new alarm above X (the alarm that is expanded, or the center of the alarms in view),
                   then increment scrollTop by the height of the alarm that was inserted.
                   Need to determine which is in view (or the center of the window), if the Alarm is being inserted or updated,
                   and where it will be inserted if it is new */
                if ((alarmIndex === -1) && (handlers !== undefined) && (handlers.alarmAdded !== undefined)) {
                    handlers.alarmAdded({ expanded: settings.autoExpand, height: $("#" + serializedAlarm.Id)[0].offsetHeight });
                }
            }
        } else {
            processPushedAlarm(serializedAlarm);
            ensureMinimumAlarmsDisplayed();
            if ((alarmIndex === -1) && (handlers !== undefined) && (handlers.alarmAdded !== undefined)) {
                // TODO: Account for margin-top; should use .outerHeight(true)
                // Increment height by the height of the DIV and the margin
                height += $("#" + serializedAlarm.Id)[0].offsetHeight;
                height += parseInt($("#" + serializedAlarm.Id).css("margin-bottom"), 10);
                handlers.alarmAdded({ expanded: settings.autoExpand, height: height });
            }
        }
    }

    function onHeartbeatPushed() {
    }

    function addNewAlarms(serializedAlarms) {
        var index, alarmVms = [];
        if (serializedAlarms.length > 0) {
            for (index = 0; index < serializedAlarms.length; index += 1) {
                if (indexOfAlarm(serializedAlarms[index].Id) < 0) { //ignore any alarm which already exists
                    alarmVms.push(new Alarm(serializedAlarms[index], { autoExpand: settings.autoExpand }));
                }
            }

            self.activeAlarms(alarmVms.concat(self.activeAlarms()).sort(compareAlarms));
            self.displayedAlarms(self.activeAlarms.slice(0, minimumDisplayAlarms));
        }
    }

    function onActiveAlarms(serializedAlarmsDataChunk) {
        if (serializedAlarmsDataChunk.IsEnd === false) {
            jci.buildingSecurityClient.getActiveAlarms(onActiveAlarms, serializedAlarmsDataChunk.LastElement.Id);
        } else {
            self.isLoading(false);
        }
        addNewAlarms(serializedAlarmsDataChunk.Data);
    }

    ///Initial alarm loading callback
    function onIntialAlarmLoad(serializedAlarmsDataChunk) {
        if (serializedAlarmsDataChunk.IsEnd === false) {
            jci.buildingSecurityClient.getActiveAlarms(onActiveAlarms);
        } else {
            self.isLoading(false);
        }
        addNewAlarms(serializedAlarmsDataChunk.Data);
    }

    function processQueuedAlarms() {
        var index;
        if (queuedAlarms.length > 0) {
            for (index = 0; index < queuedAlarms.length; index += 1) {
                processPushedAlarm(queuedAlarms[index]);
            }

            ensureMinimumAlarmsDisplayed();
            queuedAlarms = [];
            self.queuedNewAlarmCount(0);
            self.queuedPendingAlarmCount(0);
        }
    }

    function removeExpandedAlarm(alarmId) {
        var expandedAlarmIndex = indexOfExpandedAlarm(alarmId);

        if (expandedAlarmIndex >= 0) {
            expandedAlarms.splice(expandedAlarmIndex, 1);

            if ((expandedAlarms.length === 0) && (settings.freezeWhenExpanded)) {
                processQueuedAlarms();
            }
        }
    }

    function updateAlarmSelectionTracking(alarm) {
        if (alarm.selected()) {
            self.selectedAlarms.push(alarm);
            if (self.selectedAlarms().length === self.activeAlarms().length) {
                self.allSelected(true);
            }
        } else {
            self.selectedAlarms.remove(alarm);
            if (self.selectedAlarms().length === 0) {
                self.allSelected(false);
            }
        }
    }

    function setAlarmSelection(alarm, selected) {
        alarm.selected(selected);
        updateAlarmSelectionTracking(alarm);
    }

    function setAllAlarmsSelection(selected) {
        var index, alarm;

        for (index = 0; index < self.activeAlarms().length; index += 1) {
            alarm = self.activeAlarms()[index];
            if (selected !== alarm.selected()) {
                setAlarmSelection(alarm, selected);
            }
        }
    }

    function removeElement(collection, index) {
        collection[index] = collection[collection.length - 1];
        collection.pop();
    }

    function onAlarmOperationResponse(response) {
        var alarmIindex, alarm, responseIndex, operationResponse;

        for (alarmIindex = 0; alarmIindex < self.activeAlarms().length && response.length > 0; alarmIindex += 1) {
            alarm = self.activeAlarms()[alarmIindex];
            for (responseIndex = 0; responseIndex < response.length; responseIndex += 1) {
                operationResponse = response[responseIndex];
                if (operationResponse.id === alarm.id) {
                    alarm.update(operationResponse);
                    removeElement(response, responseIndex);
                    break;
                }
            }
        }
    }

    function removeActiveAlarms(alarms) {
        var index, alarm;
        for (index = 0; index < alarms.length; index += 1) {
            alarm = removeExistingAlarm(alarms[index].id);
            removeExpandedAlarm(alarms[index].id);

            if ((alarm !== jci.unDefined.value()) && (handlers !== undefined) && (handlers.totalChanged !== undefined)) {
                handlers.totalChanged(-1);
            }
        }

        ensureMinimumAlarmsDisplayed();
    }

    function onRemoveAlarmsResponse(response) {
        var alarmIindex, alarm, responseIndex, operationResponse, alarmsToRemove = [];

        for (alarmIindex = 0; alarmIindex < self.activeAlarms().length && response.length > 0; alarmIindex += 1) {
            alarm = self.activeAlarms()[alarmIindex];
            for (responseIndex = 0; responseIndex < response.length; responseIndex += 1) {
                operationResponse = response[responseIndex];
                if (operationResponse.id === alarm.id) {
                    alarm.update(operationResponse);
                    if (alarm.isCompleted() || !alarm.isErrorAvailable()) {
                        alarmsToRemove.push(alarm);
                    }
                    removeElement(response, responseIndex);
                    break;
                }
            }
        }

        removeActiveAlarms(alarmsToRemove);
    }

    function onAlarmResponses(responses) {
        self.definedResponses([jci.string.empty()].concat(responses));
    }

    function onAudioLoaded() {
        audioLoadedCount += 1;

        if (audioLoadedCount === audioAlerts.length) {
            self.audioLoaded(true);
        }
    }

    function onAudioAlerts(currentAudioAlerts) {
        var index, audioAlert;
        audioAlerts = currentAudioAlerts;
        audioLoadedCount = 0;

        for (index = 0; index < audioAlerts.length; index += 1) {
            audioAlert = audioAlerts[index];
            audioAlert.audio = new window.Audio(audioAlert.source);
            $(audioAlert.audio).on(jci.events.loadeddata(), onAudioLoaded);
        }
    }

    function audioPlaying() {
        return (audioTimeout !== jci.unDefined.value());
    }

    function audioAlertForId(audioAlertId) {
        var foundAlert = ko.utils.arrayFirst(audioAlerts, function (audioAlert) { return audioAlert.id === audioAlertId; });
        if (foundAlert !== null) {
            return foundAlert;
        }

        return jci.unDefined.value();
    }

    function getAudioAlert() {
        var pendingAlarms, highestPriorityPendingAlarm, audioAlertId;

        if (self.audioLoaded()) {
            pendingAlarms = self.activeAlarms().filter(function (alarm) { return alarm.isPending(); });

            if (pendingAlarms.length > 0) {
                if (pendingAlarms.length === 1) {
                    highestPriorityPendingAlarm = pendingAlarms[0];
                } else {
                    highestPriorityPendingAlarm = pendingAlarms.reduce(function (previousAlarm, currentAlarm) {
                        return (+previousAlarm.priority < +currentAlarm.priority) ? previousAlarm : currentAlarm;
                    });
                }

                if (highestPriorityPendingAlarm !== jci.unDefined.value()) {
                    audioAlertId = jci.alarmDisplayOptions.getPriorityAudioAlertId(highestPriorityPendingAlarm.priority);
                    return audioAlertForId(audioAlertId);
                }
            }
        }

        return jci.unDefined.value();
    }

    function playLoopingAudio() {
        var theAudioAlert = getAudioAlert();
        if (theAudioAlert !== jci.unDefined.value()) {
            theAudioAlert.audio.currentTime = 0;
            theAudioAlert.audio.play();
        }

        audioTimeout = setTimeout(function () { playLoopingAudio(); }, (theAudioAlert.duration * 1000) + 1000);
    }

    function stopLoopingAudio() {
        if (audioPlaying()) {
            var theAudioAlert = getAudioAlert();
            if (theAudioAlert !== jci.unDefined.value()) {
                theAudioAlert.audio.pause();
            }

            clearTimeout(audioTimeout);
            audioTimeout = jci.unDefined.value();
        }
    }

    function onDisplayOptions(alarmDisplayOptions) {
        jci.alarmDisplayOptions.update(alarmDisplayOptions);
        jci.buildingSecurityClient.getAudioAlerts(onAudioAlerts);
    }

    // Return the control ID of for the indicator icon for the specified sortKey and sortOrder
    function getSortIndicatorControlId(sortKey, sortOrder) {
        var directionSuffix = (sortOrder === jci.direction.Ascending) ? "Ascending" : "Descending";

        switch (sortKey) {
        case jci.alarmManagerColumns.Pending:
            return "directionPending" + directionSuffix;
        case jci.alarmManagerColumns.OccurredDateTime:
            return "directionOccurredDate" + directionSuffix;
        case jci.alarmManagerColumns.Priority:
            return "directionPriority" + directionSuffix;
        case jci.alarmManagerColumns.Status:
            return "directionStatus" + directionSuffix;
        case jci.alarmManagerColumns.Description:
            return "directionDescription" + directionSuffix;
        case jci.alarmManagerColumns.ActionDateTime:
            return "directionUpdatedDateTime" + directionSuffix;
        case jci.alarmManagerColumns.Importance:
            return "directionImportance" + directionSuffix;
        }

        return null;
    }

    // Return the default direction to sort based on the specified sortKey
    function getDefaultOrder(sortKey) {
        // The default order for OccurredDateTime, ActionDateTime, Pending is Descending, Ascending for all others
        if ((sortKey === jci.alarmManagerColumns.OccurredDateTime) ||
                (sortKey === jci.alarmManagerColumns.ActionDateTime) ||
                (sortKey === jci.alarmManagerColumns.Pending)) {
            return jci.direction.Descending;
        }

        return jci.direction.Ascending;
    }

    // Return the CSS ClassName to be used for the specified sortOrder
    function getSortClassName(sortOrder) {
        switch (sortOrder) {
        case jci.direction.Ascending:
            return "sortAscending";
        case jci.direction.Descending:
            return "sortDescending";
        case jci.direction.None:
            return "sortNone";
        default:
            return "sortNone";
        }
    }

    function hidePrimarySortIndicator() {
        $(jci.makeIdSelector(getSortIndicatorControlId(sorting().primarySortKey, sorting().primarySortOrder))).removeClass(getSortClassName(sorting().primarySortOrder));
        $(jci.makeIdSelector(getSortIndicatorControlId(sorting().primarySortKey, sorting().primarySortOrder))).addClass(getSortClassName(jci.direction.None));
    }

    function showPrimarySortIndicator() {
        $(jci.makeIdSelector(getSortIndicatorControlId(sorting().primarySortKey, sorting().primarySortOrder))).removeClass(getSortClassName(jci.direction.None));
        $(jci.makeIdSelector(getSortIndicatorControlId(sorting().primarySortKey, sorting().primarySortOrder))).addClass(getSortClassName(sorting().primarySortOrder));
    }

    // Set the primary and secondary sortKeys, or toggle the sortOrder of the primarySortKey if the specified key equals the current key
    function sortAlarms(primarySortKey, secondarySortKey) {
        var currentSort = sorting();
        hidePrimarySortIndicator();

        if (primarySortKey === currentSort.primarySortKey) {
            currentSort.primarySortOrder = -currentSort.primarySortOrder;
        } else {
            currentSort.primarySortOrder = getDefaultOrder(primarySortKey);
            currentSort.primarySortKey = primarySortKey;
        }

        currentSort.secondarySortOrder = getDefaultOrder(secondarySortKey);
        currentSort.secondarySortKey = secondarySortKey;

        sorting(currentSort);

        showPrimarySortIndicator();

        resort();
    }

    // Data
    // composite id's used for the response dialog
    self.respondDialogId = "respondDialog";
    self.respondDialogSelector = jci.makeIdSelector(self.respondDialogId);
    self.manualResponseId = "manualResponse";
    self.manualResponseSelector = jci.makeIdSelector(self.manualResponseId);
    self.alarmListId = "alarmList";
    self.alarmListSelector = jci.makeIdSelector(self.alarmListId);

    // Bound to the select all checkbox
    self.allSelected = ko.observable(false);

    // Bound to the respond dialog defined response selection
    self.definedResponse = ko.observable(jci.string.empty());

    // Bound to the respond dialog manual response text box
    self.manualResponse = ko.observable(jci.string.empty());

    // The number of new alarms received while the list is paused
    self.queuedNewAlarmCount = ko.observable(0);

    // The number of pending alarms being queued
    self.queuedPendingAlarmCount = ko.observable(0);

    // If alarm load is in progress
    self.isLoading = ko.observable(true);

    // This is the list of selected alarms, it is a convenience data
    // structure to help the view model manage selected alarms
    self.selectedAlarms = ko.observableArray();

    // This is the complete list of alarms known by the client
    self.activeAlarms = ko.observableArray();

    // This is the list of alarms currently being displayed
    self.displayedAlarms = ko.observableArray();

    // This is the defined responses bound to the respond dialog drop down
    self.definedResponses = ko.observableArray([jci.string.empty()]);

    // This is the flag used to indicate whether the audio files are loaded and audio can be played
    self.audioLoaded = ko.observable(false);

    // This is the defined responses packaged for the bootstrap type-ahead control
    self.definedResponsesArray = ko.computed(function () {
        return ko.toJSON(self.definedResponses);
    });

    self.lastNumberOfPendingAlarms = 0;

    // Computed value used to display total number of alarms to the user
    self.numberOfAlarms = ko.computed(function () {
        return self.activeAlarms().length + self.queuedNewAlarmCount();
    });

    // Computed value used to display number of pending alarms to the user
    self.numberOfPendingAlarms = ko.computed(function () {
        var index, pendingCount = 0;

        for (index = 0; index < self.activeAlarms().length; index += 1) {
            if (self.activeAlarms()[index].isPending()) {
                pendingCount += 1;
            }
        }

        if ((pendingCount + self.queuedPendingAlarmCount() !== self.lastNumberOfPendingAlarms) && (handlers !== undefined) && (handlers.pendingChanged !== undefined)) {
            handlers.pendingChanged(pendingCount + self.queuedPendingAlarmCount() - self.lastNumberOfPendingAlarms);
        }

        self.lastNumberOfPendingAlarms = pendingCount + self.queuedPendingAlarmCount();

        return pendingCount + self.queuedPendingAlarmCount();
    });

    // Computed value used to display number of selected alarms to the user
    self.numberOfSelectedAlarms = ko.computed(function () {
        return self.selectedAlarms().length;
    });

    // Should we show the new alarms count or not
    self.showNewAlarmCount = ko.computed(function () {
        return self.queuedNewAlarmCount() > 0;
    });

    // Computed value used to enable/disable the acknowledge all button
    self.isAcknowledgeSelectedEnabled = ko.computed(function () {
        var index;

        for (index = 0; index < self.selectedAlarms().length; index += 1) {
            if (self.selectedAlarms()[index].isPending()) {
                return true;
            }
        }

        return false;
    });

    //Computed value used to enable the more actions button
    self.isMoreActionsEnabled = ko.computed(function () {
        return self.selectedAlarms().length > 0;
    });

    // Computed value used to control whether audio is playing or not
    self.controlAudioPlayback = ko.computed(function () {
        if (self.audioLoaded() && self.numberOfPendingAlarms() > 0) {
            if (!audioPlaying()) {
                playLoopingAudio();
            }
        } else {
            stopLoopingAudio();
        }
    });

    // Behaviors

    // Handle the select all check box being clicked
    self.selectAllClicked = function () {
        setAllAlarmsSelection(self.allSelected());
        return true; // Prevent default handler from getting called
    };

    // Handle the select checkbox click on an alarm row
    self.selectClicked = function (alarm) {
        updateAlarmSelectionTracking(alarm);
        return true; // Prevent default handler from getting called
    };

    // Handle the acknowledge selected button click
    self.acknowledgeSelectedClicked = function () {
        var acknowledgableAlarms = getSelectedPendingAlarms();
        if (acknowledgableAlarms.length > 0) {
            jci.buildingSecurityClient.acknowledgeAlarms(acknowledgableAlarms, onAlarmOperationResponse);
        }

        setAllAlarmsSelection(false);
    };

    // Handle the acknowledge alarm button click from an alarm row
    self.acknowledgeAlarmClicked = function (alarm) {
        jci.buildingSecurityClient.acknowledgeAlarms([alarm.actionParams()], onAlarmOperationResponse);
    };

    // Handle the complete selected button click
    self.completeSelectedClicked = function () {
        var completableAlarms = getSelectedCompletableAlarms();
        if (completableAlarms.length > 0) {
            jci.buildingSecurityClient.completeAlarms(completableAlarms, onAlarmOperationResponse);
        }

        setAllAlarmsSelection(false);
    };

    // Handle the complete alarm button click from an alarm row
    self.completeAlarmClicked = function (alarm) {
        jci.buildingSecurityClient.completeAlarms([alarm.actionParams()], onAlarmOperationResponse);
    };

    // Handle the remove alarm button click from an alarm row
    self.removeAlarmClicked = function (alarm) {
        jci.buildingSecurityClient.completeAlarms([alarm.actionParams()], onRemoveAlarmsResponse);
    };

    // Handle the respond to selected button click
    self.respondToSelectedClicked = function () {
        var respondableAlarms = getSelectedRespondableAlarms();
        if (respondableAlarms.length > 0) {
            self.definedResponse(jci.string.empty());
            self.manualResponse(jci.string.empty());

            $(self.respondDialogSelector).one(modalShownEvent,
                function () { $(self.manualResponseSelector).focus(); });

            $(self.respondDialogSelector).modal(modalShowCommand);
        }
    };

    // Handle the respond to alarm button click from an alarm row
    self.respondToAlarmClicked = function (alarm) {
        jci.buildingSecurityClient.respondToAlarms([alarm.actionParams()], jci.string.trim(alarm.enteredResponse()), onAlarmOperationResponse);
        alarm.enteredResponse(jci.string.empty());
    };

    // Handle the respond button clicked in the response modal
    self.respondConfirmedClicked = function () {
        var alarmResponse = self.definedResponse() || self.manualResponse(),
            respondableAlarms = getSelectedRespondableAlarms();

        if (respondableAlarms.length > 0) {
            jci.buildingSecurityClient.respondToAlarms(respondableAlarms, jci.string.trim(alarmResponse), onAlarmOperationResponse);
        }

        $(self.respondDialogSelector).modal(modalHideCommand);
        setAllAlarmsSelection(false);
    };

    self.onResponseInputKeypress = function (alarm, event) {
        // Get the type-ahead object or default if one doesn't exist
        var typeAhead = $(event.target).data(typeAheadDataKey) || { shown: false };

        if (event.which === jci.keys.enter() && !typeAhead.shown) {
            $(event.target).change();
            if (alarm.isValidResponse()) {
                self.respondToAlarmClicked(alarm);
            }
            return false; // Prevent the default handler
        }

        return true; // Let the default handler execute
    };

    self.detailsShow = function (alarm) {
        alarm.isExpanded(true);
        expandedAlarms.push(alarm.id);
        return true;
    };

    self.detailsHide = function (alarm) {
        alarm.isExpanded(false);
        removeExpandedAlarm(alarm.id);
        return true;
    };

    // Handle the settings button click
    self.settingsClicked = function () {};

    // Handle the sort pending header click
    self.sortByPending = internalSortByPending;

    // Handle the sort occurred date/time header click
    self.sortOccurredDateTimeClicked = function () {
        sortAlarms(jci.alarmManagerColumns.OccurredDateTime, jci.alarmManagerColumns.Priority);
    };

    // Handle the sort priority header click
    self.sortByPriority = function () {
        sortAlarms(jci.alarmManagerColumns.Priority, jci.alarmManagerColumns.OccurredDateTime);
    };

    self.sortByTime = internalSortByTime;

    self.isSortedByTime = ko.computed(function () {
        return sorting().primarySortKey === jci.alarmManagerColumns.OccurredDateTime;
    });

    self.isSortedByPending = ko.computed(function () {
        return sorting().primarySortKey === jci.alarmManagerColumns.Importance;
    });

    // Handle the sort status header click
    self.sortStatusClicked = function () {
        sortAlarms(jci.alarmManagerColumns.Status, jci.alarmManagerColumns.OccurredDateTime);
    };

    // Handle the sort description header click
    self.sortDescriptionClicked = function () {
        sortAlarms(jci.alarmManagerColumns.Description, jci.alarmManagerColumns.OccurredDateTime);
    };

    // Handle the sort updated date/time header click
    self.sortUpdatedDateTimeClicked = function () {
        sortAlarms(jci.alarmManagerColumns.ActionDateTime, jci.alarmManagerColumns.OccurredDateTime);
    };

    function saveTimeZoneChangesSucceeded() {
        self.isLoading(true);
        self.displayedAlarms([]);
        self.activeAlarms([]);
        jci.buildingSecurityClient.getActiveAlarms(onIntialAlarmLoad, null, true);
    }

    function saveTimeZoneChangesFailed() {
        // Fail
    }

    self.timeZoneSelectionChanged = function (unused, event) {
        var selectedTimeZoneId = $(event.target).val();
        $(event.target).closest('.open').removeClass('open');
        jci.buildingSecurityClient.setSelectedTimeZone(selectedTimeZoneId, saveTimeZoneChangesSucceeded, saveTimeZoneChangesFailed);
    };

    self.enableDelayedLoading = function () {
        $(self.alarmListSelector).on(scrollEvent, function () {
            var start, end, scrollPosition, scollElement = $(this);
            scrollPosition = scollElement.scrollTop();
            if (scrollPosition + scollElement.innerHeight() >= (scollElement[0].scrollHeight - (scollElement.innerHeight() / 2))) {
                if (self.activeAlarms().length > self.displayedAlarms().length) {
                    start = self.displayedAlarms().length;
                    end = start + displayAlarmChunkSize;
                    addAlarmsToDisplay(start, end);
                    scollElement.scrollTop(scrollPosition);
                }
            }
        });
    };

    // Start loading some needed data
    jci.buildingSecurityClient.getAlarmResponses(onAlarmResponses);
    jci.buildingSecurityClient.getAlarmDisplayOptions(onDisplayOptions);
    jci.buildingSecurityClient.subscribeToAlarmChannel(onAlarmPushed);
    jci.buildingSecurityClient.subscribeToHeartbeatChannel(onHeartbeatPushed);
    jci.buildingSecurityClient.getActiveAlarms(onIntialAlarmLoad, null, true);
    jci.monitorConnectionStatus();
    // Display the sort direction indicator icon on the default column
    showPrimarySortIndicator();
}
