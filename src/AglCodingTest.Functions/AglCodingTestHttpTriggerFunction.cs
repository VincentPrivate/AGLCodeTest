﻿using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using AGLCodeTest.Extensions;
using AGLCodeTest.Functions.FunctionOptions;
using AGLCodeTest.Services;
using AGLCodeTest.Services.ServiceOptions;

namespace AGLCodeTest.Functions
{
    /// <summary>
    /// This represents the function entity to process for AGL coding test.
    /// </summary>
    public class AGLCodeTestHttpTriggerFunction : FunctionBase, IAGLCodeTestHttpTriggerFunction
    {
        private readonly IAglPayloadLoadingService _loadingService;
        private readonly IAglPayloadProcessingService _processingService;

        private AglPayloadLoadingServiceOptions _loadingServiceOptions;
        private AglPayloadProcessingServiceOptions _processingServiceOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AGLCodeTestHttpTriggerFunction"/> class.
        /// </summary>
        /// <param name="loadingService"><see cref="IAglPayloadLoadingService"/> instance.</param>
        /// <param name="processingService"><see cref="IAglPayloadProcessingService"/> instance.</param>
        public AGLCodeTestHttpTriggerFunction(IAglPayloadLoadingService loadingService, IAglPayloadProcessingService processingService)
        {
            this._loadingService = loadingService.ThrowIfNullOrDefault();
            this._processingService = processingService.ThrowIfNullOrDefault();
        }

        /// <inheritdoc />
        public override async Task<object> InvokeAsync<TInput, TOptions>(TInput input, TOptions options = default(TOptions))
        {
            input.ThrowIfNullOrDefault();

            var req = input as HttpRequestMessage;
            req.ThrowIfNullOrDefault();

            options.ThrowIfNullOrDefault();

            var functionOptions = options as AGLCodeTestHttpTriggerFunctionOptions;
            functionOptions.ThrowIfNullOrDefault();

            // STEP #1: Load payload.
            this._loadingServiceOptions = new AglPayloadLoadingServiceOptions();

            await this._loadingService.InvokeAsync(this._loadingServiceOptions).ConfigureAwait(false);

            if (!this._loadingServiceOptions.IsInvoked)
            {
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, "Payload couldn't be loaded");
            }

            // STEP #2: Process payload.
            this._processingServiceOptions = new AglPayloadProcessingServiceOptions()
                                                 {
                                                     People = this._loadingServiceOptions.People,
                                                     PetType = functionOptions.PetType
                                                 };

            await this._processingService.InvokeAsync(this._processingServiceOptions).ConfigureAwait(false);

            if (!this._processingServiceOptions.IsInvoked)
            {
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, "Payload couldn't be processed");
            }

            // STEP #3: Create response.
            var html = new StringBuilder();
            html.AppendLine("<html><body>");
            html.AppendLine(string.Join(string.Empty, this._processingServiceOptions.Groups));
            html.AppendLine("</body></html>");

            var content = new StringContent(html.ToString(), Encoding.UTF8, "text/html");
            var res = new HttpResponseMessage(HttpStatusCode.OK) { Content = content };

            return res;
        }
    }
}
