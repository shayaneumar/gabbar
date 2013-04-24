/*----------------------------------------------------------------------------

(C) Copyright 2012 Johnson Controls, Inc.
Use or Copying of all or any part of this program, except as
permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

/*global jQuery: false, $: false, logging: false*/
jQuery.fn.limitedColorPicker = function (onChange) {
    "use strict";
    var colorPickers = this;
    colorPickers.each(function () {
        var colorPicker = $(this),
            input = $(colorPicker).find("input"),
            display = $(colorPicker).find(".color-display"),
            currentValue = input.val();

        //if it is a color which is not part of the palette show it as best we can.
        $(display).css("background-color", currentValue);
        $(colorPicker).find('.color-option').each(function () {
            var optionColor = $(this).attr("data-color");
            //Initialize background-color based on option color
            $(this).css("background-color", optionColor);
            if (optionColor === currentValue) {
                $(display).css("background-color", optionColor);
                $(display).css("background-image", $(this).css("background-image"));
            }
        });

        $(colorPicker).find(".color-option").on("mousedown", function () {
            logging.info("User picked a color.", this);
            var optionColor = $(this).attr("data-color");
            input.val(optionColor);
            display.css({
                "background-color": optionColor,
                "background-image": $(this).css("background-image")
            });

            $(colorPicker).blur();
            onChange.call(input, optionColor);
        });

        $(colorPicker).on("blur", function () {
            logging.info("Color picker lost focus.");
            $(this).find(".color-options").hide();
        });

        $(display).on("mousedown", function () {
            var position = $(this).position(),
                colorOptions = $(colorPicker).find(".color-options");
            $(colorOptions).css({ "top": position.top + position.height, "left": position.left });
            $(colorOptions).toggle();
            if ($(colorOptions).is(':visible')) {
                $(colorPicker).focus();
            } else {
                $(colorPicker).blur();
            }
        });
    });
};
