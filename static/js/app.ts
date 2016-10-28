/// <reference path="jquery.d.ts" />
/// <reference path="handlebars.d.ts" />
/// <reference path="history.d.ts" />
/// <reference path="sortablejs.d.ts" />

var HistoryJS: Historyjs = <any>History;

var GTDPad = (function(window, $, history, tmpl, sortable) {
    var _templates = {
            page: null,
            list: null,
            item: null,
            sidebarPageList: null,
            sidebarPage: null
        },
        _ui = {
            content: null,
            sidebar: null  
        };

    function init(initialData) {
        _templates.page = tmpl.compile($('#tmpl-page').html());
        _templates.list = tmpl.compile($('#tmpl-list').html());
        _templates.item = tmpl.compile($('#tmpl-item').html());
        _templates.sidebarPageList = tmpl.compile($('#tmpl-sidebar-page-list').html());
        _templates.sidebarPage = tmpl.compile($('#tmpl-sidebar-page').html());

        tmpl.registerPartial('list', _templates.list);
        tmpl.registerPartial('item', _templates.item);
        tmpl.registerPartial('sidebar-page', _templates.sidebarPage);

        _ui.content = $('div.content');
        _ui.sidebar = $('div.sidebar');

        _ui.content.html(_templates.page(initialData.contentData));
        _ui.sidebar.html(_templates.sidebarPageList(initialData.sidebarData));
    }

    return {
        init: init
    };
}(window, jQuery, HistoryJS, Handlebars, Sortable));
