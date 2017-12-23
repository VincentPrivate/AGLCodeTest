using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using AGLCodeTest.Functions.FunctionOptions;
using AGLCodeTest.Functions.Tests.Fixtures;
using AGLCodeTest.Models;
using AGLCodeTest.Services.ServiceOptions;

using FluentAssertions;

using Moq;

using Ploeh.AutoFixture.Xunit2;

using Xunit;

namespace AGLCodeTest.Functions.Tests
{
    /// <summary>
    /// This represents the test entity for <see cref="AGLCodeTestHttpTriggerFunction"/>
    /// </summary>
    public class AGLCodeTestHttpTriggerFunctionTests : IClassFixture<FunctionFixture>
    {
        private readonly FunctionFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="AGLCodeTestHttpTriggerFunctionTests"/> class.
        /// </summary>
        /// <param name="fixture"><see cref="FunctionFixture"/> instance.</param>
        public AGLCodeTestHttpTriggerFunctionTests(FunctionFixture fixture)
        {
            this._fixture = fixture;
        }

        /// <summary>
        /// Tests whether the constructor should throw an exception or not.
        /// </summary>
        [Fact]
        public void Given_NullParameter_Constructor_ShouldThrow_Exception()
        {
            var loadingService = this._fixture.ArrangeLoadingService();

            Action action = () => new AGLCodeTestHttpTriggerFunction(null, null);
            action.ShouldThrow<ArgumentNullException>();

            action = () => new AGLCodeTestHttpTriggerFunction(loadingService.Object, null);
            action.ShouldThrow<ArgumentNullException>();
        }

        /// <summary>
        /// Tests whether the method should throw an exception or not.
        /// </summary>
        [Fact]
        public void Given_NullInput_InvokeAsync_ShouldThrow_Exception()
        {
            var loadingService = this._fixture.ArrangeLoadingService();
            var processingService = this._fixture.ArrangeProcessingService();

            var service = new AGLCodeTestHttpTriggerFunction(loadingService.Object, processingService.Object);

            Func<Task> func = async () => await service.InvokeAsync<HttpRequestMessage, FunctionOptionsBase>(null).ConfigureAwait(false);
            func.ShouldThrow<ArgumentNullException>();
        }

        /// <summary>
        /// Tests whether the method should throw an exception or not.
        /// </summary>
        [Fact]
        public void Given_InvalidInput_InvokeAsync_ShouldThrow_Exception()
        {
            var loadingService = this._fixture.ArrangeLoadingService();
            var processingService = this._fixture.ArrangeProcessingService();

            var service = new AGLCodeTestHttpTriggerFunction(loadingService.Object, processingService.Object);

            var input = new object();

            Func<Task> func = async () => await service.InvokeAsync<object, FunctionOptionsBase>(input).ConfigureAwait(false);
            func.ShouldThrow<ArgumentNullException>();
        }

        /// <summary>
        /// Tests whether the method should throw an exception or not.
        /// </summary>
        [Fact]
        public void Given_NullFunctionOptions_InvokeAsync_ShouldThrow_Exception()
        {
            var loadingService = this._fixture.ArrangeLoadingService();
            var processingService = this._fixture.ArrangeProcessingService();

            var service = new AGLCodeTestHttpTriggerFunction(loadingService.Object, processingService.Object);

            var input = new HttpRequestMessage();

            Func<Task> func = async () => await service.InvokeAsync<HttpRequestMessage, FunctionOptionsBase>(input, null).ConfigureAwait(false);
            func.ShouldThrow<ArgumentNullException>();
        }

        /// <summary>
        /// Tests whether the method should throw an exception or not.
        /// </summary>
        [Fact]
        public void Given_InvalidFunctionOptions_InvokeAsync_ShouldThrow_Exception()
        {
            var loadingService = this._fixture.ArrangeLoadingService();
            var processingService = this._fixture.ArrangeProcessingService();

            var service = new AGLCodeTestHttpTriggerFunction(loadingService.Object, processingService.Object);

            var input = new HttpRequestMessage();
            var options = new FooFunctionOptions();

            Func<Task> func = async () => await service.InvokeAsync<HttpRequestMessage, FunctionOptionsBase>(input, options).ConfigureAwait(false);
            func.ShouldThrow<ArgumentNullException>();
        }

        /// <summary>
        /// Tests whether the method should return error response or not.
        /// </summary>
        /// <param name="petType"><see cref="PetType"/> value.</param>
        [Theory]
        [AutoData]
        public async void Given_LoadingService_When_InvocationFail_InvokeAsync_ShouldReturn_ErrorResponse(PetType petType)
        {
            var loadingService = this._fixture.ArrangeLoadingService();
            var processingService = this._fixture.ArrangeProcessingService();

            var function = new AGLCodeTestHttpTriggerFunction(loadingService.Object, processingService.Object);

            var input = new HttpRequestMessage();
            var options = this._fixture.ArrangeAGLCodeTestHttpTriggerFunctionOptions(petType);

            loadingService.Setup(p => p.InvokeAsync(It.IsAny<AglPayloadLoadingServiceOptions>()))
                          .Returns(Task.CompletedTask)
                          .Callback<AglPayloadLoadingServiceOptions>(p => p.IsInvoked = false);

            var res = await function.InvokeAsync<HttpRequestMessage, FunctionOptionsBase>(input, options).ConfigureAwait(false);

            res.Should().BeOfType<HttpResponseMessage>()
               .Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            var loadingServiceOptions = typeof(AGLCodeTestHttpTriggerFunction)
                                            .GetField("_loadingServiceOptions", BindingFlags.Instance | BindingFlags.NonPublic)
                                            .GetValue(function) as AglPayloadLoadingServiceOptions;

            loadingServiceOptions.IsInvoked.Should().BeFalse();
        }

        /// <summary>
        /// Tests whether the method should return error response or not.
        /// </summary>
        /// <param name="petType"><see cref="PetType"/> value.</param>
        [Theory]
        [AutoData]
        public async void Given_ProcessingService_When_InvocationFail_InvokeAsync_ShouldReturn_ErrorResponse(PetType petType)
        {
            var loadingService = this._fixture.ArrangeLoadingService();
            var processingService = this._fixture.ArrangeProcessingService();

            var function = new AGLCodeTestHttpTriggerFunction(loadingService.Object, processingService.Object);

            var input = new HttpRequestMessage();
            var options = this._fixture.ArrangeAGLCodeTestHttpTriggerFunctionOptions(petType);

            var people = this._fixture.ArrangePeople();

            loadingService.Setup(p => p.InvokeAsync(It.IsAny<AglPayloadLoadingServiceOptions>()))
                          .Returns(Task.CompletedTask)
                          .Callback<AglPayloadLoadingServiceOptions>(p =>
                                                                         {
                                                                             p.People = people;
                                                                             p.IsInvoked = true;
                                                                         });

            processingService.Setup(p => p.InvokeAsync(It.IsAny<AglPayloadProcessingServiceOptions>()))
                             .Returns(Task.CompletedTask)
                             .Callback<AglPayloadProcessingServiceOptions>(p => p.IsInvoked = false);

            var res = await function.InvokeAsync<HttpRequestMessage, FunctionOptionsBase>(input, options).ConfigureAwait(false);

            res.Should().BeOfType<HttpResponseMessage>()
               .Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            var loadingServiceOptions = typeof(AGLCodeTestHttpTriggerFunction)
                                            .GetField("_loadingServiceOptions", BindingFlags.Instance | BindingFlags.NonPublic)
                                            .GetValue(function) as AglPayloadLoadingServiceOptions;

            loadingServiceOptions.IsInvoked.Should().BeTrue();
            loadingServiceOptions.People.Should().HaveCount(people.Count);

            var processingServiceOptions = typeof(AGLCodeTestHttpTriggerFunction)
                                               .GetField("_processingServiceOptions", BindingFlags.Instance | BindingFlags.NonPublic)
                                               .GetValue(function) as AglPayloadProcessingServiceOptions;

            processingServiceOptions.IsInvoked.Should().BeFalse();
        }

        /// <summary>
        /// Tests whether the method should return result or not.
        /// </summary>
        /// <param name="petType"><see cref="PetType"/> value.</param>
        [Theory]
        [AutoData]
        public async void Given_ProcessingService_When_InvocationFail_InvokeAsync_ShouldReturn_Result(PetType petType)
        {
            var loadingService = this._fixture.ArrangeLoadingService();
            var processingService = this._fixture.ArrangeProcessingService();

            var function = new AGLCodeTestHttpTriggerFunction(loadingService.Object, processingService.Object);

            var input = this._fixture.ArrangeHttpRequestMessage();
            var options = this._fixture.ArrangeAGLCodeTestHttpTriggerFunctionOptions(petType);

            var people = this._fixture.ArrangePeople();

            loadingService.Setup(p => p.InvokeAsync(It.IsAny<AglPayloadLoadingServiceOptions>()))
                          .Returns(Task.CompletedTask)
                          .Callback<AglPayloadLoadingServiceOptions>(p =>
                                                                         {
                                                                             p.People = people;
                                                                             p.IsInvoked = true;
                                                                         });

            var groups = this._fixture.ArrangeGroups();

            processingService.Setup(p => p.InvokeAsync(It.IsAny<AglPayloadProcessingServiceOptions>()))
                             .Returns(Task.CompletedTask)
                             .Callback<AglPayloadProcessingServiceOptions>(p =>
                                                                               {
                                                                                   p.Groups = groups;
                                                                                   p.IsInvoked = true;
                                                                               });

            var res = await function.InvokeAsync<HttpRequestMessage, FunctionOptionsBase>(input, options).ConfigureAwait(false);

            var result = res.Should().BeOfType<HttpResponseMessage>().Which;
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.Headers.ContentType.MediaType.Should().BeEquivalentTo("text/html");

            var loadingServiceOptions = typeof(AGLCodeTestHttpTriggerFunction)
                                            .GetField("_loadingServiceOptions", BindingFlags.Instance | BindingFlags.NonPublic)
                                            .GetValue(function) as AglPayloadLoadingServiceOptions;

            loadingServiceOptions.IsInvoked.Should().BeTrue();
            loadingServiceOptions.People.Should().HaveCount(people.Count);

            var processingServiceOptions = typeof(AGLCodeTestHttpTriggerFunction)
                                               .GetField("_processingServiceOptions", BindingFlags.Instance | BindingFlags.NonPublic)
                                               .GetValue(function) as AglPayloadProcessingServiceOptions;

            processingServiceOptions.IsInvoked.Should().BeTrue();
            processingServiceOptions.Groups.Should().HaveCount(groups.Count);
        }
    }
}
