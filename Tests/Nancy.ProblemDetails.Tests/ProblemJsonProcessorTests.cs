using System.Collections;
using Nancy.Responses.Negotiation;
using Shouldly;
using Tavis;
using Xunit;

namespace Nancy.ProblemDetails.Tests
{
    public class ProblemJsonProcessorTests
    {
        private readonly ProblemJsonProcessor _processor;

        public ProblemJsonProcessorTests()
        {
            _processor = new ProblemJsonProcessor();
        }

        [Fact]
        public void Process_Should_return_correct_Response()
        {
            // given
            var doc = new ProblemDocument();

            // when
            var response = _processor.Process(new MediaRange("application/json"), doc, new NancyContext());

            // thne
            response.ShouldBeOfType<ProblemJsonResponse>();
            ((ProblemJsonResponse)response).ProblemDocument.ShouldBeSameAs(doc);
        }

        [Theory]
        [MemberData(nameof(JsonMediaRanges))]
        public void CanProcess_Should_partially_match_json_media_Type(MediaRange mediaType)
        {
            // when
            var match = _processor.CanProcess(mediaType, new object(), new NancyContext());

            // then
            match.RequestedContentTypeResult.ShouldBe(MatchResult.NonExactMatch);
        }

        [Theory]
        [MemberData(nameof(NonJsonMediaRanges))]
        public void CanProcess_Should_ignore_non_json_media_Type(MediaRange mediaType)
        {
            // when
            var match = _processor.CanProcess(mediaType, new object(), new NancyContext());

            // then
            match.RequestedContentTypeResult.ShouldBe(MatchResult.DontCare);
        }

        [Fact]
        [MemberData(nameof(NonJsonMediaRanges))]
        public void CanProcess_Should_exactly_match_ProblemDocument()
        {
            // when
            var match = _processor.CanProcess(new MediaRange("application/json"), new ProblemDocument(), new NancyContext());

            // then
            match.ModelResult.ShouldBe(MatchResult.ExactMatch);
        }

        public static IEnumerable JsonMediaRanges
        {
            get
            {
                yield return new[] { new MediaRange("application/json") };
                yield return new[] { new MediaRange("application/vnd.siren+json") };
                yield return new[] { new MediaRange("application/ld+json") };
            }
        }

        public static IEnumerable NonJsonMediaRanges
        {
            get
            {
                yield return new[] { new MediaRange("application/xml") };
                yield return new[] { new MediaRange("text/plain") };
                yield return new[] { new MediaRange("text/html") };
                yield return new[] { new MediaRange("application/rdf+xml") };
                yield return new[] { new MediaRange("application/pdf") };
            }
        }
    }
}
