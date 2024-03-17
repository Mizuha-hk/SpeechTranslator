using FluentAssertions;

namespace AppLogicTest;

public class LoggerTest
{
    [Fact]
    public void Constructor1()
    {
        AppLogger actual = CreateAppLogger();

        actual.Should().NotBeNull();
    }

    [Fact]
    public void Write_ValidMessage()
    {
        // Arrange
        var logger = CreateAppLogger();
        var message = "Test message";

        // Act
        logger.Write(message);
        logger.Close();

        // Assert
        Assert.Equal(message, logger.LogData[^1].Message);
    }

    [Fact]
    public void WriteLine_ValidMessage()
    {
        // Arrange
        var logger = CreateAppLogger();
        var message = "Test message";

        // Act
        logger.WriteLine(message);
        logger.Close();

        // Assert
        Assert.Equal(message + Environment.NewLine, logger.LogData[^1].Message);
    }

    [Fact]
    public void WriteLine_ValidObject()
    {
        // Arrange
        var logger = CreateAppLogger();
        var message = "Test message";

        // Act
        logger.WriteLine(message);
        logger.Close();

        // Assert
        Assert.Equal(message + Environment.NewLine, logger.LogData[^1].Message);
    }

    [Fact]
    public void Fail_ValidMessage()
    {
        // Arrange
        var logger = CreateAppLogger();
        var message = "Test message";

        // Act
        logger.Fail(message);
        logger.Close();

        // Assert
        Assert.Equal(message + Environment.NewLine, logger.LogData[^1].Message);
    }

    [Fact]
    public void WriteAndWriteLine_ValidMessage()
    {
        // Arrange
        var logger = CreateAppLogger();
        var writeMessage = "Test Write message";
        var writeLineMessage = "Test WriteLine message";

        // Act
        logger.Write(writeMessage);
        logger.WriteLine(writeLineMessage);
        logger.Close();

        // Assert
        Assert.Equal(writeMessage + writeLineMessage + Environment.NewLine, logger.LogData[^1].Message);
    }

    [Fact]
    public void WriteLineAfterClear_ValidMessage()
    {
        // Arrange
        var logger = CreateAppLogger();
        var message = "Test message";

        // Act
        logger.Clear();
        logger.WriteLine(message);
        logger.Close();

        // Assert
        Assert.Equal(message + Environment.NewLine, logger.LogData[^1].Message);
    }

    private static AppLogger CreateAppLogger()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return new AppLogger(path);
    }
}