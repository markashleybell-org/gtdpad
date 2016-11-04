/// <reference path="jquery.d.ts" />
/// <reference path="handlebars.d.ts" />
/// <reference path="history.d.ts" />
/// <reference path="sortablejs.d.ts" />

var HistoryJS: Historyjs = <any>History;

interface ForEachPropertyOfAction { (k:string, v:any): void; }

var GTDPad = (function(window, console, $, history, tmpl, sortable) {
    var _pageID = null,
        _options = {
            debug: false
        },
        _templates = {
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
        },
        _ui = {
            content: null,
            sidebar: null,
            pageList: null
        };

    function _log(label:string, data:any) {
        if(_options.debug) console.log(label, data);
    }

    function _logGroup(label:string) {
        if(_options.debug) console.groupCollapsed(label);
    }

    function _logGroupEnd() {
        if(_options.debug) console.groupEnd();
    }

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

    function _xhrSuccess(dataSent, success) {
        if(typeof success === 'function') return success;

        return function(data, status, xhr) {
            _logGroup('XHR Request: OK');
            _log('Data Sent:', dataSent);
            _log('Data: ', data);
            _log('Status: ', status);
            _log('XHR: ', xhr);
            _logGroupEnd();
        }
    }

    function _xhrError(dataSent, error) {
        if(typeof error === 'function') return error;

        return function(xhr, status, error) {
            _logGroup('XHR Request: ERROR');
            _log('Data Sent:', dataSent);
            _log('XHR: ', xhr);
            _log('Status: ', status);
            _log('Error: ', error);
            _logGroupEnd();
        }
    }

    function _xhr(method, url, data, success?, error?) {
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
        _xhr('GET', a.attr('href'), { deep: true }, function(data) {
            history.pushState({}, data.title, '/' + data.id);
            _ui.content.html(_templates.page(data));
            _pageID = data.id;
            var page = _ui.content.find('.page');
            page.data('sortable', Sortable.create(page[0], {
                group: 'list',
                draggable: '.list',
                handle: '.drag-handle',
                animation: 150
            }));
            _ui.content.find('.list ul').each(function(i, item) {
                $(item).data('sortable', Sortable.create(item, {
                    group: 'listitem',
                    handle: '.drag-handle',
                    animation: 150
                }));
            });
        });
    }

    function _onAddPageClick(e) {
        e.preventDefault();
        var a = $(this);
        _ui.pageList.append(_templates.pageAddForm({ 
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
            title: _getText(a.parent())
        }));
    }

    function _onDeletePageClick(e) {
        e.preventDefault();
        var a = $(this);
        var url = '/pages/' + a.data('id');
        _xhr('DELETE', url, {}, function() {
            _xhr('GET', '/pages/default', { deep: true }, function(data) {
                history.pushState({}, data.title, '/');
                _ui.pageList.find('[href="' + url + '"]').parent().remove();
                _ui.content.html(_templates.page(data));
                _pageID = data.id;
            });
        });
    }

    function _onPageAddFormSubmit(e) {
        e.preventDefault();
        var form = $(this);
        _xhr('POST', form.attr('action'), _serializeFormToJson(form), function(data) {
            form.parent().replaceWith(_templates.sidebarPage(data));
        });
    }

    function _onPageEditFormSubmit(e) {
        e.preventDefault();
        var form = $(this);
        _xhr('PUT', form.attr('action'), _serializeFormToJson(form), function(data) {
            form.replaceWith(_templates.pageHeading(data));
            _ui.sidebar.find('[href="' + form.attr('action') + '"]').parent().replaceWith(_templates.sidebarPage(data));
        });
    }

    function _onAddListClick(e) {
        e.preventDefault();
        var a = $(this);
        _ui.content.find('.page').append(_templates.listForm({ 
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
            title: _getText(a.parent())
        }));
    }

    function _onDeleteListClick(e) {
        e.preventDefault();
        var a = $(this);
        var url = '/pages/' + _pageID + '/lists/' + a.data('id');
        _xhr('DELETE', url, {}, function() {
            $('#list-' + a.data('id')).remove();
        });
    }

    function _onListFormSubmit(e) {
        e.preventDefault();
        var form = $(this);
        var method = form.attr('method');
        _xhr(method, form.attr('action'), _serializeFormToJson(form), function(data) {
            form.replaceWith(_templates[method === 'PUT' ? 'listHeading' : 'list'](data));
            var page = _ui.content.find('.page');
            page.data('sortable').destroy();
            page.data('sortable', Sortable.create(page[0], {
                group: 'list',
                draggable: '.list',
                handle: '.drag-handle',
                animation: 150
            }));
            var list = $('#list-' + data.id + ' > ul');
            list.data('sortable', Sortable.create(list[0], {
                group: 'listitem',
                handle: '.drag-handle',
                animation: 150
            }));
        });
    }
    
    function _onAddItemClick(e) {
        e.preventDefault();
        var a = $(this);
        $('#list-' + a.data('listid') + ' > ul').append(_templates.itemForm({ 
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
            body: _getText(a.parent())
        }));
    }

    function _onDeleteItemClick(e) {
        e.preventDefault();
        var a = $(this);
        var url = '/pages/' + _pageID + '/lists/' + a.data('listid') + '/items/' + a.data('id');
        _xhr('DELETE', url, {}, function() {
            $('#item-' + a.data('id')).remove();
        });
    }

    function _onItemFormSubmit(e) {
        e.preventDefault();
        var form = $(this);
        var method = form.attr('method');
        _xhr(method, form.attr('action'), _serializeFormToJson(form), function(data) {
            form.parent().replaceWith(_templates.item(data));
        });
    }

    function _init(initialData, options:{}) {
        $.extend(_options, options);

        _pageID = initialData.contentData.id;

        _forEachPropertyOf(_templates, function(k, v) {
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

        _ui.pageList = _ui.sidebar.find('.sidebar-page-list ul');

        // Event handlers
        _ui.sidebar.on('click', 'a.page-add', _onAddPageClick);
        _ui.content.on('click', 'a.page-edit', _onEditPageClick);
        _ui.content.on('click', 'a.page-delete', _onDeletePageClick);
        _ui.sidebar.on('click', 'a.sidebar-page-link', _onPageLinkClick)
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

        _ui.pageList.data('sortable', Sortable.create(_ui.pageList[0], {
            group: 'page',
            handle: '.drag-handle',
            animation: 150,
            onSort: function (evt) {
                _xhr('PUT', '/pages/updateorder', { 
                    id: '00000000-0000-0000-0000-000000000000', 
                    order: this.toArray().join('|') 
                });
            }
        }));

        var page = _ui.content.find('.page');

        page.data('sortable', Sortable.create(page[0], {
            group: 'list',
            draggable: '.list',
            handle: '.drag-handle',
            animation: 150,
            onSort: function (evt) {
                _xhr('PUT', '/pages/' + _pageID + '/lists/updateorder', { 
                    id: _pageID, 
                    order: this.toArray().join('|') 
                });
            }
        }));

        _ui.content.find('.list ul').each(function(i, item) {
            $(item).data('sortable', Sortable.create(item, {
                group: 'listitem',
                handle: '.drag-handle',
                animation: 150,
                onSort: function (evt) {
                    var listID = $(this.el).parent().data('id');
                    _xhr('PUT', '/pages/' + _pageID + '/lists/' + listID + '/items/updateorder', { 
                        id: listID, 
                        order: this.toArray().join('|') 
                    });
                }
            }));
        });
        
    }

    return {
        init: _init
    };
}(window, console, jQuery, HistoryJS, Handlebars, Sortable));
