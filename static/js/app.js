/// <reference path="jquery.d.ts" />
/// <reference path="handlebars.d.ts" />
/// <reference path="history.d.ts" />
var HistoryJS = History;
var GTDPad = (function (window, $, history, tmpl) {
    var _templates = {
        page: null,
        list: null,
        item: null
    }, _ui = {
        content: null,
        sidebar: null
    };
    function init() {
        var testPage = {
            name: 'Test Page 1',
            lists: [
                {
                    name: 'Test List 1',
                    items: [
                        { text: 'Test Item 1' },
                        { text: 'Test Item 2' }
                    ]
                },
                {
                    name: 'Test List 2',
                    items: [
                        { text: 'Test Item 3' },
                        { text: 'Test Item 4' }
                    ]
                }
            ]
        };
        _templates.page = tmpl.compile($('#tmpl-page').html());
        _templates.list = tmpl.compile($('#tmpl-list').html());
        _templates.item = tmpl.compile($('#tmpl-item').html());
        tmpl.registerPartial('list', _templates.list);
        tmpl.registerPartial('item', _templates.item);
        _ui.content = $('div.content');
        _ui.sidebar = $('div.sidebar');
        _ui.content.html(_templates.page(testPage));
    }
    return {
        init: init
    };
}(window, jQuery, HistoryJS, Handlebars));
