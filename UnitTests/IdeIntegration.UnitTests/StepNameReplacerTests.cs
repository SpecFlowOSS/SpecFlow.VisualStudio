using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.IdeIntegration.EditorCommands;

namespace TechTalk.SpecFlow.IdeIntegration.UnitTests
{
    public class StepNameReplacerTests
    {
        [TestCase("the hungarian calculator", @"the hungarian calculator", "the russian calculator", "the russian calculator")]
        [TestCase("the first number is 50", @"the first number is (.*)", "the number is (.*)", "the number is 50")]
        [TestCase("the first number is 50", @"the first number is (\d+)", @"the number is (\d+)", "the number is 50")]
        [TestCase("The car should be painted blue", @"The car should be painted (blue|yellow|green)", @"The car is (blue|yellow|green)", "The car is blue")]
        public void Should_CalculateCorrectStepName_WhenParametersAreTheSame(string stepName, string originalStepRegex, string newRegex, string expectedStepName)
        {
            var binding = CreateStepDefinitionBinding(originalStepRegex);

            var sut = new StepNameReplacer();
            var result = sut.BuildStepNameWithNewRegex(stepName, newRegex, binding);

            result.Should().Be(expectedStepName);
        }

        [TestCase("my 10 and your 20", @"my (.*) and your (.*)", @"your and mine: (.*), (.*)", "your and mine: 10, 20")]
        [TestCase("my 10 and your 20", @"my (\d+) and your (\d+)", @"your and mine: (\d+), (\d+)", "your and mine: 10, 20")]
        public void Should_CalculateCorrectStepName_WhenReorderingTheParameters(string stepName, string originalStepRegex, string newRegex, string expectedStepName)
        {
            var binding = CreateStepDefinitionBinding(originalStepRegex);

            var sut = new StepNameReplacer();
            var result = sut.BuildStepNameWithNewRegex(stepName, newRegex, binding);

            result.Should().Be(expectedStepName);
        }

        [Test]
        public void Should_ThrowAnException_WhenChangingTheNumberOfParameters()
        {
            var stepName = "the number is 50";
            var originalRegex = "the number is (.*)";
            var newRegex = "the numbers are (.*) and (.*)";
            var binding = CreateStepDefinitionBinding(originalRegex);

            var sut = new StepNameReplacer();

            Assert.Throws<NotSupportedException>(() =>
                    sut.BuildStepNameWithNewRegex(stepName, newRegex, binding));
        }

        private StepDefinitionBinding CreateStepDefinitionBinding(string regex)
        {
            var bindingType = new BindingType("StepNameReplacerTests", "UnitTests.StepNameReplacerTests");
            var bindingScope = new BindingScope(null, null, null);
            var bindingMethod = new BindingMethod(bindingType, "", Enumerable.Empty<IBindingParameter>(), bindingType);

            var binding = new StepDefinitionBinding(StepDefinitionType.Given, regex, bindingMethod, bindingScope);

            return binding;
        }
    }
}
