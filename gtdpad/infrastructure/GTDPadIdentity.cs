using System;
using System.Security.Principal;

namespace gtdpad
{
    public class GTDPadIdentity : GenericIdentity
    {
        public GTDPadIdentity(Guid identifier, string username)
            : base(username) => Identifier = identifier;

        public Guid Identifier { get; private set; }
    }
}
