using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AdventureWorks.DAL.Data;

namespace AdventureWorks.API
{
    public class QueryTrackerMiddleware
    {
        private readonly RequestDelegate _next;

        public QueryTrackerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, QueryTrackerService queryTracker)
        {
            // Capture the original response body
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                // Call the next middleware in the pipeline
                await _next(context);

                // If the response is not an error
                if (context.Response.StatusCode == StatusCodes.Status200OK && context.Response.ContentType?.Contains("application/json") == true)
                {
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    var responseBodyContent = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    // Deserialize the original response body
                    var originalResponse = JsonSerializer.Deserialize<dynamic>(responseBodyContent);

                    // Create the modified response by adding the sqlQueryCount
                    var modifiedResponse = new
                    {
                        data = originalResponse,
                        sqlQueryCount = queryTracker.QueryCount
                    };

                    // Serialize the modified response and write it back to the response body
                    var modifiedResponseBody = JsonSerializer.Serialize(modifiedResponse);
                    await context.Response.WriteAsync(modifiedResponseBody);

                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                }

                // Copy the contents of the new response body to the original response stream
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }

}