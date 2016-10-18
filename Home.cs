using Nancy;
using Nancy.Security;
using Dapper;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using Nancy.Authentication.Forms;

namespace gtdpad
{
    public class DataSource
    {
        string _connectionString;

        public DataSource(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string Test()
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                return conn.Query<string>("SELECT username FROM users WHERE id = 1").First().ToString();
            }
        }
    }

    public class UserDatabase : IUserMapper
    {
        private static List<Tuple<string, string, Guid>> users = new List<Tuple<string, string, Guid>>();

        static UserDatabase()
        {
            users.Add(new Tuple<string, string, Guid>("admin", "password", new Guid("55E1E49E-B7E8-4EEA-8459-7A906AC4D4C0")));
            users.Add(new Tuple<string, string, Guid>("user", "password", new Guid("56E1E49E-B7E8-4EEA-8459-7A906AC4D4C0")));
        }

        public ClaimsPrincipal GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var userRecord = users.FirstOrDefault(u => u.Item3 == identifier);

            return userRecord == null
                       ? null
                       : new ClaimsPrincipal(new GenericIdentity(userRecord.Item1));
        }

        public static Guid? ValidateUser(string username, string password)
        {
            var userRecord = users.FirstOrDefault(u => u.Item1 == username && u.Item2 == password);

            if (userRecord == null)
            {
                return null;
            }

            return userRecord.Item3;
        }
    }

    public class Home : NancyModule
    {
        public Home()
        {
            var dataSource = new DataSource("Server=localhost;Database=linx;Trusted_Connection=yes");

            Get("/", args => {
                return dataSource.Test();
            });

            Get("/secured", args => {
                this.RequiresAuthentication();
                return "SECURE";
            });

            Get("/login", args => {
                return View["login.html"];
            });

            Post("/login", args => {
                var id = UserDatabase.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);
                return this.LoginAndRedirect(id.Value, null);
            });

            Get("/logout", args => {
                return this.LogoutAndRedirect("~/");
            });
        }
    }
}