/*----------------------------------------------------------------------------

(C) Copyright 2013 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global define: false*/

define('webapi.csrf', ['jquery'],
    function ($) {
        "use strict";

        function getCsrfPreventionToken() {
            return $('#__csrfprevention input[name=__RequestVerificationToken]').val();
        }

        return {
            addToken: function (data) {
                data.__RequestVerificationToken = getCsrfPreventionToken(); //dangling '_' is to prevent naming collisions
                return data;
            }
        };
    }
    );