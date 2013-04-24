/*----------------------------------------------------------------------------

(C) Copyright 2012 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global window: false, $: false, Modernizr: false, ko: false, document: false, logging: false, setInterval: false*/

(function (window) {
    "use strict";

    var jci = (function () {
        var internalRequestType = {
            get: function () { return "GET"; },
            post: function () { return "POST"; }
        },
            internalDataType = {
                json: function () { return "json"; }
            },
            internalContentType = {
                json: function () { return "application/json"; }
            },
            internalCurrectObservableDate = ko.observable(new Date()),
            internalUpdateCurrentDate = function () {
                internalCurrectObservableDate(new Date());
            },
            pushServices = $.connection.pushServices,
            warningThreshold = 30,
            warningNotified = false,
            internalLastContactWithP2000 = ko.observable(new Date()),
            internalIsActiveConnectionRequired = ko.observable(false),
            internalConnectionStatuses = { connected: 0, canNotReachWebServer: 1, canNotReachP2000: 2, noNetworkConnection: 3 },
            internalConnectionStatus = ko.observable(internalConnectionStatuses.connected),

            connectionStatusTrigger = function (state) {
                $(pushServices).trigger(pushServices.events.connectionStateChanged, [state]);
            },

            resetHeartbeatCounter = function () {
                if (internalConnectionStatus() === internalConnectionStatuses.connected) {
                    internalLastContactWithP2000(new Date());
                    warningNotified = false;
                }
            },

            lastContactMonitor = function () {
                var counter = Math.round((new Date().getTime() - internalLastContactWithP2000().getTime()) / 1000);
                if (counter > warningThreshold && !warningNotified) {
                    if (internalConnectionStatus() === internalConnectionStatuses.connected) {
                        warningNotified = true;
                        connectionStatusTrigger(internalConnectionStatuses.canNotReachP2000);
                    }
                }
                window.setTimeout(lastContactMonitor, 1000);
            },

            connectionStateChanged = function (change) {
                if (change.newState === $.signalR.connectionState.connected) {
                    connectionStatusTrigger(internalConnectionStatuses.connected);
                } else {
                    connectionStatusTrigger(internalConnectionStatuses.canNotReachWebServer);
                }
            };
        setInterval(internalUpdateCurrentDate, 60 * 1000); //once a minute update the date

        function endsWith(string, suffix) {
            return string.indexOf(suffix, string.length - suffix.length) !== -1;
        }

        function applicationPath(controller, parameter, alarmsController) {
            var id = parameter || "",
                postfix = alarmsController ? "/" + alarmsController : "",
                apiVersion = "api/v1/",
                pathApiSeperator = endsWith(window.applicationPath, "/") ? "" : "/";
            return window.applicationPath + pathApiSeperator + apiVersion + controller + "/" + id + postfix;
        }

        function handleStandardErrors(errorHandler) {
            return function (xhr, ajaxOptions, thrownError) {
                if (xhr.status === 401) {
                    logging.info("An ajax call resulted in a 401 status code.", xhr);
                    jci.returnToSignin();
                    return;
                }
                if ('undefined' !== typeof errorHandler) {
                    errorHandler(xhr, ajaxOptions, thrownError);
                }
            };
        }

        function ajaxGetJson(url, success, error) {
            $.ajax({
                url: url,
                type: internalRequestType.get(),
                dataType: internalDataType.json(),
                success: success,
                error: handleStandardErrors(error)
            });
        }

        function ajaxPostJson(url, jsonData, success, error) {
            $.ajax({
                url: url,
                type: internalRequestType.post(),
                data: jsonData,
                contentType: internalContentType.json(),
                success: success,
                error: handleStandardErrors(error)
            });
        }

        function getCsrfPreventionToken() {
            return $('#__csrfprevention input[name=__RequestVerificationToken]').val();
        }

        function addCsrfPreventionToken(data) {
            data.__RequestVerificationToken = getCsrfPreventionToken(); //dangling '_' is to prevent naming collisions
            return data;
        }

        function ajaxPost(url, data, success, error) {
            ajaxPostJson(url, JSON.stringify(addCsrfPreventionToken(data)), success, error);
        }

        function internalGetQueryString(obj) {
            var parameters = [], result, property;
            for (property in obj) {
                if (obj[property] !== undefined) {
                    parameters.push(encodeURIComponent(property) + "=" + encodeURIComponent(obj[property]));
                }
            }
            result = parameters.join("&");
            if (result !== '') {
                return '?' + result;
            }
            return result;
        }

        function internalLogOffMonitor() {
            ajaxGetJson(applicationPath("currentuser"), function (username) {
                if (username.toLowerCase() !== window.currentUser && window.currentUser !== "") {
                    jci.returnToSignin();
                    logging.info("Detected change in currently signed in user. Was " + window.currentUser + " now is " + username);
                } else {
                    window.setTimeout(internalLogOffMonitor, 15000);
                }
            }, function () {//on error
                window.setTimeout(internalLogOffMonitor, 15000);
            });
        }

        pushServices.events = { alarmChannelPushed: "alarmChannelPushed", caseChannelPushed: "caseChannelPushed", heartbeatChannelPushed: "heartbeatChannelPushed", connectionStateChanged: "connectionStateChanged" };

        pushServices.pushUpdate = function (channel, update) {
            logging.info('Received update on channel ' + channel);
            $(pushServices).trigger(channel, update);
            resetHeartbeatCounter();
        };

        return {
            isActiveConnectionRequired: internalIsActiveConnectionRequired,

            connectionStatuses: internalConnectionStatuses,

            connectionStatus: internalConnectionStatus,

            lastContactWithP2000: internalLastContactWithP2000,

            currentObservableDate: internalCurrectObservableDate,

            postToApiController: function (controllerName, data) {
                ajaxPost(applicationPath(controllerName), addCsrfPreventionToken(data));
            },

            requestType: internalRequestType,

            dataType: internalDataType,

            contentType: internalContentType,

            getQueryString: internalGetQueryString,

            nonBreakingSpace: function () { return "\u00a0"; },

            string: (function () {
                var stringTrimRegex = /^(\s|\u00A0)+|(\s|\u00A0)+$/g;

                return {
                    empty: function () { return ""; },
                    trim: function (string) {
                        return (string || "").replace(stringTrimRegex, "");
                    }
                };
            }()),

            unDefined: (function () {
                //while this avoids the issue of referencing the undefined object,
                //and the issue of someone replacing undefined with a valued object
                //I (john) believe this just moved the all the issues with the undefined
                //object onto the undefinedValue object. As in if I set window.undefinedValue='3'
                //this will break, and (just like for window.undefined). If there is a reason this
                //is better than just doing comparison against undefined please add a comment
                //so we can avoid having this discussion over and over again.
                function value(undefinedValue) {
                    return undefinedValue;
                }

                return {
                    name: function () { return "undefined"; },
                    value: function () { return value(); }
                };
            }()),

            makeIdSelector: function (id) { return "#" + id; },

            spaceConsumingString: function (value) {
                if (value === null || value === undefined || value === jci.unDefined.value() || value === jci.string.empty()) {
                    return jci.nonBreakingSpace();
                }

                return value.toString();
            },

            isAudioSupported: function () {
                return ((Modernizr.audio) && ((Modernizr.audio.mp3) || (Modernizr.audio.wav)));
            },

            getAudioType: function () {
                var audioType = null;

                if (Modernizr.audio) {
                    if (Modernizr.audio.mp3) {
                        audioType = "mp3";
                    } else if (Modernizr.audio.wav) {
                        audioType = "wav";
                    }
                }
                logging.info(audioType + " was detected as the preferred the supported audio format.");
                return audioType;
            },

            keys: {
                enter: function () { return 13; }
            },

            keyCodes: {
                backspace: function () { return 8; },
                del: function () { return 46; },
                left: function () { return 37; },
                right: function () { return 39; }
            },

            events: {
                beforeUnload: function () { return "beforeunload"; },
                loadeddata: function () { return "loadeddata"; },
                change: function () { return "change"; }
            },

            fadeSpeeds: {
                slow: function () { return "slow"; },
                fast: function () { return "fast"; }
            },

            letterCase: {
                upper: function () { return "uppercase"; },
                lower: function () { return "lowercase"; }
            },

            alertType: {
                success: function () { return "alert-success"; },
                error: function () { return "alert-error"; },
                info: function () { return "alert-info"; }
            },

            monitorConnectionStatus: function () {
                internalIsActiveConnectionRequired(true);
                logging.info("Connection status monitor activated.");
                //signlr connection events
                jci.buildingSecurityClient.subscribeToConnectionChangeEvent(function (state) {
                    if (state !== internalConnectionStatuses.connected) {//never go back to a connected state without a refresh
                        logging.info("Connection state has changed.", state);
                        internalConnectionStatus(state);
                    }
                });

                // Start the function that monitors time elapsed since last P2000 contact.
                lastContactMonitor();

                //html5 browser network events
                document.body.onoffline = function () {
                    connectionStatusTrigger(internalConnectionStatuses.noNetworkConnection);
                };
            },

            alarmManagerColumns: {
                Pending: 1,
                OccurredDateTime: 2,
                Priority: 3,
                Status: 4,
                Description: 5,
                ActionDateTime: 6,
                Importance: 7
            },

            message: (function () {
                function getMessageHtml(messageTypeClass, messageText) {
                    return "<div class='alert " + messageTypeClass + "'>" + messageText + "</div>";
                }

                function fadeInMessageContainer(messageContainerId, messageHtml) {
                    var $elem = $(jci.makeIdSelector(messageContainerId));

                    $elem.hide();
                    $elem.empty();
                    $elem.append(messageHtml);
                    $elem.fadeIn(jci.fadeSpeeds.slow());
                }

                function fadeOutMessageContainer(messageContainerId) {
                    var $elem = $(jci.makeIdSelector(messageContainerId));

                    $elem.fadeOut(jci.fadeSpeeds.slow());
                }

                return {
                    /**
                    * Fade in a message container with the specified messageText
                    * @param {string} messageContainerId : id of the container div (<div id="message" class="row" style="height: 70px; margin-left:0px"></div>)
                    * @param {string} messageText : Text to be included within the container
                    * @param {string} messageType : CSS class to be used for formatting ("success" or "error")
                    */
                    display: function (messageContainerId, messageText, messageType) {
                        if ((messageText === null) || (messageText === "")) {
                            return;
                        }

                        var messageTypeClass = jci.alertType[messageType] !== undefined ? jci.alertType[messageType]() : jci.alertType.info(),
                            messageHtml = getMessageHtml(messageTypeClass, messageText);

                        fadeInMessageContainer(messageContainerId, messageHtml);
                    },

                    hide: function (messageContainerId) {
                        fadeOutMessageContainer(messageContainerId);
                    }
                };
            }()),

            buildingSecurityClient: (function () {
                var hubStartCalled = false,
                    pushServicesStarted = false,
                    isAlarmChannelSubscribed = false,
                    isCaseChannelSubscribed = false,
                    isHeartbeatChannelSubscribed = false,
                    isConnectionStatusChangeChannelSubscribed = false;

                function executeWhenPushServiceStart(onStartCallback) {
                    $($.connection.hub).on("onStart", onStartCallback);
                }

                function startPushServices(onStartCallback) {
                    if (!hubStartCalled) {
                        hubStartCalled = true;
                        $.connection.hub.start({ transport: 'longPolling' }, function () {
                            onStartCallback.call($.connection.hub);
                            pushServicesStarted = true;
                        });
                        $.connection.hub.stateChanged(connectionStateChanged);
                    } else {
                        executeWhenPushServiceStart(onStartCallback);
                    }
                }

                function addPushServiceEventCallback(event, callback) {
                    if ($.type(callback) === "function") {
                        $(pushServices).bind(event, function (unused, data) {
                            callback.call(this, data);
                        });
                    }
                }

                function addPushServiceConnectionEventCallback(event, callback) {
                    if ($.type(callback) === "function") {
                        $(pushServices).bind(event, function (unused, state, status) {
                            callback.call(this, state, status);
                        });
                    }
                }

                function addAlarmChannelPushedEventCallback(callback) {
                    addPushServiceEventCallback(pushServices.events.alarmChannelPushed, callback);
                }

                function addHeartbeatChannelPushedEventCallback(callback) {
                    addPushServiceEventCallback(pushServices.events.heartbeatChannelPushed, callback);
                }

                function addCaseChannelPushedEventCallback(callback) {
                    addPushServiceConnectionEventCallback(pushServices.events.caseChannelPushed, callback);
                }

                function addConnectionStateChangedEventCallback(callback) {
                    addPushServiceConnectionEventCallback(pushServices.events.connectionStateChanged, callback);
                }

                return {
                    logOffMonitor: function () {
                        logging.info("Log off monitor activated.");
                        internalLogOffMonitor();
                    },

                    getActiveAlarms: function (callback, after, sorted) {
                        var queryString = internalGetQueryString({ "after": after, "sorted": sorted });
                        ajaxGetJson(applicationPath("alarms") + queryString, callback);
                    },

                    getAlarmHistory: function (id, callback) {
                        ajaxGetJson(applicationPath("alarms", id, "history"), callback);
                    },

                    getAlarmResponses: function (callback) {
                        ajaxGetJson(applicationPath("alarmresponses"), callback);
                    },

                    acknowledgeAlarms: function (alarms, callback) {
                        ajaxPost(applicationPath("alarms"), { 'action': "ACKNOWLEDGE", 'alarms': alarms }, callback);
                    },

                    respondToAlarms: function (alarms, response, callback) {
                        ajaxPost(applicationPath("alarms"), { 'action': "RESPOND", 'alarms': alarms, 'response': response }, callback);
                    },

                    completeAlarms: function (alarms, callback) {
                        ajaxPost(applicationPath("alarms"), { 'action': "COMPLETE", 'alarms': alarms }, callback);
                    },

                    getAudioAlerts: function (callback) {
                        var audioType = jci.getAudioType();

                        if (audioType !== null) {
                            ajaxGetJson(applicationPath("audioalerts", audioType), callback);
                        }
                    },

                    getAudioAlertsAlways: function (callback) {
                        var audioType = jci.getAudioType();
                        if (audioType === null) {
                            audioType = "mp3";
                        }

                        ajaxGetJson(applicationPath("audioalerts", audioType), callback);
                    },

                    getAlarmDisplayOptions: function (onSuccess, onError) {
                        ajaxGetJson(applicationPath("alarmdisplayoptions"), onSuccess, onError);
                    },

                    getDefaultAlarmDisplayOptions: function (onSuccess, onError) {
                        ajaxGetJson(applicationPath("alarmdisplayoptions", "default"), onSuccess, onError);
                    },

                    saveAlarmDisplayOptions: function (alarmDisplayOptions, onSuccess, onError) {
                        ajaxPost(applicationPath("alarmdisplayoptions"), { "DisplayRanges": alarmDisplayOptions }, onSuccess, onError);
                    },

                    getSelectedTimeZone: function (onSuccess, onError) {
                        ajaxGetJson(applicationPath("userpreferences"), onSuccess, onError);
                    },

                    setSelectedTimeZone: function (selectedTimeZoneId, onSuccess, onError) {
                        ajaxPost(applicationPath("userpreferences"), { preferenceKey: "SelectedTimeZone", preferenceValue: selectedTimeZoneId }, onSuccess, onError);
                    },

                    subscribeToAlarmChannel: function (callback) {
                        logging.info("Alarm channel subscribed to.");
                        if (!isAlarmChannelSubscribed) {
                            isAlarmChannelSubscribed = true;

                            if (!pushServicesStarted) {
                                startPushServices(function () { pushServices.subscribeToChannel(pushServices.events.alarmChannelPushed); });
                            } else {
                                pushServices.subscribeToChannel(pushServices.events.alarmChannelPushed);
                            }
                        }

                        addAlarmChannelPushedEventCallback(callback);
                    },

                    subscribeToCaseChannel: function (callback) {
                        logging.info("Case channel subscribed to.");
                        if (!isCaseChannelSubscribed) {
                            isCaseChannelSubscribed = true;

                            if (!pushServicesStarted) {
                                startPushServices(function () { pushServices.subscribeToChannel(pushServices.events.caseChannelPushed); });
                            } else {
                                pushServices.subscribeToChannel(pushServices.events.caseChannelPushed);
                            }
                        }

                        addCaseChannelPushedEventCallback(callback);
                    },

                    subscribeToHeartbeatChannel: function (callback) {
                        logging.info("Heartbeat channel subscribed to.");
                        if (!isHeartbeatChannelSubscribed) {
                            isHeartbeatChannelSubscribed = true;

                            if (!pushServicesStarted) {
                                startPushServices(function () { pushServices.subscribeToChannel(pushServices.events.heartbeatChannelPushed); });
                            } else {
                                pushServices.subscribeToChannel(pushServices.events.heartbeatChannelPushed);
                            }
                        }

                        addHeartbeatChannelPushedEventCallback(callback);
                    },

                    subscribeToConnectionChangeEvent: function (callback) {
                        logging.info("Connection state channel subscribed to.");
                        if (!isConnectionStatusChangeChannelSubscribed) {
                            isConnectionStatusChangeChannelSubscribed = true;
                        }

                        addConnectionStateChangedEventCallback(callback);
                    },

                    unsubscribeFromAllChannels: function () {
                        logging.info("Unsubscribing from all channels.");
                        if (isAlarmChannelSubscribed) {
                            pushServices.unsubscribeFromChannel(pushServices.events.alarmChannelPushed);
                        }

                        if (isCaseChannelSubscribed) {
                            pushServices.unsubscribeFromChannel(pushServices.events.caseChannelPushed);
                        }

                        if (isHeartbeatChannelSubscribed) {
                            pushServices.unsubscribeFromChannel(pushServices.events.heartbeatChannelPushed);
                        }

                        if (pushServicesStarted) {
                            $.connection.hub.stop();
                        }
                    },

                    getDependentParameterValues: function (reportId, dataSource, parameterValues, onSuccess, onError) {
                        ajaxPost(applicationPath("reportparameters"), { 'reportId': reportId, 'dataSource': dataSource, 'parameterValues': parameterValues }, onSuccess, onError);
                    }
                };
            }()),

            localization: (function () {
                return {
                    // function attempts to display date picker for the
                    // requested locale.  If the requested locale
                    // is not found in the jquery-ui-i18n.js then
                    // '' is returned to display date picker in English.
                    getJqueryUserLanguage: function (languageCode) {
                        var l = languageCode.toLowerCase().split('-');
                        if (l.length === 1) {
                            if ($.datepicker.regional[l[0]] !== jci.unDefined.value()) {
                                return l[0];
                            }
                        } else if (l.length > 1) {
                            if ($.datepicker.regional[l[0] + '-' + l[1].toUpperCase()] !== jci.unDefined.value()) {
                                return l[0] + '-' + l[1].toUpperCase();
                            }
                            if ($.datepicker.regional[l[0]] !== jci.unDefined.value()) {
                                return l[0];
                            }
                        }

                        return '';
                    }
                };
            }()),

            direction: (function () {
                return {
                    Ascending: 1,
                    None: 0,
                    Descending: -1
                };
            }())
        };
    }());

    // Expose jci to the global object
    window.jci = jci;
}(window));
