// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.DataContracts;
using Xunit;

namespace Microsoft.ApplicationInsights.ServiceContext.Tests
{
    public class ServiceContextTelemetryIntitializerTests : IDisposable
    {
        public void Dispose()
        {
            Environment.SetEnvironmentVariable("APPINSIGHTS_APP_NAME", null);
            Environment.SetEnvironmentVariable("APPINSIGHTS_APP_VERSION", null);
            Environment.SetEnvironmentVariable("APPINSIGHTS_APP_CONTEXT_a", null);
            Environment.SetEnvironmentVariable("APPINSIGHTS_APP_CONTEXT_b", null);
        }

        [Fact]
        public void InitializerSetsPropertiesFromEnvVariables()
        {
            Environment.SetEnvironmentVariable("APPINSIGHTS_APP_NAME", "service-a");
            Environment.SetEnvironmentVariable("APPINSIGHTS_APP_VERSION", "1.2.3");
            Environment.SetEnvironmentVariable("APPINSIGHTS_APP_CONTEXT_a", "b");
            Environment.SetEnvironmentVariable("APPINSIGHTS_APP_CONTEXT_c", "d");

            var initializer = new ServiceContextTelemetryIntitializer();

            var traceTelemetry = new TraceTelemetry();
            initializer.Initialize(traceTelemetry);

            Assert.Equal("service-a", traceTelemetry.Context.Cloud.RoleName);
            Assert.Equal("1.2.3", traceTelemetry.Context.Component.Version);

            Assert.Contains(traceTelemetry.Context.Properties, item => item.Key == "a" && item.Value == "b");
            Assert.Contains(traceTelemetry.Context.Properties, item => item.Key == "c" && item.Value == "d");

            Assert.Equal(2, traceTelemetry.Context.Properties.Count);
        }

        [Fact]
        public void InitializerSetsProperties()
        {
            IDictionary context = new Dictionary<string,string> {{ "APPINSIGHTS_APP_CONTEXT_a", "b"}, { "APPINSIGHTS_APP_CONTEXT_c", "d" } };
            var initializer = new ServiceContextTelemetryIntitializer("service-a", "1.2.3", context);

            var traceTelemetry = new TraceTelemetry();
            initializer.Initialize(traceTelemetry);

            Assert.Equal("service-a", traceTelemetry.Context.Cloud.RoleName);
            Assert.Equal("1.2.3", traceTelemetry.Context.Component.Version);

            Assert.Contains(traceTelemetry.Context.Properties, item => item.Key == "a" && item.Value == "b" );
            Assert.Contains(traceTelemetry.Context.Properties, item => item.Key == "c" && item.Value == "d");

            Assert.Equal(2, traceTelemetry.Context.Properties.Count);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void InitializerDoesNotSetMissingProperties(string value)
        {
            var initializer = new ServiceContextTelemetryIntitializer(value, value, value == null ? null : new Dictionary<string,string>());

            var traceTelemetry = new TraceTelemetry();
            traceTelemetry.Context.Cloud.RoleName = "service-a";
            traceTelemetry.Context.Component.Version = "1.0.0.0";
            traceTelemetry.Properties.Add("a", "x");
            initializer.Initialize(traceTelemetry);

            Assert.Equal("service-a", traceTelemetry.Context.Cloud.RoleName);
            Assert.Equal("1.0.0.0", traceTelemetry.Context.Component.Version);

            Assert.Contains(traceTelemetry.Context.Properties, item => item.Key == "a" && item.Value == "x");
            Assert.Equal(1, traceTelemetry.Context.Properties.Count);
        }


        [Fact]
        public void InitializerOverwritesProperties()
        {
            IDictionary context = new Dictionary<string, string> { { "APPINSIGHTS_APP_CONTEXT_a", "b" }, { "APPINSIGHTS_APP_CONTEXT_c", "d" } };
            var initializer = new ServiceContextTelemetryIntitializer("service-a", "1.2.3", context);

            var traceTelemetry = new TraceTelemetry();
            traceTelemetry.Context.Cloud.RoleName = "service-b";
            traceTelemetry.Context.Component.Version = "1.0.0.0";
            traceTelemetry.Properties.Add("a", "x");
            initializer.Initialize(traceTelemetry);

            Assert.Equal("service-a", traceTelemetry.Context.Cloud.RoleName);
            Assert.Equal("1.2.3", traceTelemetry.Context.Component.Version);

            Assert.Contains(traceTelemetry.Context.Properties, item => item.Key == "a" && item.Value == "b");
            Assert.Contains(traceTelemetry.Context.Properties, item => item.Key == "c" && item.Value == "d");

            Assert.Equal(2, traceTelemetry.Context.Properties.Count);
        }
    }
}