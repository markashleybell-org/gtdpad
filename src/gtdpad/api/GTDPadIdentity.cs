using System;
using System.Security.Principal;

namespace gtdpad
{
    public class GTDPadIdentity : GenericIdentity
    {
        public Guid Identifier { get; private set; }

        public GTDPadIdentity(Guid identifier, string username) : base(username)
        {
            Identifier = identifier;
        }
    }
}