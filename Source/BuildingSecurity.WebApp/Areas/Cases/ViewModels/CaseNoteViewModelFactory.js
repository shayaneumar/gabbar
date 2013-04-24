/*----------------------------------------------------------------------------

(C) Copyright 2013 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global define: false*/

define('caseNoteViewModelFactory', ['ko', 'rivets', 'jci', 'jquery'],
    function (ko, rivets, jci, $) {
        "use strict";

        return function (caseNote) {
            this.text = ko.observable(caseNote.Text);
            this.textAsSingleLine = ko.observable(caseNote.Text.replace(/(\r\n|\n|\r)/gm, " ").replace(/\s+/g, " "));
            this.createdBy = ko.observable(caseNote.CreatedBy);
            this.timestampUtcMilliseconds = ko.observable(caseNote.TimestampUtcMilliseconds);
            this.timestampDateTimeString = ko.observable(caseNote.TimestampDateTimeString);
            this.timestampDisplay = ko.computed({
                read: function () {
                    var dayOfAlarm;
                    dayOfAlarm = new Date(caseNote.TimestampUtcMilliseconds);
                    if ((jci.currentObservableDate().getTime() - dayOfAlarm.getTime()) > (23 * 60 * 60 * 1000)) {//if the alarm is over 23hrs old
                        return caseNote.TimestampDateTimeString;
                    }
                    return caseNote.TimestampTimeString;
                },
                deferEvaluation: true
            });

            this.viewCaseNoteClick = function () {
                // TODO: Change to use rivets.softLink with a URL including the caseNote Guid
                $('#main-container').removeClass('full-width');
                rivets.show('content-details', 'note details', [this]);
            };
        };
    });