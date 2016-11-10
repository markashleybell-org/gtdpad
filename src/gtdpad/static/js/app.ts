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

    function _log(label:string, data?:any) {
        if(_options.debug) {
            if(data)
                console.log(label, data);
            else
                console.log(label);
        }
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
        var itemLink = element.find('a.item-link');
        if(itemLink.length) {
            return itemLink.attr('href');
        }
        return element.contents()
                      .filter(function(){ return this.nodeType === 3 && $.trim(this.nodeValue) !== ''; })
                      .get()
                      .map(function(el, i) { return $.trim(el.nodeValue); })[0];
    }

    function _autoLink(text) {
        if(text == null) return null;
        return text.replace(/((?:https?|ftp|dict):\/\/[^\s\<]+)/img, function(match, group) {
            return '<a class="item-link" href="' + group + '">' + group + '</a>';
        });
    }

    function _focusTextInput(input) {
        input.focus();
        var val = input.val();
        input.val('');
        input.val(val);
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

    function _setupPageSorting() {
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
    }

    function _setupListSorting() {
        var page = _ui.content.find('.page');

        if(page.data('sortable'))
            page.data('sortable').destroy();

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
    }

    function _setupItemSorting(list) {
        if(list.data('sortable'))
            list.data('sortable').destroy();

        list.data('sortable', Sortable.create(list[0], {
            group: 'listitem',
            handle: '.drag-handle',
            animation: 150,
            onAdd: function (evt) {
                var item = $(evt.item);
                var itemID = item.data('id');
                var fromListID = $(evt.from).parent().data('id');
                var toList = $(evt.to);
                var toListID = toList.parent().data('id');
                _xhr('PUT', '/pages/' + _pageID + '/lists/' + toListID + '/items/' + itemID, { 
                    id: itemID,
                    listID: toListID,
                    body: _getText(item)
                }, function(data) {
                    _xhr('PUT', '/pages/' + _pageID + '/lists/' + toListID + '/items/updateorder', { 
                        id: toListID,
                        order: toList.data('sortable').toArray().join('|') 
                    });
                });
            },
            onUpdate: function (evt) {
                var listID = $(this.el).parent().data('id');
                _xhr('PUT', '/pages/' + _pageID + '/lists/' + listID + '/items/updateorder', { 
                    id: listID, 
                    order: this.toArray().join('|') 
                });
            }
        }));
    }

    function _onPageLinkClick(e) {
        e.preventDefault();
        var a = $(this);
        _xhr('GET', a.attr('href'), { deep: true }, function(data) {
            history.pushState({}, data.title, '/' + data.id);
            _ui.content.html(_templates.page(data));
            _pageID = data.id;
            _setupPageSorting();
            _setupListSorting();
            _ui.content.find('.list ul').each(function(i, item) {
                _setupItemSorting($(item));
            });
        });
    }

    function _onAddPageClick(e) {
        e.preventDefault();
        var a = $(this);
        a.parent().before(_templates.pageAddForm({ 
            method: 'POST', 
            id: a.data('id')
        }));
        _focusTextInput(_ui.sidebar.find('input[name="title"]:first'));
        _ui.sidebar.find('.cancel-button').on('click', function(e) { 
            e.preventDefault();
            $(this).parents('form').remove();
        });
    }

    function _onEditPageClick(e) {
        e.preventDefault();
        var a = $(this);
        var heading = a.parents('h1');
        var page = heading.parents('.page');
        var id = a.data('id');
        var title = _getText(heading);
        heading.replaceWith(_templates.pageEditForm({ 
            method: 'PUT', 
            id: id,
            title: title
        }));
        _focusTextInput(page.find('input[name="title"]:first'));
        _ui.content.find('.cancel-button').on('click', function(e) { 
            e.preventDefault();
            $(this).parents('form').replaceWith(_templates.pageHeading({
                id: id,
                title: title
            }));
        });
    }

    function _onDeletePageClick(e) {
        e.preventDefault();
        if(confirm('Are you sure you want to delete this page?')) {
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
    }

    function _onPageAddFormSubmit(e) {
        e.preventDefault();
        var form = $(this);
        _xhr('POST', form.attr('action'), _serializeFormToJson(form), function(data) {
            form.remove();
            _ui.pageList.append(_templates.sidebarPage(data));
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
        var page = _ui.content.find('.page');
        page.append(_templates.listForm({ 
            method: 'POST', 
            id: a.data('id'), 
            pageID: _pageID
        }));
        _focusTextInput(page.find('> .list-form input[name="title"]:first'));
        _ui.content.find('.cancel-button').on('click', function(e) { 
            e.preventDefault();
            $(this).parents('form').remove();
        });
    }

    function _onEditListClick(e) {
        e.preventDefault();
        var a = $(this);
        var heading = a.parents('h2');
        var list = heading.parents('.list');
        var id = a.data('id');
        var title = _getText(heading);
        heading.replaceWith(_templates.listForm({ 
            method: 'PUT', 
            id: id, 
            pageID: _pageID, 
            title: title
        }));
        _focusTextInput(list.find('input[name="title"]:first'));
        _ui.content.find('.cancel-button').on('click', function(e) { 
            e.preventDefault();
            $(this).parents('form').replaceWith(_templates.listHeading({
                id: id,
                title: title
            }));
        });
    }

    function _onDeleteListClick(e) {
        e.preventDefault();
        if(confirm('Are you sure you want to delete this list?')) {
            var a = $(this);
            var url = '/pages/' + _pageID + '/lists/' + a.data('id');
            _xhr('DELETE', url, {}, function() {
                $('#list-' + a.data('id')).remove();
            });
        }
    }

    function _onListFormSubmit(e) {
        e.preventDefault();
        var form = $(this);
        var method = form.attr('method');
        _xhr(method, form.attr('action'), _serializeFormToJson(form), function(data) {
            form.replaceWith(_templates[method === 'PUT' ? 'listHeading' : 'list'](data));
            _setupListSorting();
            _setupItemSorting($('#list-' + data.id + ' > ul'));
        });
    }

    function _onAddItemClick(e) {
        e.preventDefault();
        var a = $(this);
        var list = $('#list-' + a.data('listid'));
        list.find(' > ul').append(_templates.itemForm({ 
            method: 'POST', 
            id: a.data('id'), 
            listID: a.data('listid'),
            pageID: _pageID
        }));
        _focusTextInput(list.find('input[name="body"]:first'));
        _ui.content.find('.cancel-button').on('click', function(e) { 
            e.preventDefault();
            $(this).parents('form').remove();
        });
    }

    function _onEditItemClick(e) {
        e.preventDefault();
        var a = $(this);
        var li = a.parents('li');
        var id = a.data('id');
        var listID = a.data('listid');
        var list = $('#list-' + listID);
        var text = _getText(li);
        li.replaceWith(_templates.itemForm({ 
            method: 'PUT', 
            id: id, 
            listID: listID, 
            pageID: _pageID,
            body: text
        }));
        _focusTextInput(list.find('input[name="body"]:first'));
        _ui.content.find('.cancel-button').on('click', function(e) { 
            e.preventDefault();
            $(this).parents('form').parent().replaceWith(_templates.item({
                id: id,
                listID: listID, 
                pageID: _pageID,
                body: text
            }));
        });
    }

    function _onDeleteItemClick(e) {
        e.preventDefault();
        if(confirm('Are you sure you want to delete this item?')) {
            var a = $(this);
            var url = '/pages/' + _pageID + '/lists/' + a.data('listid') + '/items/' + a.data('id');
            _xhr('DELETE', url, {}, function() {
                $('#item-' + a.data('id')).remove();
            });
        }
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

        tmpl.registerHelper('autoLink', function(options) {
            return new Handlebars.SafeString(_autoLink(options.fn(this)));
        });

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

        _setupPageSorting();
        _setupListSorting();
        _ui.content.find('.list ul').each(function(i, item) {
            _setupItemSorting($(item));
        });
    }

    return {
        init: _init
    };
}(window, console, jQuery, HistoryJS, Handlebars, Sortable));
