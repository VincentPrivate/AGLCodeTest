using System;
using System.Threading.Tasks;

using AGLCodeTest.Services.ServiceOptions;

namespace AGLCodeTest.Services
{
    /// <summary>
    /// This provides interfaces to all classes in the service layer.
    /// </summary>
    public interface IService : IDisposable
    {
        /// <summary>
        /// Invokes the service.
        /// </summary>
        /// <typeparam name="TOptions">Type of service options.</typeparam>
        /// <param name="options"><see cref="TOptions"/> instance.</param>
        /// <returns>Returns <see cref="Task"/>.</returns>
        Task InvokeAsync<TOptions>(TOptions options)
            where TOptions : ServiceOptionsBase;
    }
}