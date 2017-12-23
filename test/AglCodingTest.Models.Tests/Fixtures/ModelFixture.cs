﻿using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace AGLCodeTest.Models.Tests.Fixtures
{
    /// <summary>
    /// This represents the fixture entity to test models.
    /// </summary>
    public class ModelFixture : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelFixture"/> class.
        /// </summary>
        public ModelFixture()
        {
            this.Fixture = new Fixture().Customize(new AutoMoqCustomization());
            this.JsonSerializerSettings = new JsonSerializerSettings
                                              {
                                                  ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                                  Converters = { new StringEnumConverter() },
                                                  Formatting = Formatting.Indented,
                                                  NullValueHandling = NullValueHandling.Ignore,
                                                  MissingMemberHandling = MissingMemberHandling.Ignore
                                              };
        }

        /// <summary>
        /// Gets the <see cref="IFixture"/> instance.
        /// </summary>
        public IFixture Fixture { get; }

        /// <summary>
        /// Gets the <see cref="Newtonsoft.Json.JsonSerializerSettings"/> instance.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            this._disposed = true;
        }
    }
}
