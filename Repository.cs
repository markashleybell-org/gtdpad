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
    public class Repository : IUserMapper
    {
        string _connectionString;

        public Repository()
        {
            _connectionString = "Server=localhost;Database=gtdpad;Trusted_Connection=yes";
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

        private int Create(string sql, params object[] parameters) 
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                return conn.Query<int>(sql, ConvertParameters(parameters)).FirstOrDefault();
            }
        }

        private void Update(string sql, params object[] parameters) 
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                conn.Execute(sql, ConvertParameters(parameters));
            }
        }

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
    }
}