using System;
using Nancy;
using System.Security.Claims;
using System.Collections.Generic;

namespace gtdpad
{
    public interface IRepository
    {
        ClaimsPrincipal GetUserFromIdentifier(Guid identifier, NancyContext context);
        Guid? ValidateUser(string username, string password);
        Guid? GetUserID(string username);

        Page CreatePage(Page page);
        Page ReadPage(Guid id);
        object ReadPageDeep(Guid id);
        Page UpdatePage(Page page);
        Page DeletePage(Guid id);
        IEnumerable<Page> ReadPages(Guid userID);
        Guid ReadDefaultPageID(Guid userID);

        List CreateList(List list);
        List ReadList(Guid id);
        List UpdateList(List list);
        List DeleteList(Guid id);
        IEnumerable<List> ReadLists(Guid pageID);

        Item CreateItem(Item item);
        Item ReadItem(Guid id);
        Item UpdateItem(Item item);
        Item DeleteItem(Guid id);
        IEnumerable<Item> ReadItems(Guid listID);
    }
}