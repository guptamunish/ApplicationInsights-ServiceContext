﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Microsoft.ApplicationInsights.ServiceContext
{
    public sealed class ServiceContextTelemetryIntitializer : ITelemetryInitializer
    {
        private const string NameEnvironmentVariable = "APPINSIGHTS_APP_NAME";
        private const string VersionEnvironmentVariable = "APPINSIGHTS_APP_VERSION";
        private const string ContextEnvironmentVariablePrefix = "APPINSIGHTS_APP_CONTEXT_";
        private readonly int contextPrefixLength = ContextEnvironmentVariablePrefix.Length;

        private readonly string name;
        private readonly string version;
        private readonly IReadOnlyDictionary<string, string> context;

        public ServiceContextTelemetryIntitializer() : this(
            GetEnvironmentVariable(NameEnvironmentVariable),
            GetEnvironmentVariable(VersionEnvironmentVariable),
            Environment.GetEnvironmentVariables())
        {
        }

        internal ServiceContextTelemetryIntitializer(string name, string version, IDictionary context)
        {
            this.name = name;
            this.version = version;
            this.context = ParseContext(context);
        }

        public void Initialize(ITelemetry telemetry)
        {
            // We assume that if env variable is set explicitly, it has higher priority than properties set on telemetry by SDK
            if (version != null)
            {
                telemetry.Context.Component.Version = version;
            }

            if (name != null)
            {
                telemetry.Context.Cloud.RoleName = name;
            }

            var properties = telemetry.Context.Properties;
            foreach (var kvp in context)
            {
                properties[kvp.Key] = kvp.Value;
            }
        }

        private IReadOnlyDictionary<string, string> ParseContext(IDictionary environment)
        {
            var ctx = new Dictionary<string, string>();

            if (environment != null)
            {
                foreach (var variable in environment.Keys)
                {
                    string varName = variable.ToString();
                    if (varName.StartsWith(ContextEnvironmentVariablePrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        varName = varName.Substring(contextPrefixLength);
                        if (!ctx.ContainsKey(varName))
                        {
                            ctx.Add(varName, environment[variable].ToString());
                        }
                    }
                }
            }

            return ctx;
        }

        private static string GetEnvironmentVariable(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
