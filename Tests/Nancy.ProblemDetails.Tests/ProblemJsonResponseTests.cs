using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Tavis;
using Xunit;

namespace Nancy.ProblemDetails.Tests
{
    public class ProblemJsonResponseTests
    {
        [Fact]
        public void ProblemJsonResponse_Should_serialize_to_correct_media_type()
        {
            // given
            var response = new ProblemJsonResponse(new ProblemDocument());

            // then
            response.ContentType.ShouldBe("application/problem+json");
        }

        [Fact]
        public void ProblemJsonResponse_Should_have_status_code_of_problem_document()
        {
            // given
            var response = new ProblemJsonResponse(new ProblemDocument
            {
                StatusCode = System.Net.HttpStatusCode.GatewayTimeout
            });

            // then
            response.StatusCode.ShouldBe(HttpStatusCode.GatewayTimeout);
        }

        [Fact]
        public void ProblemJsonResponse_Should_expose_problem_document_as_property()
        {
            // given
            var document = new ProblemDocument();
            var response = new ProblemJsonResponse(document);

            // then
            response.ProblemDocument.ShouldBeSameAs(document);
        }

        [Fact]
        public void Saving_ProblemJsonResponse_Should_save_correct_JSON()
        {
            // given
            Stream stream = new MemoryStream();
            var response = new ProblemJsonResponse(new ProblemDocument
            {
                Title = "I dunno",
                Detail = "Very serious error, NOT!",
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                ProblemInstance = new Uri("http://example.api/error/123"),
                ProblemType = new Uri("http://example.api/error"),
                Extensions = new Dictionary<string, JToken>
                {
                    { "codeLocation", new JValue("code.cs:65") }
                }
            });

            // when
            response.Contents(stream);
            stream.Seek(0, SeekOrigin.Begin);

            // then
            using (var reader = new StreamReader(stream))
            {
                var serializedObject = JToken.Parse(reader.ReadToEnd());

                var expected = JToken.Parse(@"{
    'title': 'I dunno',
    'detail': 'Very serious error, NOT!',
    'status': 500,
    'instance': 'http://example.api/error/123',
    'type': 'http://example.api/error',
    'codeLocation': 'code.cs:65'
}");
                JToken.DeepEquals(serializedObject, expected).ShouldBeTrue(() => serializedObject.ToString(Formatting.Indented));
            }
        }
    }
}
