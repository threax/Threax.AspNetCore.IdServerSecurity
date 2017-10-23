using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.IdServerAuth
{
    public class IdServerAuthAppOptions
    {
        /// <summary>
        /// The authority to use. Should be the url of the id server.
        /// </summary>
        public String Authority { get; set; }

        /// <summary>
        /// The client id to use.
        /// </summary>
        public String ClientId { get; set; }

        /// <summary>
        /// The primary scope for this app.
        /// </summary>
        public String Scope { get; set; }

        /// <summary>
        /// The display name for this application, will be used for the id server metadata.
        /// </summary>
        public String DisplayName { get; set; }

        /// <summary>
        /// Any additional scopes that this app should request.
        /// </summary>
        public List<String> AdditionalScopes { get; set; }

        /// <summary>
        /// The client secret for this app. This can be null to have no secret. It defaults to
        /// "notyetdefined". Ideally this should be set to something.
        /// </summary>
        public String ClientSecret { get; set; } = "notyetdefined";
    }
}
