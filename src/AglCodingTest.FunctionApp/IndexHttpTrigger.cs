using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using AGLCodeTest.Dependencies;
using AGLCodeTest.Functions;
using AGLCodeTest.Functions.FunctionOptions;
using AGLCodeTest.Models;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AGLCodeTest.FunctionApp
{
    /// <summary>
    /// This represents the HTTP trigger entity for index.
    /// </summary>
    public static class IndexHttpTrigger
    {
        /// <summary>
        /// Gets or sets the <see cref="Dependencies.FunctionFactory"/> instance.
        /// </summary>
        public static IFunctionFactory FunctionFactory { get; set; } = new FunctionFactory();

        /// <summary>
        /// Runs the function.
        /// </summary>
        /// <param name="req"><see cref="HttpRequestMessage"/> instance.</param>
        /// <param name="log"><see cref="TraceWriter"/> instance.</param>
        /// <returns>Returns the <see cref="HttpResponseMessage"/> instance.</returns>
        [FunctionName("IndexHttpTrigger")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "pets")]HttpRequestMessage req, TraceWriter log)
        {
            AGLCodeTestHttpTriggerFunctionOptions options;
            try
            {
                options = GetOptions(req);
            }
            catch (Exception ex)
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            try
            {
                var res = await FunctionFactory.Create<IAGLCodeTestHttpTriggerFunction>(log)
                                               .InvokeAsync(req, options)
                                               .ConfigureAwait(false);

                return res as HttpResponseMessage;
            }
            catch (Exception ex)
            {
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        private static AGLCodeTestHttpTriggerFunctionOptions GetOptions(HttpRequestMessage req)
        {
            var pet = req.GetQueryNameValuePairs()
                         .SingleOrDefault(p => p.Key.Equals("type", StringComparison.CurrentCultureIgnoreCase))
                         .Value;
            var petType = string.IsNullOrWhiteSpace(pet)
                              ? PetType.Cat
                              : (PetType)Enum.Parse(typeof(PetType), pet, true);

            var options = new AGLCodeTestHttpTriggerFunctionOptions() { PetType = petType };

            return options;
        }
    }
}
