using Tavis;

namespace Nancy.ProblemDetails
{
    public class ProblemJsonResponse : Response
    {
        public ProblemJsonResponse(ProblemDocument problemDocument)
        {
            ProblemDocument = problemDocument;
            Contents = problemDocument.Save;
            ContentType = "application/problem+json";
            StatusCode = (HttpStatusCode) problemDocument.StatusCode.GetValueOrDefault();
        }

        public ProblemDocument ProblemDocument { get; }
    }
}