/// <reference path="jquery.d.ts" />
/// <reference path="handlebars.d.ts" />
/// <reference path="history.d.ts" />
var HistoryJS = History;
var GTDPad = (function (window, $, history, tmpl) {
    function init() {
        console.log('INIT');
    }
    return {
        init: init
    };
}(window, jQuery, HistoryJS, Handlebars));
