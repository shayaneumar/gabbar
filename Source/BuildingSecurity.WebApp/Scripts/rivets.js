/*global define:false, require:false*/
define('rivets', ['jquery', 'ko', 'modernizr', 'crossroads', 'context', 'applicationPath'], function ($, ko, modernizr, crossroads, context, appPath) {
    'use strict';
    function construct(constructor, args) {
        function F() {
            return constructor.apply(this, args);
        }
        F.prototype = constructor.prototype;
        return new F();
    }
    function clear(containerName) {
        var $container = $('#' + containerName + '-container');
        $container.empty();
        ko.cleanNode($container[0]);
    }

    function explicitShow(containerName, viewId, viewModelConstructorName, viewModelParemeters) {
        var $container = $('#' + containerName + '-container'),
            //should take the templates object in as a dependency in the future.
            $templates = $(context.document.createElement('div')).html($('#part-templates').text()),
            $template = $templates.find('#' + viewId).clone(false);

        clear(containerName);

        $template.hide();
        require([viewModelConstructorName], function (createViewModel) {

            ko.applyBindings(construct(createViewModel, viewModelParemeters), $template[0]);
        });

        $template.appendTo($container).fadeIn('fast');
    }

    function show(containerName, contentName, viewModelParemeters) {
        //Use naming conventions to infer template and view model names
        var words = contentName.split(' '),
            upperCasedWords = words.map(function (word) {
                return word.charAt(0).toUpperCase() + word.slice(1);
            }),
            camelCased = words[0] + upperCasedWords.splice(1),
            hyphenated = words.join('-');
        explicitShow(containerName, hyphenated + '-view', camelCased + 'ViewModelFactory', viewModelParemeters);
    }

    return {
        clear: clear,
        show: function () {
            //This lets us call an overloaded function
            if (arguments.length <= 3) {
                show.apply(this, arguments);
            } else {
                explicitShow.apply(this, arguments);
            }
        },
        softlink: function (linkablePath) {
            crossroads.parse(this.getRoutablePath(linkablePath));
            if (context.location.pathname !== linkablePath && modernizr.history) {
                context.history.pushState(null, null, linkablePath);
            }
        },
        getLinkablePath: function (routablePath) {
            return appPath + routablePath;
        },
        getRoutablePath: function (linkablePath) {
            return linkablePath.substring(appPath.length);
        }

    };
});