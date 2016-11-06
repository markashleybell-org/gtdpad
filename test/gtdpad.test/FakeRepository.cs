using System;
using System.Collections.Generic;
using System.Security.Claims;
using Nancy;

namespace gtdpad.test 
{
    public class FakeRepository : IRepository
    {
        public Item CreateItem(Item item)
        {
            throw new NotImplementedException();
        }

        public List CreateList(List list)
        {
            throw new NotImplementedException();
        }

        public Page CreatePage(Page page)
        {
            throw new NotImplementedException();
        }

        public Item DeleteItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public List DeleteList(Guid id)
        {
            throw new NotImplementedException();
        }

        public Page DeletePage(Guid id)
        {
            throw new NotImplementedException();
        }

        public ClaimsPrincipal GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            throw new NotImplementedException();
        }

        public Guid? GetUserID(string username)
        {
            throw new NotImplementedException();
        }

        public Guid ReadDefaultPageID(Guid userID)
        {
            throw new NotImplementedException();
        }

        public Item ReadItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Item> ReadItems(Guid listID)
        {
            throw new NotImplementedException();
        }

        public List ReadList(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<List> ReadLists(Guid pageID)
        {
            throw new NotImplementedException();
        }

        public Page ReadPage(Guid id)
        {
            throw new NotImplementedException();
        }

        public object ReadPageDeep(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Page> ReadPages(Guid userID)
        {
            throw new NotImplementedException();
        }

        public Item UpdateItem(Item item)
        {
            throw new NotImplementedException();
        }

        public void UpdateItemDisplayOrder(Ordering ordering)
        {
            throw new NotImplementedException();
        }

        public List UpdateList(List list)
        {
            throw new NotImplementedException();
        }

        public void UpdateListDisplayOrder(Ordering ordering)
        {
            throw new NotImplementedException();
        }

        public Page UpdatePage(Page page)
        {
            throw new NotImplementedException();
        }

        public void UpdatePageDisplayOrder(Ordering ordering)
        {
            throw new NotImplementedException();
        }

        public Guid? ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}