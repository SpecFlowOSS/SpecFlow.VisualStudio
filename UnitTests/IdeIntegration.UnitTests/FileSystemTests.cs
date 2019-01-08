using System.IO;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.IdeIntegration.Services;

namespace TechTalk.SpecFlow.IdeIntegration.UnitTests
{
    public class FileSystemTests
    {
        [Test]
        public void AppendDirectorySeparatorIfNotPresent_PathWithAltSeparatorAtEnd_ShouldReturnSamePath()
        {
            // ARRANGE
            string inputPath = @"C:" + Path.AltDirectorySeparatorChar + "PathToDirectory" + Path.AltDirectorySeparatorChar;
            string expectedPath = @"C:" + Path.AltDirectorySeparatorChar + "PathToDirectory" + Path.AltDirectorySeparatorChar;
            var fileSystem = new FileSystem();

            // ACT
            string actualPath = fileSystem.AppendDirectorySeparatorIfNotPresent(inputPath);

            // ASSERT
            actualPath.Should().Be(expectedPath);
        }

        [Test]
        public void AppendDirectorySeparatorIfNotPresent_PathWithSeparatorAtEnd_ShouldReturnSamePath()
        {
            // ARRANGE
            string inputPath = @"C:" + Path.DirectorySeparatorChar + "PathToDirectory" + Path.DirectorySeparatorChar;
            string expectedPath = @"C:" + Path.DirectorySeparatorChar + "PathToDirectory" + Path.DirectorySeparatorChar;
            var fileSystem = new FileSystem();

            // ACT
            string actualPath = fileSystem.AppendDirectorySeparatorIfNotPresent(inputPath);

            // ASSERT
            actualPath.Should().Be(expectedPath);
        }

        [Test]
        public void AppendDirectorySeparatorIfNotPresent_PathWithAltSeparator_ShouldReturnPathWithAltSeparatorAtEnd()
        {
            // ARRANGE
            string inputPath = @"C:" + Path.AltDirectorySeparatorChar + "PathToDirectory";
            string expectedPath = @"C:" + Path.AltDirectorySeparatorChar + "PathToDirectory" + Path.AltDirectorySeparatorChar;
            var fileSystem = new FileSystem();

            // ACT
            string actualPath = fileSystem.AppendDirectorySeparatorIfNotPresent(inputPath);

            // ASSERT
            actualPath.Should().Be(expectedPath);
        }

        [Test]
        public void AppendDirectorySeparatorIfNotPresent_PathWithSeparator_ShouldReturnPathWithSeparatorAtEnd()
        {
            // ARRANGE
            string inputPath = @"C:" + Path.DirectorySeparatorChar + "PathToDirectory";
            string expectedPath = @"C:" + Path.DirectorySeparatorChar + "PathToDirectory" + Path.DirectorySeparatorChar;
            var fileSystem = new FileSystem();

            // ACT
            string actualPath = fileSystem.AppendDirectorySeparatorIfNotPresent(inputPath);

            // ASSERT
            actualPath.Should().Be(expectedPath);
        }

        [Test]
        public void AppendDirectorySeparatorIfNotPresent_PathWithNoSeparator_ShouldReturnPathWithSeparatorAtEnd()
        {
            // ARRANGE
            string inputPath = @"PathToDirectory";
            string expectedPath = @"PathToDirectory" + Path.DirectorySeparatorChar;
            var fileSystem = new FileSystem();

            // ACT
            string actualPath = fileSystem.AppendDirectorySeparatorIfNotPresent(inputPath);

            // ASSERT
            actualPath.Should().Be(expectedPath);
        }

        [Test]
        public void AppendDirectorySeparatorIfNotPresent_PathWithSeparator_ShouldIgnoreWhitespaceAtEnd()
        {
            // ARRANGE
            string inputPath = @"C:" + Path.DirectorySeparatorChar + "PathToDirectory        ";
            string expectedPath = @"C:" + Path.DirectorySeparatorChar + "PathToDirectory" + Path.DirectorySeparatorChar;
            var fileSystem = new FileSystem();

            // ACT
            string actualPath = fileSystem.AppendDirectorySeparatorIfNotPresent(inputPath);

            // ASSERT
            actualPath.Should().Be(expectedPath);
        }
    }
}
