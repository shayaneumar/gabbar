/*global requirejs:false, require:false, define:false, window:false, console:false, $:false, document:false */

(function (context) {
    "use strict";

    requirejs.config({
        //By default load any module ID that aren't already loaded via <script> tags from /client 
        baseUrl: 'Scripts'
    });

    $(document).ready(function () {
        // create require-able modules from the third party scripts included in the script tags in the page's head 
        define('jquery', [], function () { return context.jQuery; });
        define('ko', [], function () { return context.ko; });
        define('jci', [], function () { return context.jci; });
        define('jquery.bootstrap', [], function () { return context["jquery.bootstrap"]; });
        define('logging', [], function () { return window.logging; });
        define('context', [], function () { return window; });
        define('modernizr', [], function () { return window.Modernizr; });
        define('crossroads', [], function () { return window.crossroads; });
    });
} (window));