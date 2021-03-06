﻿using System;

using AGLCodeTest.Extensions.Tests.Fixtures;

using FluentAssertions;

using Ploeh.AutoFixture;

using Xunit;

namespace AGLCodeTest.Extensions.Tests
{
    /// <summary>
    /// This represents the test entity for the <see cref="GenericExtensions"/> class.
    /// </summary>
    public class GenericExtensionsTests : IClassFixture<ExtensionFixture>
    {
        private readonly IFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericExtensionsTests"/> class.
        /// </summary>
        /// <param name="fixture"><see cref="ExtensionFixture"/> instance.</param>
        public GenericExtensionsTests(ExtensionFixture fixture)
        {
            this._fixture = fixture.Fixture;
        }

        /// <summary>
        /// Tests the method whether to return <c>True</c> or not.
        /// </summary>
        [Fact]
        public void Given_Null_IsNullOrDefault_ShouldReturn_True()
        {
            var result = GenericExtensions.IsNullOrDefault((object)null);
            result.Should().BeTrue();

            result = GenericExtensions.IsNullOrDefault(default(object));
            result.Should().BeTrue();
        }

        /// <summary>
        /// Tests the method whether to return <c>False</c> or not.
        /// </summary>
        [Fact]
        public void Given_Value_IsNullOrDefault_ShouldReturn_False()
        {
            var result = GenericExtensions.IsNullOrDefault(new object());
            result.Should().BeFalse();
        }

        /// <summary>
        /// Tests the method whether to throw an exception or not.
        /// </summary>
        [Fact]
        public void Given_Null_ThrowIfNullOrDefault_ShouldThrow_Exception()
        {
            Action action = () => GenericExtensions.ThrowIfNullOrDefault((object)null);
            action.ShouldThrow<ArgumentNullException>();

            action = () => GenericExtensions.ThrowIfNullOrDefault(default(object));
            action.ShouldThrow<ArgumentNullException>();
        }

        /// <summary>
        /// Tests the method whether to return result or not.
        /// </summary>
        [Fact]
        public void Given_Value_ThrowIfNullOrDefault_ShouldReturn_Object()
        {
            var result = GenericExtensions.ThrowIfNullOrDefault(new object());
            result.Should().BeOfType<object>();
        }
    }
}