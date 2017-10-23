using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder
{
    public class AdditionalPoliciesCallbackArgs
    {
        public AdditionalPoliciesCallbackArgs(IServiceProvider services)
        {
            this.Services = services;
        }

        public IServiceProvider Services { get; private set; }
    }
}
