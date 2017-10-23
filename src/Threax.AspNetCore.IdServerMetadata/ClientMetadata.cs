using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.IdServerMetadata
{
    [HalModel]
    public class ClientMetadata
    {
        public String ClientId { get; set; }

        public String Name { get; set; }

        public String LogoutUri { get; set; }

        public bool LogoutSessionRequired { get; set; } = true;

        public List<String> AllowedGrantTypes { get; set; } = new List<string>();

        public List<String> RedirectUris { get; set; } = new List<string>();

        public List<String> AllowedScopes { get; set; } = new List<string>();

        public bool EnableLocalLogin { get; set; }

        public int AccessTokenLifetime { get; set; } = 3600;
    }
}
