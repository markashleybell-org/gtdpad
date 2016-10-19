using Dapper;
using Nancy;
using System.Linq;
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
            Execute("INSERT INTO pages (id, user_id, name, display_order) VALUES (@p0, @p1, @p2, @p3)", page.ID, page.UserID, page.Name, page.DisplayOrder);
            return ReadPage(page.ID);
        }

        public Page ReadPage(Guid id)
        {
            return GetSingle<Page>("SELECT * FROM pages WHERE id = @p0 AND deleted is null", id);
        }

        public Page UpdatePage(Page page)
        {
            Execute("UPDATE pages SET name = @p1, display_order = @p2 WHERE id = @p0", page.ID, page.Name, page.DisplayOrder);
            return ReadPage(page.ID);
        }

        public Page DeletePage(Guid id)
        {
            return GetSingle<Page>("UPDATE pages SET deleted = @p1 WHERE id = @p0; SELECT * FROM pages WHERE id = @p0;", id, DateTime.Now);
        }

        public IEnumerable<Page> ListPages(Guid userID)
        {
            return GetMultiple<Page>("SELECT * FROM pages WHERE user_id = @p0 AND deleted is null", userID);
        }

        // END Page methods
    }
}