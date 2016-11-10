using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nancy.Responses.Negotiation;

namespace Nancy.ProblemDetails
{
    public class ProblemJsonProcessor : IResponseProcessor
    {
        [ExcludeFromCodeCoverage]
        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
        {
            get
            {
                yield break;
            }
        }

        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            var processorMatch = new ProcessorMatch
            {
                RequestedContentTypeResult = MatchResult.DontCare,
                ModelResult = MatchResult.NoMatch
            };

            if (requestedMediaRange.Matches("application/json")
                || requestedMediaRange.Subtype?.ToString().EndsWith("json", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                processorMatch.RequestedContentTypeResult = MatchResult.NonExactMatch;
            }

            if (model is Tavis.ProblemDocument)
            {
                processorMatch.ModelResult = MatchResult.ExactMatch;
            }

            return processorMatch;
        }

        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            return new ProblemJsonResponse((Tavis.ProblemDocument)model);
        }
    }
}
