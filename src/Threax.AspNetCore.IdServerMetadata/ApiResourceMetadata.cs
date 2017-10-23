using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.IdServerMetadata
{
    [HalModel]
    public class ApiResourceMetadata
    {
        public String ScopeName { get; set; }

        public String DisplayName { get; set; }
    }
}
