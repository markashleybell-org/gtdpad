/// <reference path="jquery.d.ts" />
/// <reference path="handlebars.d.ts" />
/// <reference path="history.d.ts" />
/// <reference path="sortablejs.d.ts" />
var HistoryJS = History;
var GTDPad = (function (window, $, history, tmpl, sortable) {
    var _pageID = null, _templates = {
        page: null,
        list: null,
        item: null,
        sidebarPageList: null,
        sidebarPage: null,
        listHeading: null,
        listForm: null,
        itemForm: null,
        pageAddForm: null,
        pageEditForm: null,
        pageHeading: null
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
    function _getText(element) {
        return $.trim(element[0].childNodes[0].nodeValue);
    }
    function _xhrSuccess(dataSent, success) {
        if (typeof success === 'function')
            return success;
        return function (data, status, xhr) {
            console.log('Data Sent:', dataSent);
            console.log('Data: ', data);
            console.log('Status: ', status);
            console.log('XHR: ', xhr);
        };
    }
    function _xhrError(dataSent, error) {
        if (typeof error === 'function')
            return error;
        return function (xhr, status, error) {
            console.log('Data Sent:', dataSent);
            console.log('XHR: ', xhr);
            console.log('Status: ', status);
            console.log('Error: ', error);
        };
    }
    function _xhr(method, url, data, success, error) {
        var baseOptions = {
            url: url,
            type: method
        };
        var jsonData = JSON.stringify(data);
        var jsonOptions = {
            data: jsonData,
            contentType: 'application/json;charset=utf-8',
            success: _xhrSuccess(jsonData, success),
            error: _xhrError(jsonData, error)
        };
        var formOptions = {
            data: data,
            success: _xhrSuccess(data, success),
            error: _xhrError(data, error)
        };
        var options = $.extend(baseOptions, method === 'GET' ? formOptions : jsonOptions);
        $.ajax(options);
    }
    function _onPageLinkClick(e) {
        e.preventDefault();
        var a = $(this);
        _xhr('GET', a.attr('href'), {}, function (data) {
            _ui.content.html(_templates.page(data));
            _pageID = data.id;
        });
    }
    function _onAddPageClick(e) {
        e.preventDefault();
        var a = $(this);
        a.parent().before(_templates.pageAddForm({
            method: 'POST',
            id: a.data('id')
        }));
    }
    function _onEditPageClick(e) {
        e.preventDefault();
        var a = $(this);
        a.parent().replaceWith(_templates.pageEditForm({
            method: 'PUT',
            id: a.data('id'),
            name: _getText(a.parent())
        }));
    }
    function _onDeletePageClick(e) {
        e.preventDefault();
        var a = $(this);
        var url = '/pages/' + a.data('id');
        _xhr('DELETE', url, {}, function () {
            _xhr('GET', '/pages/default', {}, function (data) {
                _ui.sidebar.find('[href="' + url + '"]').parent().remove();
                _ui.content.html(_templates.page(data));
                _pageID = data.id;
            });
        });
    }
    function _onPageAddFormSubmit(e) {
        e.preventDefault();
        var form = $(this);
        _xhr('POST', form.attr('action'), _serializeFormToJson(form), function (data) {
            form.replaceWith(_templates.sidebarPage(data));
        });
    }
    function _onPageEditFormSubmit(e) {
        e.preventDefault();
        var form = $(this);
        var method = form.attr('method');
        _xhr(method, form.attr('action'), _serializeFormToJson(form), function (data) {
            form.replaceWith(_templates[method === 'PUT' ? 'listHeading' : 'list'](data));
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
    function _onDeleteListClick(e) {
        e.preventDefault();
        var a = $(this);
        var url = '/pages/' + _pageID + '/lists/' + a.data('id');
        _xhr('DELETE', url, {}, function () {
            $('#list-' + a.data('id')).remove();
        });
    }
    function _onListFormSubmit(e) {
        e.preventDefault();
        var form = $(this);
        var method = form.attr('method');
        _xhr(method, form.attr('action'), _serializeFormToJson(form), function (data) {
            form.replaceWith(_templates[method === 'PUT' ? 'listHeading' : 'list'](data));
        });
    }
    function _onAddItemClick(e) {
        e.preventDefault();
        var a = $(this);
        a.parent().before(_templates.itemForm({
            method: 'POST',
            id: a.data('id'),
            listID: a.data('listid'),
            pageID: _pageID
        }));
    }
    function _onEditItemClick(e) {
        e.preventDefault();
        var a = $(this);
        a.parent().replaceWith(_templates.itemForm({
            method: 'PUT',
            id: a.data('id'),
            listID: a.data('listid'),
            pageID: _pageID,
            text: _getText(a.parent())
        }));
    }
    function _onDeleteItemClick(e) {
        e.preventDefault();
        var a = $(this);
        var url = '/pages/' + _pageID + '/lists/' + a.data('listid') + '/items/' + a.data('id');
        _xhr('DELETE', url, {}, function () {
            $('#item-' + a.data('id')).remove();
        });
    }
    function _onItemFormSubmit(e) {
        e.preventDefault();
        var form = $(this);
        var method = form.attr('method');
        _xhr(method, form.attr('action'), _serializeFormToJson(form), function (data) {
            form.replaceWith(_templates[method === 'PUT' ? 'item' : 'item'](data));
        });
    }
    function _init(initialData) {
        // Set current page ID
        _pageID = initialData.contentData.id;
        _forEachPropertyOf(_templates, function (k, v) {
            _templates[k] = tmpl.compile($('#tmpl-' + k).html());
        });
        tmpl.registerPartial('list', _templates.list);
        tmpl.registerPartial('listHeading', _templates.listHeading);
        tmpl.registerPartial('item', _templates.item);
        tmpl.registerPartial('sidebarPage', _templates.sidebarPage);
        tmpl.registerPartial('pageHeading', _templates.pageHeading);
        _ui.content = $('div.content');
        _ui.sidebar = $('div.sidebar');
        _ui.content.html(_templates.page(initialData.contentData));
        _ui.sidebar.html(_templates.sidebarPageList(initialData.sidebarData));
        // Event handlers
        _ui.sidebar.on('click', 'a.page-add', _onAddPageClick);
        _ui.content.on('click', 'a.page-edit', _onEditPageClick);
        _ui.content.on('click', 'a.page-delete', _onDeletePageClick);
        _ui.sidebar.on('click', 'a.sidebar-page-link', _onPageLinkClick);
        _ui.sidebar.on('submit', 'form.page-add-form', _onPageAddFormSubmit);
        _ui.content.on('submit', 'form.page-edit-form', _onPageEditFormSubmit);
        _ui.content.on('click', 'a.list-add', _onAddListClick);
        _ui.content.on('click', 'a.list-edit', _onEditListClick);
        _ui.content.on('click', 'a.list-delete', _onDeleteListClick);
        _ui.content.on('submit', 'form.list-form', _onListFormSubmit);
        _ui.content.on('click', 'a.item-add', _onAddItemClick);
        _ui.content.on('click', 'a.item-edit', _onEditItemClick);
        _ui.content.on('click', 'a.item-delete', _onDeleteItemClick);
        _ui.content.on('submit', 'form.item-form', _onItemFormSubmit);
    }
    return {
        init: _init
    };
}(window, jQuery, HistoryJS, Handlebars, Sortable));
