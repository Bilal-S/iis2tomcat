using System;
using System.IO;
using System.Threading;
using Xunit;
using BonCodeAJP13;

namespace Connector.Tests.BonCodeAJP13
{
    /// <summary>
    /// Unit tests for BonCodeAJP13Logger class
    /// </summary>
    public class LoggerTests : IDisposable
    {
        private readonly Mutex _testMutex;
        private readonly string _testLogDir;
        private readonly string _testLogFileName;

        public LoggerTests()
        {
            // Create a unique mutex for testing
            _testMutex = new Mutex(false, "BonCodeTestMutex_" + Guid.NewGuid().ToString());
            
            // Create a temp directory for test logs
            _testLogDir = Path.Combine(Path.GetTempPath(), "BonCodeTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testLogDir);
            _testLogFileName = Path.Combine(_testLogDir, "test.log");
        }

        public void Dispose()
        {
            // Cleanup test directory
            try
            {
                if (Directory.Exists(_testLogDir))
                {
                    Directory.Delete(_testLogDir, true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
            
            _testMutex?.Dispose();
        }

        [Fact]
        public void Constructor_WithNullMutex_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new BonCodeAJP13Logger("test.log", null));
        }

        [Fact]
        public void Constructor_WithNullFileName_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new BonCodeAJP13Logger(null, _testMutex));
        }

        [Fact]
        public void Constructor_WithEmptyFileName_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new BonCodeAJP13Logger("", _testMutex));
        }

        [Fact]
        public void Constructor_WithValidParameters_CreatesLogFile()
        {
            // Arrange
            var expectedDateSuffix = DateTime.Now.ToString("yyyyMMdd");
            var expectedFileName = Path.Combine(_testLogDir, "test-" + expectedDateSuffix + ".log");

            // Temporarily override the log directory setting
            var originalLogDir = BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR;
            BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR = _testLogDir;

            try
            {
                // Ensure logging is enabled
                BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL = BonCodeAJP13LogLevels.BONCODEAJP13_LOG_DEBUG;

                // Act
                var logger = new BonCodeAJP13Logger(_testLogFileName, _testMutex);

                // Assert
                Assert.True(File.Exists(expectedFileName), $"Log file should be created at {expectedFileName}");
                
                // Verify the file contains the version header
                var content = File.ReadAllText(expectedFileName);
                Assert.Contains("BonCode AJP Connector version", content);
            }
            finally
            {
                BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR = originalLogDir;
            }
        }

        [Fact]
        public void LogMessage_WhenLoggingEnabled_WritesToLogFile()
        {
            // Arrange
            var originalLogDir = BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR;
            BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR = _testLogDir;
            BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL = BonCodeAJP13LogLevels.BONCODEAJP13_LOG_DEBUG;

            try
            {
                var logger = new BonCodeAJP13Logger(_testLogFileName, _testMutex);
                var testMessage = "Test message " + Guid.NewGuid().ToString();

                // Act
                logger.LogMessage(testMessage, BonCodeAJP13LogLevels.BONCODEAJP13_LOG_DEBUG);

                // Assert
                var expectedDateSuffix = DateTime.Now.ToString("yyyyMMdd");
                var expectedFileName = Path.Combine(_testLogDir, "test-" + expectedDateSuffix + ".log");
                var content = File.ReadAllText(expectedFileName);
                Assert.Contains(testMessage, content);
            }
            finally
            {
                BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR = originalLogDir;
            }
        }

        [Fact]
        public void LogMessage_WhenLoggingDisabled_DoesNotWriteToFile()
        {
            // Arrange
            var originalLogDir = BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR;
            var originalLogLevel = BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL;
            BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR = _testLogDir;
            BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL = BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG;

            try
            {
                var logger = new BonCodeAJP13Logger(_testLogFileName, _testMutex);
                var testMessage = "This should not be logged " + Guid.NewGuid().ToString();

                // Act
                logger.LogMessage(testMessage, BonCodeAJP13LogLevels.BONCODEAJP13_LOG_DEBUG);

                // Assert - message should not be in file
                var expectedDateSuffix = DateTime.Now.ToString("yyyyMMdd");
                var expectedFileName = Path.Combine(_testLogDir, "test-" + expectedDateSuffix + ".log");
                
                if (File.Exists(expectedFileName))
                {
                    var content = File.ReadAllText(expectedFileName);
                    Assert.DoesNotContain(testMessage, content);
                }
            }
            finally
            {
                BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR = originalLogDir;
                BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL = originalLogLevel;
            }
        }

        [Fact]
        public void GetLogDir_WhenCustomDirSet_ReturnsCustomDir()
        {
            // Arrange
            var originalLogDir = BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR;
            BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR = _testLogDir;

            try
            {
                // Act
                var result = BonCodeAJP13Logger.GetLogDir();

                // Assert
                Assert.Equal(_testLogDir, result);
            }
            finally
            {
                BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR = originalLogDir;
            }
        }

        [Fact]
        public void GetAssemblyDirectory_ReturnsValidDirectory()
        {
            // Act
            var result = BonCodeAJP13Logger.GetAssemblyDirectory();

            // Assert
            Assert.NotNull(result);
            Assert.True(Directory.Exists(result), $"Directory should exist: {result}");
        }
    }
}