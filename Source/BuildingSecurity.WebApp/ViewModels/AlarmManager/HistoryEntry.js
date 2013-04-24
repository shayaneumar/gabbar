/*----------------------------------------------------------------------------

(C) Copyright 2012 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global jci: false */

//
// This is the Alarm view model. It is the view model for each row in the active alarm list.
//
function HistoryEntry(serializedEntry) {
    "use strict";

    // Private methods

    // The id of the entry
    this.id = serializedEntry.Id;

    // The timestamp of the action.
    this.timestamp = jci.spaceConsumingString(serializedEntry.timestamp);

    // The timestamp of the action in a sortable format.
    this.sortableTimestamp = serializedEntry.sortableTimestamp;

    // The operator who took the action.
    this.operatorName = jci.spaceConsumingString(serializedEntry.operatorName);

    // The status of the parent alarm after this action.
    this.alarmStatus = jci.spaceConsumingString(serializedEntry.alarmStatus);

    // The state of the parent alarm after this action.
    this.alarmState = jci.spaceConsumingString(serializedEntry.alarmState);

    // The response that the operator entered as part of a respond action. Other actions are not expected to have a response.
    this.response = jci.spaceConsumingString(serializedEntry.response);
}