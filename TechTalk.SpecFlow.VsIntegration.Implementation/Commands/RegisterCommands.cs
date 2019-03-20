﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BoDi;
using TechTalk.SpecFlow.VsIntegration;
using TechTalk.SpecFlow.VsIntegration.Commands;

namespace TechTalk.SpecFlow.VsIntegration.Implementation
{
    public partial class DefaultDependencyProvider
    {
        static partial void RegisterCommands(IObjectContainer container)
        {
            container.RegisterTypeAs<ReGenerateAllCommand, MenuCommandHandler>(SpecFlowCmdSet.ReGenerateAll.ToString());
            container.RegisterTypeAs<ContextDependentNavigationCommand, MenuCommandHandler>(SpecFlowCmdSet.ContextDependentNavigation.ToString());
            container.RegisterTypeAs<GenerateStepDefinitionSkeletonCommand, MenuCommandHandler>(SpecFlowCmdSet.GenerateStepDefinitionSkeleton.ToString());

            // internal commands
            container.RegisterTypeAs<GoToStepsCommand, IGoToStepsCommand>();
            container.RegisterTypeAs<GoToStepDefinitionCommand, IGoToStepDefinitionCommand>();
        }
    }
}
