/*----------------------------------------------------------------------------

(C) Copyright 2013 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global define: false*/

///
// Provides generic error handling for ajax requests.
// Handles:
//      401 redirects to sign-in page
// All other errors are handled by the pass error handler
///
define('webapi.errors', ['logging', 'returnToSignin'],
    function (logging, returnToSignin) {
        "use strict";

        return ({
            handle: function (errorHandler) {
                return function (xhr, ajaxOptions, thrownError) {
                    if (xhr.status === 401) {
                        logging.info("An ajax call resulted in a 401 status code.", xhr);
                        returnToSignin();
                        return;
                    }
                    if (errorHandler !== undefined) {
                        errorHandler(xhr, ajaxOptions, thrownError);
                    }
                };
            }
        });
    }
    );