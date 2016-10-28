/// <reference path="jquery.d.ts" />
/// <reference path="handlebars.d.ts" />
/// <reference path="history.d.ts" />
/// <reference path="sortablejs.d.ts" />
var HistoryJS = History;
var GTDPad = (function (window, $, history, tmpl, sortable) {
    var _templates = {
        page: null,
        list: null,
        item: null,
        sidebarPageList: null,
        sidebarPage: null,
        listAdd: null,
        itemAdd: null,
        listForm: null,
        itemForm: null
    }, _ui = {
        content: null,
        sidebar: null
    };
    function _forEachPropertyOf(obj, action) {
        for (var p in obj) {
            if (obj.hasOwnProperty(p)) {
                action(p, obj[p]);
            }
        }
    }
    function _init(initialData) {
        _forEachPropertyOf(_templates, function (k, v) {
            _templates[k] = tmpl.compile($('#tmpl-' + k).html());
        });
        tmpl.registerPartial('list', _templates.list);
        tmpl.registerPartial('listAdd', _templates.listAdd);
        tmpl.registerPartial('item', _templates.item);
        tmpl.registerPartial('itemAdd', _templates.itemAdd);
        tmpl.registerPartial('sidebarPage', _templates.sidebarPage);
        _ui.content = $('div.content');
        _ui.sidebar = $('div.sidebar');
        _ui.content.html(_templates.page(initialData.contentData));
        _ui.sidebar.html(_templates.sidebarPageList(initialData.sidebarData));
    }
    return {
        init: _init
    };
}(window, jQuery, HistoryJS, Handlebars, Sortable));
