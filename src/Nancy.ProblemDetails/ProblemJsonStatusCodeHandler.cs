using System.Collections.Generic;
using System.Linq;
using Nancy.ErrorHandling;
using Tavis;

namespace Nancy.ProblemDetails
{
    public delegate bool ShouldHandleStatusCode(HttpStatusCode code, NancyContext context);

    public delegate ProblemDocument ProblemDocumentFactry(NancyContext context);

    public abstract class ProblemJsonStatusCodeHandler : IStatusCodeHandler
    {
        private readonly ICollection<Handler> _handlers = new List<Handler>();

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return _handlers.Any(handler => handler.ShouldHandleStatusCode(statusCode, context));
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var handler = _handlers.First(h => h.ShouldHandleStatusCode(statusCode, context));

            var problemDocument = handler.ProblemDocumentFactory(context);
            problemDocument.StatusCode = (System.Net.HttpStatusCode)statusCode;
            context.Response = new ProblemJsonResponse(problemDocument).WithStatusCode(statusCode);
        }

        protected void When(ShouldHandleStatusCode shouldhandleStatusCode, ProblemDocumentFactry problemDocumentFactory)
        {
            _handlers.Add(new Handler
            {
                ShouldHandleStatusCode = shouldhandleStatusCode,
                ProblemDocumentFactory = problemDocumentFactory
            });
        }

        private class Handler
        {
            public ShouldHandleStatusCode ShouldHandleStatusCode { get; set; }
            public ProblemDocumentFactry ProblemDocumentFactory { get; set; }
        }
    }
}