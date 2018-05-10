using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder
{
    public class PrefixPoliciesCallbackArgs
    {
        public PrefixPoliciesCallbackArgs(IServiceProvider services, IUserBuilder next)
        {
            this.Services = services;
            this.Next = next;
        }

        public IServiceProvider Services { get; private set; }

        public IUserBuilder Next { get; private set; }
    }
}
