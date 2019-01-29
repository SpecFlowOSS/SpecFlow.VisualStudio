using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.IdeIntegration.Services;

namespace TechTalk.SpecFlow.IdeIntegration.UnitTests
{
    public class FileSystemTests
    {
        [TestCase(@"C:/PathToDirectory/", @"C:/PathToDirectory/")]
        [TestCase(@"C:/PathToDirectory", @"C:/PathToDirectory/")]
        [TestCase(@"C:\PathToDirectory\", @"C:\PathToDirectory\")]
        [TestCase(@"C:\PathToDirectory", @"C:\PathToDirectory\")]
        [TestCase(@"C:\PathToDirectory      ", @"C:\PathToDirectory\")]
        [TestCase(@"PathToDirectory", @"PathToDirectory\")]
        public void AppendDirectorySeparatorIfNotPresent_Path_ShouldReturnCorrectPath(string inputPath, string expectedPath)
        {
            // ARRANGE
            var fileSystem = new FileSystem();

            // ACT
            string actualPath = fileSystem.AppendDirectorySeparatorIfNotPresent(inputPath);

            // ASSERT
            actualPath.Should().Be(expectedPath);
        }
    }
}
