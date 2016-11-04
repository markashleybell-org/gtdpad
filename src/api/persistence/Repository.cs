using Dapper;
using Nancy;
using System.Linq;
using System.Data;
using System;
using Nancy.Authentication.Forms;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Security.Principal;

namespace gtdpad
{
    public class Repository : IRepository, IUserMapper
    {
        string _connectionString;

        public Repository()
        {
            _connectionString = "Server=localhost;Database=gtdpad;Trusted_Connection=yes";
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        private DynamicParameters ConvertParameters(object[] parameters)
        {
            if(parameters == null || parameters.Length == 0)
                return null;

            var paramList = new Dictionary<string, object>();

            for (var i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == DBNull.Value)
                    paramList.Add("@p" + i, null);
                else
                    paramList.Add("@p" + i, parameters[i]);
            }

            return new DynamicParameters(paramList);
        }

        private T GetSingle<T>(string sql, params object[] parameters) 
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                return conn.Query<T>(sql, ConvertParameters(parameters)).FirstOrDefault();
            }
        }

        private IEnumerable<T> GetMultiple<T>(string sql, params object[] parameters) 
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                return conn.Query<T>(sql, ConvertParameters(parameters)).ToList();
            }
        }

        private void Execute(string sql, params object[] parameters) 
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                conn.Execute(sql, ConvertParameters(parameters));
            }
        }

        private void ExecuteProc(string sql, object parameters) 
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                conn.Execute(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // BEGIN Forms Auth methods

        public ClaimsPrincipal GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var userRecord = GetSingle<User>("SELECT * FROM users WHERE id = @p0", identifier);

            return userRecord == null ? null : new ClaimsPrincipal(new GenericIdentity(userRecord.Username));
        }

        public Guid? ValidateUser(string username, string password)
        {
            var userRecord = GetSingle<User>("SELECT * FROM users WHERE username = @p0 AND password = @p1", username, password);

            if (userRecord == null)
                return null;

            return userRecord.ID;
        }

        public Guid? GetUserID(string username)
        {
            var userRecord = GetSingle<User>("SELECT * FROM users WHERE username = @p0", username);

            if (userRecord == null)
                return null;

            return userRecord.ID;
        }

        // END Forms Auth methods

        // BEGIN Page methods

        public Page CreatePage(Page page)
        {
            Execute("INSERT INTO pages (id, user_id, title) VALUES (@p0, @p1, @p2)", page.ID, page.UserID, page.Title);
            return ReadPage(page.ID);
        }

        public Page ReadPage(Guid id)
        {
            return GetSingle<Page>("SELECT * FROM pages WHERE id = @p0 AND deleted is null", id);
        }

        public object ReadPageDeep(Guid id)
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                using (var multi = conn.QueryMultiple("ReadPageDeep", new { id = id }, commandType: CommandType.StoredProcedure))
                {
                    var page = multi.Read<Page>().Single();
                    var lists = multi.Read<List>().ToList();
                    var items = multi.Read<Item>().ToList();

                    return new { 
                        id = page.ID,
                        title = page.Title,
                        lists = lists.Select(list => new {
                            id = list.ID,
                            title = list.Title,
                            items = items.Where(item => item.ListID == list.ID).Select(item => new {
                                id = item.ID,
                                listID = item.ListID,
                                body = item.Body
                            })
                        })
                    };
                } 
            }
        }

        public Page UpdatePage(Page page)
        {
            Execute("UPDATE pages SET title = @p1 WHERE id = @p0", page.ID, page.Title);
            return ReadPage(page.ID);
        }

        public Page DeletePage(Guid id)
        {
            return GetSingle<Page>("UPDATE pages SET deleted = @p1 WHERE id = @p0; SELECT * FROM pages WHERE id = @p0;", id, DateTime.Now);
        }

        public IEnumerable<Page> ReadPages(Guid userID)
        {
            return GetMultiple<Page>("SELECT * FROM pages WHERE user_id = @p0 AND deleted is null ORDER BY display_order, created", userID);
        }

        public Guid ReadDefaultPageID(Guid userID)
        {
            var page = GetSingle<Page>("SELECT TOP 1 * FROM pages WHERE user_id = @p0 AND deleted is null ORDER BY display_order, created", userID);
            return page == null ? Guid.Empty : page.ID;
        }

        public void UpdatePageDisplayOrder(Ordering ordering)
        {
            ExecuteProc("UpdatePageDisplayOrder", new { userID = ordering.ID, order = ordering.Order });
        }

        // END Page methods

        // BEGIN List methods

        public List CreateList(List list)
        {
            Execute("INSERT INTO lists (id, page_id, title) VALUES (@p0, @p1, @p2)", list.ID, list.PageID, list.Title);
            return ReadList(list.ID);
        }

        public List ReadList(Guid id)
        {
            return GetSingle<List>("SELECT * FROM lists WHERE id = @p0 AND deleted is null", id);
        }

        public List UpdateList(List list)
        {
            Execute("UPDATE lists SET page_id = @p1, title = @p2 WHERE id = @p0", list.ID, list.PageID, list.Title);
            return ReadList(list.ID);
        }

        public List DeleteList(Guid id)
        {
            return GetSingle<List>("UPDATE lists SET deleted = @p1 WHERE id = @p0; SELECT * FROM lists WHERE id = @p0;", id, DateTime.Now);
        }

        public IEnumerable<List> ReadLists(Guid pageID)
        {
            return GetMultiple<List>("SELECT * FROM lists WHERE page_id = @p0 AND deleted is null ORDER BY display_order, created", pageID);
        }

        public void UpdateListDisplayOrder(Ordering ordering)
        {
            ExecuteProc("UpdateListDisplayOrder", new { pageID = ordering.ID, order = ordering.Order });
        }

        // END List methods

        // BEGIN Item methods

        public Item CreateItem(Item item)
        {
            Execute("INSERT INTO items (id, list_id, body) VALUES (@p0, @p1, @p2)", item.ID, item.ListID, item.Body);
            return ReadItem(item.ID);
        }

        public Item ReadItem(Guid id)
        {
            return GetSingle<Item>("SELECT * FROM items WHERE id = @p0 AND deleted is null", id);
        }

        public Item UpdateItem(Item item)
        {
            Execute("UPDATE items SET list_id = @p1, body = @p2 WHERE id = @p0", item.ID, item.ListID, item.Body);
            return ReadItem(item.ID);
        }

        public Item DeleteItem(Guid id)
        {
            return GetSingle<Item>("UPDATE items SET deleted = @p1 WHERE id = @p0; SELECT * FROM items WHERE id = @p0;", id, DateTime.Now);
        }

        public IEnumerable<Item> ReadItems(Guid listID)
        {
            return GetMultiple<Item>("SELECT * FROM items WHERE list_id = @p0 AND deleted is null ORDER BY display_order, created", listID);
        }

        public void UpdateItemDisplayOrder(Ordering ordering)
        {
            ExecuteProc("UpdateItemDisplayOrder", new { listID = ordering.ID, order = ordering.Order });
        }

        // END Item methods
    }
}