/*----------------------------------------------------------------------------

(C) Copyright 2012 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global window: false, console: false*/

/// <summary>
/// Class provides methods to append text, with icons (depending on the browser) in the browser Developer Tools Console
/// </summary>
window.logging = (new function () { //The new is needed to get this to point to the right thing.
    "use strict";
    var self = this,
        getTime = function () {
            var timestamp = new Date(),
                formatted = timestamp.toLocaleTimeString();
            return formatted;
        },
        safeConsole = (function () {
            return {
                log: function (message, args) {
                    try {
                        if (args) {// This is a hack, to work around IE9s lack of support for console.log.call() or console.log.apply()
                            console.log(message, args);
                        } else {
                            console.log(message);
                        }
                    } catch (e) {
                    }
                },
                info: function (message, args) {
                    try {
                        if (args) {
                            console.info(message, args);
                        } else {
                            console.info(message);
                        }
                    } catch (e) {
                    }
                },
                warn: function (message, args) {
                    try {
                        if (args) {
                            console.warn(message, args);
                        } else {
                            console.warn(message);
                        }
                    } catch (e) {
                    }
                },
                error: function (message, args) {
                    try {
                        if (args) {
                            console.error(message, args);
                        } else {
                            console.error(message);
                        }
                    } catch (e) {
                    }
                }
            };
        }());
    function writeLog(logFunction, message, args) {
        logFunction(getTime() + " : " + message, args);
    }

    /// <summary>
    /// Displays the specified text within the browser Developer Tools Console, with no idicative icon (prefixed with "LOG: " in IE9)
    /// </summary>
    /// <param name="message">Text to be displayed within the browser Developer Console</param>
    self.verbose = function (message, args) {
        writeLog(safeConsole.log, message, args);
    };

    /// <summary>
    /// Displays the specified text within the browser Developer Tools Console, with an INFO icon (except in Chrome)
    /// </summary>
    /// <param name="message">Text to be displayed within the browser Developer Console</param>
    self.info = function (message, args) {
        writeLog(safeConsole.info, message, args);
    };

    /// <summary>
    /// Displays the specified text within the browser Developer Tools Console, with a WARNING icon
    /// </summary>
    /// <param name="message">Text to be displayed within the browser Developer Console</param>
    self.warn = function (message, args) {
        writeLog(safeConsole.warn, message, args);
    };

    /// <summary>
    /// Displays the specified text within the browser Developer Tools Console, with an ERROR icon
    /// </summary>
    /// <param name="message">Text to be displayed within the browser Developer Console</param>
    self.error = function (message, args) {
        writeLog(safeConsole.error, message, args);
    };
}());
