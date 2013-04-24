/*----------------------------------------------------------------------------

(C) Copyright 2013 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global define: false*/

define('noteDetailsViewModelFactory', ['ko', 'rivets', 'jquery'],
    function (ko, rivets, $) {
        "use strict";

        return function (caseNote) {
            this.closeDetailsClick = function () {
                //Knowledge of the page layout is a bad thing for a view model to have.
                //This is a flawed implementation.
                $('#main-container').addClass('full-width');
                rivets.clear('content-details');
            };

            this.text = ko.observable(caseNote.text());
            this.createdBy = ko.observable(caseNote.createdBy());
            this.timestampDateTimeString = ko.observable(caseNote.timestampDateTimeString());
        };
    });