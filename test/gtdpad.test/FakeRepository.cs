using System;
using System.Collections.Generic;
using System.Security.Claims;
using Nancy;
using System.Linq;
using Nancy.Authentication.Forms;

namespace gtdpad.test 
{
    public class FakeRepository : IRepository, IUserMapper
    {
        private Guid _userID;
        private List<User> _users;
        private List<Page> _pages;
        private List<List> _lists;

        private List<Item> _items;

        public FakeRepository()
        {
            _userID = new Guid("39f4aa1a-c452-41ef-b0c9-e873274da1b3");

            _users = new List<User> {
                new User {
                    ID = _userID,
                    Username = "test"
                }
            };

            _pages = new List<Page> {
                new Page { 
                    ID = new Guid("9bcf9ac4-4256-4070-9553-7e2db1b4c368"),
                    UserID = _userID,
                    Title = "PAGE A"
                },
                new Page {
                    ID = new Guid("7f677dea-16e7-463d-b43e-1826fedbc0b7"),
                    UserID = _userID,
                    Title = "PAGE B"
                }
            };

            _lists = new List<List> {
                new List { 
                    ID = new Guid("548e9be5-14c9-4da2-bc7b-ffb83dff6c16"),
                    PageID = _pages[0].ID,
                    Title = "LIST A"
                },
                new List {
                    ID = new Guid("7d7ca192-fdd8-4784-89ab-b0247440bb1d"),
                    PageID = _pages[1].ID,
                    Title = "LIST B"
                }
            };

            _items = new List<Item> {
                new Item { 
                    ID = new Guid("92d22842-b842-4314-86c7-21bb1433925f"),
                    ListID = _lists[0].ID,
                    Body = "ITEM A"
                },
                new Item {
                    ID = new Guid("4aa46481-c8b6-431c-8182-ef1cdfc0e664"),
                    ListID = _lists[0].ID,
                    Body = "ITEM B"
                },
                new Item {
                    ID = new Guid("0003de9c-8b52-4322-8e56-be894c66a303"),
                    ListID = _lists[1].ID,
                    Body = "ITEM C"
                }
            };
        }

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
            return _pages.FirstOrDefault(p => p.ID == id);
        }

        public Page ReadPageDeep(Guid id)
        {
            var page = _pages.FirstOrDefault(p => p.ID == id);
            var lists = _lists.Where(l => l.PageID == id).ToList();
            var items = _items.Where(i => lists.Any(l => l.ID == i.ListID));

            lists.ForEach(list => list.Items = items.Where(item => item.ListID == list.ID));
            page.Lists = lists;

            return page;
        }

        public IEnumerable<Page> ReadPages(Guid userID)
        {
            return _pages.Where(p => p.UserID == userID);
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