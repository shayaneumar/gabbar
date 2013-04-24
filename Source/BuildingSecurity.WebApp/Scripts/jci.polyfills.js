/*----------------------------------------------------------------------------

(C) Copyright 2012 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
/*global $: false, document:false*/

$.fn.extend({
    applyPolyfills: function () {
        'use strict';
        var self = this;
        /*--------------Password fields select content on focus-------*/
        // for selectOnFocus class, this helper selects
        // text in password field when tabbing into it for IE.
        $(self).find("input[type='password']").on('focus click', function () {
            var element = $(this);
            element.select();
        });

        /*-------------Numeric Input Min/Max--------------------------*/
        $(self).find("input[type='number']").on('focus', function () {
            var element = $(this),
                value = element.val();
            element.attr("data-previousValue", value);
        });

        $(self).find("input[type='number']").on('blur', function () {
            var element = $(this),
                valIsNumber = !isNaN(element.val()),
                value = parseFloat(element.val()),
                min = parseFloat(element.attr("min")),
                max = parseFloat(element.attr("max")),
                step = parseFloat(element.attr("step")),
                previousValue = element.attr("data-previousValue");

            if (!valIsNumber || (step && value % step !== 0)) {//if new value is not a number or an increment of the step
                element.val(previousValue);
                element.change();
            }
            if (!isNaN(min) && value < min) {
                element.val(min);
                element.change();
            } else if (!isNaN(max) && value > max) {
                element.val(max);
                element.change();
            }
        });

        $(self).find("form").submit(function () {
            $(this).find(':submit').focus();
        });
    }
});
$(document).ready(function () {
    'use strict';
    $(document).applyPolyfills();
});
