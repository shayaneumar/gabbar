/*----------------------------------------------------------------------------

(C) Copyright 2013 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global define: false*/

define('webapi.ajax', ['jquery', 'webapi.errors', 'webapi.csrf'],
    function ($, errors, csrf) {
        "use strict";

        return ({
            get: function (url, success, error) {
                $.ajax({
                    url: url,
                    type: 'GET',
                    dataType: 'json',
                    success: success,
                    error: errors.handle(error)
                });
            },

            // NOTE: This method appends the Request Token, therefore should only be used for internal requests
            post: function (url, data, success, error) {
                var jsonData = JSON.stringify(csrf.addToken(data));

                $.ajax({
                    url: url,
                    type: 'POST',
                    data: jsonData,
                    contentType: 'application/json',
                    success: success,
                    error: errors.handle(error)
                });
            },

            // NOTE: This method appends the Request Token, therefore should only be used for internal requests
            put: function (url, data, success, error) {
                var jsonData = JSON.stringify(csrf.addToken(data));

                $.ajax({
                    url: url,
                    type: 'PUT',
                    data: jsonData,
                    contentType: 'application/json',
                    success: success,
                    error: errors.handle(error)
                });
            }
        });
    }
    );