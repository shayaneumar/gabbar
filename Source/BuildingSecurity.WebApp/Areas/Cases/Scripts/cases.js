/*----------------------------------------------------------------------------

(C) Copyright 2013 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global define: false*/

define('cases', ['webapi.routes', 'webapi.ajax', 'logging'],
    function (routes, ajax, log) {
        "use strict";

        var controllerName = "cases";

        function callErroHandlerWithParsedError(xhr, errorHandler) {
            if (errorHandler) {
                errorHandler({ messageType: 'error', message: JSON.parse(xhr.responseText).Message });
            }
        }

        return {
            get: function (id, success, error) {
                var parameters = { "id": id },
                    url = routes.resolve(controllerName, parameters);

                ajax.get(url, success, error);
            },

            getAll: function (success, error) {
                var parameters = {},
                    url = routes.resolve(controllerName, parameters);

                ajax.get(url, success, error);
            },

            create: function (title, success, error) {
                var url = routes.resolve(controllerName);
                ajax.post(url, { 'title': title }, function (caseId) {
                    log.info('Case created successfully');
                    if (success) {
                        success(caseId);
                    }
                }, function (xhr) {
                    log.error('Failed to create case');
                    callErroHandlerWithParsedError(xhr, error);
                });
            },

            updateStatus: function (caseId, status, success, error) {
                var url = routes.resolve(controllerName);
                ajax.put(url, { 'caseId': caseId, 'updatedField': 'status', 'status': status }, success, error);
            },

            createNote: function (caseId, text, success, error) {
                var url = routes.resolve("caseNotes");
                ajax.post(url, { 'caseId': caseId, 'text': text }, success, error);
            },

            updateCaseTitle: function (caseId, title, success, error) {
                var url = routes.resolve("cases");
                ajax.put(url, { 'caseId': caseId, 'updatedField': 'title', 'title': title }, success, error);
            },

            updateCaseOwner: function (caseId, owner, success, error) {
                var url = routes.resolve("cases");
                ajax.put(url, { 'caseId': caseId, 'updatedField': 'owner', 'owner': owner }, success, error);
            }
        };
    });