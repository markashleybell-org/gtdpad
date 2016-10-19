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
        Page UpdatePage(Page page);
        Page DeletePage(Guid id);
        IEnumerable<Page> ListPages(Guid userID);
    }
}