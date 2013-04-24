/*----------------------------------------------------------------------------

(C) Copyright 2013 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global define: false*/

///
// Provides a way to resolve the url for a given api controller, id, and
// other parameters.
// This should be used to resolve urls for all webapi calls.
// Note: Parameters is optional.
///
define('webapi.routes', ['applicationPath'],
    function (applicationPath) {
        "use strict";

        function getQueryStringForParameters(obj) {
            var parameters = [], result, property;
            for (property in obj) {
                if (obj[property] !== undefined && property.toString() !== 'id') {
                    parameters.push(encodeURIComponent(property) + '=' + encodeURIComponent(obj[property]));
                }
            }
            result = parameters.join('&');
            if (result !== '') {
                return '?' + result;
            }
            return result;
        }

        return ({
            resolve: function (controller, parameters) {
                var parameterString = '';

                parameters = parameters !== undefined ? parameters : {};

                if (parameters.id !== undefined) {
                    parameterString = '/' + parameters.id;
                    parameters.id = null;
                }

                parameterString = parameterString + getQueryStringForParameters(parameters);
                return applicationPath + 'api/v1/' + controller + parameterString;
            }
        });
    }
    );