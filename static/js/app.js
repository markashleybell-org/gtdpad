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
    function _serializeFormToJson(form) {
        return form.serializeArray().reduce(function (data, field) {
            data[field.name] = field.value;
            return data;
        }, {});
    }
    function _ajaxSuccess(dataSent, success) {
        if (typeof success === 'function')
            return success;
        return function (data, status, xhr) {
            console.log('Data Sent:', dataSent);
            console.log('Data: ', data);
            console.log('Status: ', status);
            console.log('XHR: ', xhr);
        };
    }
    function _ajaxError(dataSent, error) {
        if (typeof error === 'function')
            return error;
        return function (xhr, status, error) {
            console.log('Data Sent:', dataSent);
            console.log('XHR: ', xhr);
            console.log('Status: ', status);
            console.log('Error: ', error);
        };
    }
    function _ajax(method, url, data, success, error) {
        var jsonData = JSON.stringify(data);
        $.ajax({
            url: url,
            data: jsonData,
            contentType: 'application/json;charset=utf-8',
            type: method,
            success: _ajaxSuccess(jsonData, success),
            error: _ajaxError(jsonData, error)
        });
    }
    function _get(url, data, success, error) {
        _ajax('GET', url, data, success, error);
    }
    function _post(url, data, success, error) {
        _ajax('POST', url, data, success, error);
    }
    function _put(url, data, success, error) {
        _ajax('PUT', url, data, success, error);
    }
    function _delete(url, data, success, error) {
        _ajax('PUT', url, data, success, error);
    }
    function _onAddListClick(e) {
        e.preventDefault();
        var a = $(this);
        a.parent().replaceWith(_templates.listForm({ pageID: a.data('pageid') }));
    }
    function _onListFormSubmit(e) {
        e.preventDefault();
        var form = $(this);
        var data = _serializeFormToJson(form);
        _post(form.attr('action'), data);
    }
    function _onAddItemClick(listID) {
        // Replace the new item link with a text box and submit button
    }
    function _onItemFormSubmit(itemID) {
        // Serialize the form inputs and POST/PUT them to the correct API endpoint
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
        // Event handlers
        _ui.content.on('click', 'a.list-add', _onAddListClick);
        _ui.content.on('submit', 'form.list-form', _onListFormSubmit);
    }
    return {
        init: _init
    };
}(window, jQuery, HistoryJS, Handlebars, Sortable));
