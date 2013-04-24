/*----------------------------------------------------------------------------

(C) Copyright 2012 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global ko: false, $: false, jci: false, HistoryEntry: false */

//
// This is the Alarm view model. It is the view model for each row in the active alarm list.
//
function Alarm(serializedAlarm, settings) {
    "use strict";

    var self = this;

    // Private methods
    function alarmHistoryRecieved(historyEntries) {
        var entry, deleteCount = 1, takeIndex = 0, index;

        // If the list of entries is empty do nothing.
        if (historyEntries.length === 0) {
            return;
        }

        // Only merge if it's likely to help performance
        if (self.historyEntries().length > 15 ||
                self.historyEntries().length > (historyEntries.length / 2)) {

            // How many entries have the latest timestamp
            entry = self.historyEntries()[0];
            while (deleteCount < self.historyEntries().length &&
                    entry.sortableTimestamp === self.historyEntries()[deleteCount].sortableTimestamp) {
                deleteCount += 1;
            }

            // Skip all the entries until we get to the latest timestamp
            historyEntries.sort(function (a, b) { return a.sortableTimestamp.localeCompare(b.sortableTimestamp); });
            while (takeIndex < historyEntries.length &&
                    entry.sortableTimestamp.localeCompare(historyEntries[takeIndex].sortableTimestamp) > 0) {
                takeIndex += 1;
            }
            historyEntries = historyEntries.slice(takeIndex);

            // Merge the new items if there are any
            if (deleteCount !== historyEntries.length) {
                self.historyEntries.splice(0, deleteCount);
                while (historyEntries.length >= 10) {
                    self.historyEntries.unshift(new HistoryEntry(historyEntries[9]), new HistoryEntry(historyEntries[8]),
                        new HistoryEntry(historyEntries[7]), new HistoryEntry(historyEntries[6]), new HistoryEntry(historyEntries[5]),
                        new HistoryEntry(historyEntries[4]), new HistoryEntry(historyEntries[3]), new HistoryEntry(historyEntries[2]),
                        new HistoryEntry(historyEntries[1]), new HistoryEntry(historyEntries[0]));
                    historyEntries.splice(0, 10);
                }
                for (index = 0; index < historyEntries.length; index += 1) {
                    self.historyEntries.unshift(new HistoryEntry(historyEntries[index]));
                }
            }
        } else {
            // Otherwise just update with updated list
            for (index = 0; index < historyEntries.length; index += 1) {
                historyEntries[index] = new HistoryEntry(historyEntries[index]);
            }

            self.historyEntries(historyEntries.sort(function (a, b) {
                return b.sortableTimestamp.localeCompare(a.sortableTimestamp);
            }));
        }
    }

    function updateHistory() {
        jci.buildingSecurityClient.getAlarmHistory(self.id, alarmHistoryRecieved);
    }

    // Data
    // Bound to the id of the details collapsible panel, also the id needed for calls to manipulate this alarm
    self.id = serializedAlarm.Id;

    // composite id's used for various elements displaying the alarm
    self.detailsId = self.id + "-details";
    self.detailsIdSelector = jci.makeIdSelector(self.detailsId);
    self.responseTextId = self.id + "-responseText";
    self.responseTextIdSelector = jci.makeIdSelector(self.responseTextId);
    self.enterResponseButtonId = self.id + "-enterResponseButton";
    self.enterResponseButtonIdSelector = jci.makeIdSelector(self.enterResponseButtonId);

    // Bound to the history list
    self.historyEntries = ko.observableArray();

    // Bound to the selected checkbox
    self.selected = ko.observable(false);

    // The date/time the alarm occurred - bound to a column - this is not observable - only set on creation, then static
    self.messageDateString = ko.observable(jci.spaceConsumingString(serializedAlarm.MessageDateString));
    self.messageTimeString = ko.observable(jci.spaceConsumingString(serializedAlarm.MessageTimeString));
    self.messageDateTimeString = ko.observable(jci.spaceConsumingString(serializedAlarm.MessageDateTimeString));
    self.messageUtcDateTime = ko.observable(serializedAlarm.MessageUtcDateTime);
    self.messageDisplayDateTimeString = ko.computed({
        read: function () {
            var dayOfAlarm;
            dayOfAlarm = new Date(self.messageUtcDateTime());
            if ((jci.currentObservableDate().getTime() - dayOfAlarm.getTime()) > (23 * 60 * 60 * 1000)) {//if the alarm is over 23hrs old
                return self.messageDateString();
            }
            return self.messageTimeString();
        },
        deferEvaluation: true
    });

    // Not bound, just used to sort based on occurred date time
    self.messageDateTimeSortableString = ko.observable(serializedAlarm.MessageDateTimeSortableString);

    // The priority of the alarm - bound to a column - this is not observable - only set on creation, then static
    self.priority = jci.spaceConsumingString(serializedAlarm.Priority);

    // The priority color of the alarm - bound to a style - this should be observable (though right now it is static) - should change with user preferences updates
    self.priorityColor = ko.observable(jci.alarmDisplayOptions.getPriorityColor(self.priority));
    jci.alarmDisplayOptions.updateCount.subscribe(function () {
        self.priorityColor(jci.alarmDisplayOptions.getPriorityColor(self.priority));
    });

    // The status of the alarm - bound to a column - this is observable and changes as the alarm is managed
    self.alarmStateDescription = ko.observable(jci.spaceConsumingString(serializedAlarm.AlarmStateDescription));

    // Display the alarm state description in the alarm item header.
    self.alarmStateDescriptionDispaly = ko.computed(function () {
        return " (" + this.alarmStateDescription() + ")";
    }, self);

    // The Source(description) of the alarm - bound to a column - this is observable and may change as the alarm is managed by the P2000
    self.description = ko.observable(jci.spaceConsumingString(serializedAlarm.Description));

    // The date/time of the last action taken on the alarm - bound to a column - this is observable and changes as the alarm is managed
    self.stateDateString = ko.observable(jci.spaceConsumingString(serializedAlarm.StateDateString));
    self.stateTimeString = ko.observable(jci.spaceConsumingString(serializedAlarm.StateTimeString));
    self.stateDateTimeString = ko.observable(jci.spaceConsumingString(serializedAlarm.StateDateTimeString));

    // Not bound, just used to sort based on action date timeThe date/time of the last action taken on the alarm
    self.stateDateTimeSortableString = ko.observable(serializedAlarm.StateDateTimeSortableString);

    // Incremented every time the triggering item changes state
    self.conditionSequence = ko.observable(serializedAlarm.ConditionSequence);

    // The site of the alarm - bound to the alarm details - this is not observable - only set on creation, then static
    self.site = jci.spaceConsumingString(serializedAlarm.Site);

    // Is the alarm public - bound to the alarm details - this is not observable - only set on creation, then static
    self.isPublicDescription = jci.spaceConsumingString(serializedAlarm.IsPublicDescription);

    // The category of the alarm - bound to the alarm details - this is not observable - only set on creation, then static
    self.category = jci.spaceConsumingString(serializedAlarm.Category);

    // The description of the current state of the alarm - bound to the alarm details - this is observable and changes as the alarm is managed
    self.sourceState = ko.observable(serializedAlarm.SourceState);
    self.sourceStateDescription = ko.observable(jci.spaceConsumingString(serializedAlarm.SourceStateDescription));

    self.isAlarmStateNA = ko.computed(function () {
        return self.sourceState() === -1;
    });

    self.isAlarmStateSecure = ko.computed(function () {
        return self.sourceState() === 0;
    });

    self.isAlarmStateAlarm = ko.computed(function () {
        return self.sourceState() === 1;
    });

    self.isAlarmStateShort = ko.computed(function () {
        return self.sourceState() === 2;
    });

    self.isAlarmStateOpen = ko.computed(function () {
        return self.sourceState() === 3;
    });

    self.isAlarmStateSuppressed = ko.computed(function () {
        return self.sourceState() === 4;
    });

    self.isAlarmStateUnknown = ko.computed(function () {
        return (!self.isAlarmStateNA() &&
            !self.isAlarmStateSecure() &&
                !self.isAlarmStateAlarm() &&
                    !self.isAlarmStateShort() &&
                        !self.isAlarmStateOpen() &&
                            !self.isAlarmStateSuppressed());
    });

    // The escalation value of the alarm - bound to the alarm details - this is observable and changes as the alarm is managed
    self.escalation = ko.observable(jci.spaceConsumingString(serializedAlarm.Escalation));

    // The instructions for the alarm - bound to the alarm details - this is not observable - only set on creation, then static
    self.instructions = serializedAlarm.Instructions;

    // Bound to the response text box in the alarm details
    self.enteredResponse = ko.observable(jci.string.empty());
    self.enteredResponse.subscribe(function (newValue) {
        self.isValidResponse(jci.string.trim(newValue) !== jci.string.empty());
    });

    self.isValidResponse = ko.observable(false);

    // Holds the error that may result from operations taken on the alarm
    self.error = ko.observable(jci.string.empty());

    // Computed value used to control visibility of the error block
    self.isErrorAvailable = ko.computed(function () {
        return (self.error() !== jci.string.empty());
    });

    self.isInstructionAvailable = ko.computed(function () {
        return (self.instructions !== jci.string.empty());
    });

    self.isEscalationVisible = ko.computed(function () {
        return ((+(self.escalation())) > 0);
    });

    // True if the Alarm is in a Pending state, otherwise false
    self.isPending = ko.observable(serializedAlarm.IsPending);

    // True if the Alarm can be Completed, otherwise false
    self.isCompletable = ko.observable(serializedAlarm.IsCompletable);

    // True if one can Respond to the Alarm, otherwise false
    self.isRespondable = ko.observable(serializedAlarm.IsRespondable);

    // True if the Alarm can be Removed, otherwise false
    self.isRemovable = ko.observable(serializedAlarm.IsRemovable);

    // True if the Alarm can be completed, otherwise false
    self.isCompleted = ko.observable(serializedAlarm.IsCompleted);

    self.isAcknowledgeVisible = ko.computed(function () {
        return self.isPending();
    });

    self.isCompleteVisible = ko.computed(function () {
        return !self.isAcknowledgeVisible() && !self.isCompleted();
    });

    self.isRemoveVisible = ko.computed(function () {
        return !self.isCompleteVisible() && !self.isAcknowledgeVisible();
    });

    // -1=Don't care, 0=Collapsed, 1=Expanded
    self.isExpandedValue = ko.observable(settings.autoExpand ? -1 : 0);

    // used to indicate whether we are expanded, bound to a css class - this is observable and changes as the alarm is managed
    self.isExpanded = ko.computed({
        read: function () {
            if (self.isExpandedValue() === 1) {
                return true;
            }

            if (self.isExpandedValue() === 0) {
                return false;
            }

            // To expand by default when State = Alarm: || (self.sourceState() === 1)
            return (self.isPending());
        },
        write: function (newValue) {
            self.isExpandedValue(newValue ? 1 : 0);
        }
    });

    if (self.isExpanded()) {
        updateHistory();
    }

    self.isExpanded.subscribe(function (newValue) {
        if (newValue) {
            updateHistory();
        }
    });

    // Return value of the specified sortKey formatted as a sortable type
    self.getSortableValue = function (sortKey) {
        var value;

        switch (sortKey) {
        case jci.alarmManagerColumns.Pending:
            return self.isPending() ? 1 : 0;
        case jci.alarmManagerColumns.OccurredDateTime:
            return self.messageDateTimeSortableString();
        case jci.alarmManagerColumns.Priority:
            return +self.priority;
        case jci.alarmManagerColumns.Status:
            return self.alarmStateDescription();
        case jci.alarmManagerColumns.Description:
            return self.description.toLowerCase();
        case jci.alarmManagerColumns.ActionDateTime:
            return self.stateDateTimeSortableString();
        case jci.alarmManagerColumns.Importance:
            // TODO: Need to support an arbitrary list of sort-able values
            value = 0;

            value += self.isPending() ? 10 : 0;
            value += self.isAlarmStateAlarm() ? 1 : 0;

            return value;
        }

        return null;
    };

    self.isAlarmCollapsed = ko.computed(function () {
        return self.isCompleteVisible() && !(self.isCompletable());
    });

    // Handle the details being shown
    self.detailsShown = function () {
        // Works in all browsers with double call - doesn't with a single
        $(self.responseTextIdSelector).focus().focus();
    };

    // Determine the background color depending on whether the alarm is pending.
    self.priorityColorBackground = ko.computed(function () {
        return this.isPending() ? this.priorityColor() : 'none';
    }, self);
    
    // Determine the highest pending status and alarm state.
    self.isHighStatusState = ko.computed(function () {
        return this.isAlarmStateAlarm() && this.isPending();
    }, self);

    // Determine if the current priority level is 'highest' and return appropriate visibility style value.
    self.setHighStatusStateVisible = ko.computed(function () {
        return this.isHighStatusState() === true ? "visible" : "hidden";
    }, self);

    // Determine the background color depending on whether the alarm is pending.
    self.highStatusStateColorBackground = ko.computed(function () {
        return this.isHighStatusState() ? this.priorityColor() : 'none';
    }, self);
    
    // Helpers - are not data bound to anything

    self.actionParams = function () {
        return { 'alarmId': self.id, 'conditionSequence': self.conditionSequence() };
    };

    self.update = function (alarmUpdates) {
        var propertyName;
        for (propertyName in alarmUpdates) {
            if (alarmUpdates.hasOwnProperty(propertyName) && ko.isObservable(self[propertyName])) {
                self[propertyName](alarmUpdates[propertyName]);
            }
        }
    };

    self.processChanges = function (alarmChanges) {
        self.alarmStateDescription(jci.spaceConsumingString(alarmChanges.AlarmStateDescription));
        self.description(jci.spaceConsumingString(alarmChanges.Description));
        self.messageDateString(jci.spaceConsumingString(alarmChanges.MessageDateString));
        self.messageTimeString(jci.spaceConsumingString(alarmChanges.MessageTimeString));
        self.messageDateTimeString(jci.spaceConsumingString(alarmChanges.MessageDateTimeString));
        self.messageUtcDateTime(alarmChanges.MessageUtcDateTime);
        self.messageDateTimeSortableString(alarmChanges.MessageDateTimeSortableString);
        self.stateDateString(jci.spaceConsumingString(alarmChanges.StateDateString));
        self.stateTimeString(jci.spaceConsumingString(alarmChanges.StateTimeString));
        self.stateDateTimeString(jci.spaceConsumingString(alarmChanges.StateDateTimeString));
        self.stateDateTimeSortableString(alarmChanges.StateDateTimeSortableString);
        self.sourceState(alarmChanges.SourceState);
        self.sourceStateDescription(jci.spaceConsumingString(alarmChanges.SourceStateDescription));
        self.escalation(jci.spaceConsumingString(alarmChanges.Escalation));
        self.conditionSequence(alarmChanges.ConditionSequence);
        self.isPending(alarmChanges.IsPending);
        self.isCompletable(alarmChanges.IsCompletable);
        self.isRespondable(alarmChanges.IsRespondable);
        self.isRemovable(alarmChanges.IsRemovable);
        self.isCompleted(alarmChanges.IsCompleted);

        if (self.isExpanded()) {
            updateHistory();
        }
    };
}
