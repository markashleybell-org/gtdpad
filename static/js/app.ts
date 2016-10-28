/// <reference path="jquery.d.ts" />
/// <reference path="handlebars.d.ts" />
/// <reference path="history.d.ts" />
/// <reference path="sortablejs.d.ts" />

var HistoryJS: Historyjs = <any>History;

interface ForEachPropertyOfAction { (k:string, v:any): void; }

var GTDPad = (function(window, $, history, tmpl, sortable) {
    var _pageID = null,
        _templates = {
            page: null,
            list: null,
            item: null,
            sidebarPageList: null,
            sidebarPage: null,
            listHeading: null,
            itemAdd: null,
            listForm: null,
            itemForm: null
        },
        _ui = {
            content: null,
            sidebar: null  
        },
        _xhr = {
            get: function (url, data, success?, error?) {
                _ajax('GET', url, data, success, error);
            },
            _post: function (url, data, success?, error?) {
                _ajax('POST', url, data, success, error);
            },
            _put: function (url, data, success?, error?) {
                _ajax('PUT', url, data, success, error);
            },
            _delete: function (url, data, success?, error?) {
                _ajax('PUT', url, data, success, error);
            }
        };

    function _forEachPropertyOf(obj:{}, action:ForEachPropertyOfAction) {
        for(var p in obj) {
            if (obj.hasOwnProperty(p)) {
                action(p, obj[p]);
            }
        }
    }

    function _serializeFormToJson(form:JQuery) {
        return form.serializeArray().reduce(function(data, field) { 
            data[field.name] = field.value; return data; 
        }, {});
    }

    function _getText(element:JQuery) {
        return $.trim(element[0].childNodes[0].nodeValue);
    }

    function _ajaxSuccess(dataSent, success) {
        if(typeof success === 'function') return success;

        return function(data, status, xhr) {
            console.log('Data Sent:', dataSent);
            console.log('Data: ', data);
            console.log('Status: ', status);
            console.log('XHR: ', xhr);
        }
    }

    function _ajaxError(dataSent, error) {
        if(typeof error === 'function') return error;

        return function(xhr, status, error) {
            console.log('Data Sent:', dataSent);
            console.log('XHR: ', xhr);
            console.log('Status: ', status);
            console.log('Error: ', error);
        }
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

    function _onAddListClick(e) {
        e.preventDefault();
        var a = $(this);
        a.parent().before(_templates.listForm({ 
            method: 'POST', 
            id: a.data('id'), 
            pageID: _pageID
        }));
    }

    function _onEditListClick(e) {
        e.preventDefault();
        var a = $(this);
        a.parent().replaceWith(_templates.listForm({ 
            method: 'PUT', 
            id: a.data('id'), 
            pageID: _pageID, 
            name: _getText(a.parent())
        }));
    }

    function _onListFormSubmit(e) {
        e.preventDefault();
        var form = $(this);
        var method = form.attr('method').toLowerCase();
        _xhr['_' + method](form.attr('action'), _serializeFormToJson(form), function(data) {
            form.replaceWith(_templates[method === 'put' ? 'listHeading' : 'list'](data));
        });
    }
    
    function _onAddItemClick(listID) {
        // Replace the new item link with a text box and submit button
    }

    function _onItemFormSubmit(itemID) {
        // Serialize the form inputs and POST/PUT them to the correct API endpoint
    }

    function _init(initialData) {
        // Set current page ID
        _pageID = initialData.contentData.id;

        _forEachPropertyOf(_templates, function(k, v) {
            _templates[k] = tmpl.compile($('#tmpl-' + k).html());
        });

        tmpl.registerPartial('list', _templates.list);
        tmpl.registerPartial('listHeading', _templates.listHeading);
        tmpl.registerPartial('item', _templates.item);
        tmpl.registerPartial('itemAdd', _templates.itemAdd);
        tmpl.registerPartial('sidebarPage', _templates.sidebarPage);

        _ui.content = $('div.content');
        _ui.sidebar = $('div.sidebar');

        _ui.content.html(_templates.page(initialData.contentData));
        _ui.sidebar.html(_templates.sidebarPageList(initialData.sidebarData));

        // Event handlers
        _ui.content.on('click', 'a.list-add', _onAddListClick);
        _ui.content.on('click', 'a.list-edit', _onEditListClick);
        _ui.content.on('submit', 'form.list-form', _onListFormSubmit);
    }

    return {
        init: _init
    };
}(window, jQuery, HistoryJS, Handlebars, Sortable));
