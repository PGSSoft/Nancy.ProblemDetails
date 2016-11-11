# Nancy.ProblemDetails [![Build status][av-badge]][build] [![NuGet version][lib-badge]][lib-link] [![codecov.io][cov-badge]][cov-link]

## About

Nancy responses following [RFC7807 - Problem Details for HTTP APIs](http://webconcepts.info/specs/IETF/RFC/7807).

Currently supports JSON

## Installation

```
package-install Nancy.ProblemDetails
```

## Usage

### Problem Details as model

In your modules you can simply return `Tavis.ProblemDocument`. For example

``` c#
public class SomeModule : NancyModule
{
    public SomeModule()
    {
        Post["some"] = _ => ComePostSome();
    }

    private dynamic ComeGetSome()
    {
        var model = this.BindAndValidate<Some>();

        if (ModelValidationResult.IsValid == false)
        {
            return new Tavis.ProblemDocument
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
        }

        // do somethign else
        return 200;
    }
}
```

### Nancy Response type

You can also use `ProblemJsonResponse` (for example in Pipelines).

``` c#
public class SomeHandlerStartup : IRequestStartup
{
    public void Initialize(IPipelines pipelines, NancyContext context)
    {
        pipelines.AfterRequest.AddItemToEndOfPipeline(ReturnProblemDetails);
    }

    private static void ReturnProblemDetails(NancyContext ctx)
    {
        ctx.Response = new ProblemJsonResponse(new ProblemDocument
        {
            StatusCode = System.Net.HttpStatusCode.Forbidden
        });
    }
}
```

### Status code handler

Finally, you can handle selected errors globally by implementing the abstract `ProblemJsonStatusCodeHandler` class

``` c#
public class ServerErrorHandler : ProblemJsonStatusCodeHandler
{
    public ServerErrorHandler()
    {
        // wire up handlers using the When method
        When(CodeIsForbidden, HandleForbidden);
    }

    private static bool CodeIsForbidden(HttpStatusCode code, NancyContext context)
    {
        // return true if codeshould be handled
        return code == HttpStatusCode.Forbidden;
    }

    private static ProblemDocument HandleForbidden(NancyContext context)
    {
        // create the details document to be serialized
        return new ProblemDocument
        {
            ProblemType = new Uri("http://my.api/error-codes/unauthorized")
        };
    }
```

[av-badge]: https://ci.appveyor.com/api/projects/status/xugkf0u8ahbxpqrq?svg=true
[build]: https://ci.appveyor.com/project/tpluscode78631/nancy-problemdetails
[lib-badge]: https://badge.fury.io/nu/nancy.problemdetails.svg
[lib-link]: https://badge.fury.io/nu/nancy.problemdetails
[cov-badge]: https://codecov.io/gh/tpluscode/nancy.problemdetails/branch/master/graph/badge.svg
[cov-link]: https://codecov.io/gh/tpluscode/nancy.problemdetails
