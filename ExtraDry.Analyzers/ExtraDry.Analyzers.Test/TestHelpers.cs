using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraDry.Analyzers.Test {
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

    public string Name { get; set; }

    public string Template { get; set; }

    public int Order { get; set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class HttpPatchAttribute : Attribute
{
    public HttpPatchAttribute() {}

    public HttpPatchAttribute(string route) {}

    public string Name { get; set; }

    public string Template { get; set; }

    public int Order { get; set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class HttpDeleteAttribute : Attribute
{
    public HttpDeleteAttribute() {}

    public HttpDeleteAttribute(string route) {}

    public string Name { get; set; }

    public string Template { get; set; }

    public int Order { get; set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class HttpPostAttribute : Attribute
{
    public HttpPostAttribute() {}

    public HttpPostAttribute(string route) {}

    public string Name { get; set; }

    public string Template { get; set; }

    public int Order { get; set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class HttpPutAttribute : Attribute
{
    public HttpPutAttribute() {}

    public HttpPutAttribute(string route) {}

    public string Name { get; set; }

    public string Template { get; set; }

    public int Order { get; set; }
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

public class ControllerBase {}

public class Controller : ControllerBase {}

";

    }
}
