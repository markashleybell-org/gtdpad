/// <reference path="jquery.d.ts" />
/// <reference path="handlebars.d.ts" />
/// <reference path="history.d.ts" />

var HistoryJS: Historyjs = <any>History;

var GTDPad = (function(window, $, history, tmpl) {
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
        var contentData = {
            id: 'GUID1',
            name: 'Test Page 1',
            lists: [
                {
                    id: 'GUID4',
                    name: 'Test List 1',
                    items: [
                        { id: 'GUID6', text: 'Test Item 1' },
                        { id: 'GUID7', text: 'Test Item 2' }
                    ]
                },
                {
                    id: 'GUID5',
                    name: 'Test List 2',
                    items: [
                        { id: 'GUID8', text: 'Test Item 3' },
                        { id: 'GUID9', text: 'Test Item 4' }
                    ]
                }
            ]
        };

        var sidebarData = {
            pages: [
                { id: 'GUID1', name: 'Test Page 1' },
                { id: 'GUID2', name: 'Test Page 2' },
                { id: 'GUID3', name: 'Test Page 3' }
            ]
        };

        contentData = initialData.contentData;
        sidebarData = initialData.sidebarData;

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

        _ui.content.html(_templates.page(contentData));
        _ui.sidebar.html(_templates.sidebarPageList(sidebarData));
    }

    return {
        init: init
    };
}(window, jQuery, HistoryJS, Handlebars));
