﻿using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public class EnvironmentSpecFlowTelemetryChecker : IEnvironmentSpecFlowTelemetryChecker
    {
        public const string SpecFlowTelemetryEnvironmentVariable = "SPECFLOW_TELEMETRY_ENABLED";

        public bool IsSpecFlowTelemetryEnabled()
        {
            var specFlowTelemetry = Environment.GetEnvironmentVariable(SpecFlowTelemetryEnvironmentVariable);
            return specFlowTelemetry == null || specFlowTelemetry.Equals("1");
        }
    }
}
