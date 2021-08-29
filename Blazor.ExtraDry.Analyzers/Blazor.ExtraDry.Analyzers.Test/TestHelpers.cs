using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.ExtraDry.Analyzers.Test {
    public static class TestHelpers {

        public static string Stubs = @"
using System;
using System.Collections.Generic;
using System.Text;

[AttributeUsage(AttributeTargets.Class)]
public class ApiControllerAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RouteAttribute : Attribute
{
    public RouteAttribute() {}

    public RouteAttribute(string route) {}
}

[AttributeUsage(AttributeTargets.Method)]
public class HttpGetAttribute : Attribute
{
    public HttpGetAttribute() {}

    public HttpGetAttribute(string route) {}
}

[AttributeUsage(AttributeTargets.Method)]
public class HttpPatchAttribute : Attribute
{
    public HttpPatchAttribute() {}

    public HttpPatchAttribute(string route) {}
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute
{
    public AllowAnonymousAttribute() {}
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute
{
    public AuthorizeAttribute() {}

    public AuthorizeAttribute(string policy) {}

    public string Policy { get; set; }

    public string Roles { get; set; }
}

";

    }
}
