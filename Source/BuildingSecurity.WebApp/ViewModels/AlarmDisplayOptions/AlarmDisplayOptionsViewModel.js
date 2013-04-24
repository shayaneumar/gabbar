/*----------------------------------------------------------------------------

(C) Copyright 2012 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global window: false, $: false, jci: false, ko: false, AlarmDisplayRange: false, document: false, logging:false*/

var AlarmDisplayOptionsViewModel = function (errorMessages) {
    "use strict";

    var self = this,
        colorPickerClassSelector = ".color-picker",
        audioLoadedCount;

    function getAlert(alertType, message) {
        return "<div class='alert " + alertType + "'>" + message + "</div>";
    }

    function fadeInAlert(alert) {
        logging.info("Showing alert.", alert);
        var $elem = $(self.messageSelector);

        $elem.hide();
        $elem.empty();
        $elem.append(alert);
        $elem.fadeIn(jci.fadeSpeeds.slow());
    }

    function fadeOutAlert() {
        $(self.messageSelector).fadeOut(jci.fadeSpeeds.fast(), function () {
            var $elem = $(this);
            $elem.empty();
            $elem.show();
        });
    }

    function saveChangesSucceeded(message) {
        logging.info("Save succeeded");
        fadeInAlert(getAlert(jci.alertType.success(), message));
        self.changeMade(false);
        self.operationInProgress(false);
    }

    function saveChangesFailed() {
        logging.error("Save failed.");
        fadeInAlert(getAlert(jci.alertType.error(), errorMessages.saveFailedMessage));
        self.operationInProgress(false);
    }

    function audioAlertForId(audioAlertId) {
        return ko.utils.arrayFirst(self.availableAudioAlerts(), function (audioAlert) { return audioAlert.id === audioAlertId; });
    }

    function valueChanged() {
        self.changeMade(true);
    }

    function onAlarmDisplayOptions(alarmDisplayRanges) {
        logging.info("Received alarm display options.", alarmDisplayRanges);
        var index, currentSerializedRange, currentRange, prevRange, isUpperFixed, list = [];

        alarmDisplayRanges.sort(function (a, b) {
            return a.id - b.id;
        });

        for (index = 0; index < alarmDisplayRanges.length; index += 1) {
            prevRange = index > 0 ? list[index - 1] : jci.unDefined.value();
            currentSerializedRange = alarmDisplayRanges[index];
            isUpperFixed = (index === (alarmDisplayRanges.length - 1));
            currentRange = new AlarmDisplayRange(currentSerializedRange, prevRange, index, 255 - (alarmDisplayRanges.length - (index + 1)), isUpperFixed);

            currentRange.lowerLimit.subscribe(valueChanged);
            currentRange.color.subscribe(valueChanged);
            currentRange.audioAlertId.subscribe(valueChanged);

            list.push(currentRange);
        }

        self.displayRanges(list);
        self.changeMade(false);
        self.operationInProgress(false);

        $(colorPickerClassSelector).limitedColorPicker(function () {
            $(this).trigger(jci.events.change());
        });

        $("#displayOptions").applyPolyfills();
    }

    function getAlarmDisplayOptionsFailed() {
        logging.error("Failed to load alarm display options.");
        fadeInAlert(getAlert(jci.alertType.error(), errorMessages.loadFailedMessage));
        self.operationInProgress(false);
    }

    function onAudioLoaded() {
        audioLoadedCount += 1;

        if (audioLoadedCount === self.availableAudioAlerts().length) {
            self.audioLoaded(true);
        }
    }

    function onAudioAlerts(currentAudioAlerts) {
        var index, audioAlert;
        logging.info("Received audio alert list.", currentAudioAlerts);
        audioLoadedCount = 0;
        if (jci.isAudioSupported()) {
            for (index = 0; index < currentAudioAlerts.length; index += 1) {
                audioAlert = currentAudioAlerts[index];
                audioAlert.audio = new window.Audio(audioAlert.source);
                $(audioAlert.audio).on(jci.events.loadeddata(), onAudioLoaded);
            }
        }

        self.availableAudioAlerts(currentAudioAlerts);
        jci.buildingSecurityClient.getAlarmDisplayOptions(onAlarmDisplayOptions, getAlarmDisplayOptionsFailed);
    }

    self.messageId = "message";
    self.messageSelector = jci.makeIdSelector(self.messageId);

    self.changeMade = ko.observable(false);
    self.changeMade.subscribe(function (newValue) {
        if (newValue) {
            fadeOutAlert();
        }
    });

    self.operationInProgress = ko.observable(false);

    self.availableAudioAlerts = ko.observableArray();

    self.audioLoaded = ko.observable(false);

    self.displayRanges = ko.observableArray();

    self.buttonsEnabled = ko.computed(function () {
        return self.changeMade() && !self.operationInProgress();
    });

    self.playSelectedAudio = function (alarmDisplayRange) {
        logging.info("Playing audio.", alarmDisplayRange);
        var index, selecteAudioAlert;
        for (index = 0; index < self.availableAudioAlerts().length; index += 1) {
            self.availableAudioAlerts()[index].audio.pause();
        }

        selecteAudioAlert = audioAlertForId(alarmDisplayRange.audioAlertId());
        selecteAudioAlert.audio.currentTime = 0;
        selecteAudioAlert.audio.play();
    };

    self.resetPreferences = function () {
        logging.info("Resetting preferences.");
        self.operationInProgress(true);
        jci.buildingSecurityClient.getAlarmDisplayOptions(onAlarmDisplayOptions, getAlarmDisplayOptionsFailed);
    };

    self.defaultPreferences = function () {
        logging.info("Resetting preferences to default values.");
        self.operationInProgress(true);
        jci.buildingSecurityClient.getDefaultAlarmDisplayOptions(function (x) {
            onAlarmDisplayOptions(x);
            self.changeMade(true);
        }, getAlarmDisplayOptionsFailed);
    };

    self.submitChanges = function () {
        logging.info("Attempting to save changes.");
        var index, postData = [];
        if (self.changeMade()) {
            for (index = 0; index < self.displayRanges().length; index += 1) {
                postData.push(self.displayRanges()[index].postData());
            }

            self.operationInProgress(true);
            jci.buildingSecurityClient.saveAlarmDisplayOptions(postData, saveChangesSucceeded, saveChangesFailed);
        }

        return false;
    };

    // Load the data needed by the page
    jci.buildingSecurityClient.getAudioAlertsAlways(onAudioAlerts);
};

$(document).ready(function () {
    "use strict";

    var viewModel = new AlarmDisplayOptionsViewModel(window.errorMessages);
    ko.applyBindings(viewModel);
});