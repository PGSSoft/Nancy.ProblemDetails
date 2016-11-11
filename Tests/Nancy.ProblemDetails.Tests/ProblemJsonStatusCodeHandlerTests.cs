using Shouldly;
using Tavis;
using Xunit;

namespace Nancy.ProblemDetails.Tests
{
    public class ProblemJsonStatusCodeHandlerTests
    {
        private readonly ProblemJsonStatusCodeHandlerTestable _handler;

        public ProblemJsonStatusCodeHandlerTests()
        {
            _handler = new ProblemJsonStatusCodeHandlerTestable();
        }

        [Fact]
        public void When_not_set_up_to_handle_status_code_HandlesStatusCode_Should_return_false()
        {
            // when
            var handlesStatusCode = _handler.HandlesStatusCode(HttpStatusCode.BadGateway, new NancyContext());

            // then
            handlesStatusCode.ShouldBeFalse();
        }

        [Fact]
        public void When_set_up_to_handle_status_code_HandlesStatusCode_Should_return_true()
        {
            // given
            _handler.SetHandler(HttpStatusCode.BadGateway);

            // when
            var handlesStatusCode = _handler.HandlesStatusCode(HttpStatusCode.BadGateway, new NancyContext());

            // then
            handlesStatusCode.ShouldBeTrue();
        }

        [Fact]
        public void When_not_set_up_to_handle_status_code_Handle_should_set_ProblemJsonResponse_with_returned_ProblemDocument()
        {
            // given
            var nancyContext = new NancyContext();
            var problemDocument = new ProblemDocument();
            _handler.SetHandler(HttpStatusCode.BadGateway, problemDocument);

            // when
            _handler.Handle(HttpStatusCode.BadGateway, nancyContext);

            // then
            nancyContext.Response.ShouldBeOfType<ProblemJsonResponse>();
            ((ProblemJsonResponse)nancyContext.Response).ProblemDocument.ShouldBeSameAs(problemDocument);
        }

        [Fact]
        public void When_set_up_to_handle_status_code_Handle_should_set_ProblemJsonResponse_with_unchanged_code()
        {
            // given
            var nancyContext = new NancyContext();
            var problemDocument = new ProblemDocument();
            _handler.SetHandler(HttpStatusCode.BadGateway, problemDocument);

            // when
            _handler.Handle(HttpStatusCode.BadGateway, nancyContext);

            // then
            nancyContext.Response.StatusCode.ShouldBe(HttpStatusCode.BadGateway);
        }

        [Fact]
        public void When_set_up_to_handle_status_code_Handle_should_set_ProblemDocument_status_code()
        {
            // given
            var nancyContext = new NancyContext();
            var problemDocument = new ProblemDocument();
            _handler.SetHandler(HttpStatusCode.BadGateway, problemDocument);

            // when
            _handler.Handle(HttpStatusCode.BadGateway, nancyContext);

            // then
            ((ProblemJsonResponse)nancyContext.Response).ProblemDocument.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadGateway);
        }

        private class ProblemJsonStatusCodeHandlerTestable : ProblemJsonStatusCodeHandler
        {
            public void SetHandler(HttpStatusCode httpStatusCode, ProblemDocument problemDocument = null)
            {
                problemDocument = problemDocument ?? new ProblemDocument();
                When((code, context) => code == httpStatusCode, context => problemDocument);
            }
        }
    }
}