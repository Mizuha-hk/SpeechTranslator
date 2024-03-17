using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using static SpeechTranslator.Services.IAppLogger;

namespace SpeechTranslator.Services;

public enum LogType
{
    Trace,
    Error
}

public class LogData
{
    private readonly string _logTimeFormat = "yyyy-MM-dd HH:mm:ss";

    public LogType Type { get; set; } = LogType.Trace;

    public string Time { get; set; }

    public string Message { get; set; }

    public LogData(LogType type, string? message)
    {
        Type = type;

        Message = message ?? string.Empty;

        Time = DateTime.Now.ToString(_logTimeFormat);
    }
}

public class LogDataChangedEventArgs(LogType type, string logData)
{
    public LogType Type { get; set; } = type;
    public string LogData { get; set; } = logData;
}

public class AppLogger: TraceListener, IAppLogger
{
    private DateTime _logTime;
    private readonly string _dateFormat = "yyyy-MM-dd";
    private int _suffix = 0;
    private string _logFileName;
    private string _logDirectory;
    private DirectoryInfo _logDirInfo;
    private string _logFilePath;
    private readonly long _maxLogFileSize = 1024 * 1024 * 10; // 10MB
    private readonly object _lock = new();

    public AppLogger(string appDataPath)
    {
        LogData.CollectionChanged += (sender, e) =>
        {
            if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                return;
            }

            if (e.Action != NotifyCollectionChangedAction.Add || e.NewItems == null)
            {
                return;
            }

            foreach (LogData logData in e.NewItems)
            {
                LogDataChanged?.Invoke(this, new LogDataChangedEventArgs(logData.Type, logData.Message));
            }
        };

        _logTime = DateTime.Now;
        _logDirectory = Path.Combine(appDataPath, "logs", _logTime.ToString(_dateFormat));
        _logFileName = ResolveLogFileName();
        _logFilePath = _logFileName = Path.Combine(_logDirectory, _logFileName);

        _logDirInfo = Directory.CreateDirectory(_logDirectory);

        LogData.Add(new LogData(LogType.Trace, "Logger initialized." + Environment.NewLine));
    }

    public ObservableCollection<LogData> LogData { get; } = [];

    public event LogDataChangedEventHandler? LogDataChanged;

    public override void Write(string? message)
    {
        lock (_lock)
        {
            if(LogData.Count == 0)
            {
                LogData.Add(new LogData(LogType.Trace, message ?? string.Empty));
                return;
            }

            var recentLog = LogData[^1];

            if(recentLog.Message.IndexOf(Environment.NewLine) > 0)
            {
                LogData.Add(new LogData(LogType.Trace, message ?? string.Empty));
            }
            else
            {
                recentLog.Message += message;
            }
        }
    }

    public override void WriteLine(string? message)
    {
        lock (_lock)
        {
            if(LogData.Count == 0)
            {
                LogData.Add(new LogData(LogType.Trace, $"{message ?? string.Empty}{Environment.NewLine}"));
                return;
            }

            var recentLog = LogData[^1];

            if(recentLog.Message.IndexOf(Environment.NewLine) > 0)
            {
                LogData.Add(new LogData(LogType.Trace, $"{message ?? string.Empty}{Environment.NewLine}"));
                WriteToLogFile();
            }
            else
            {
                recentLog.Message += $"{message}{Environment.NewLine}";
                WriteToLogFile();
            }
        }
    }

    public override void WriteLine(object? obj)
    {
        lock (_lock)
        {
            if(LogData.Count == 0)
            {
                LogData.Add(new LogData(LogType.Trace, $"{obj?.ToString() ?? string.Empty}{Environment.NewLine}"));
                return;
            }

            var recentLog = LogData[^1];

            if(recentLog.Message.IndexOf(Environment.NewLine) > 0)
            {
                LogData.Add(new LogData(LogType.Trace, $"{obj?.ToString() ?? string.Empty} {Environment.NewLine}"));
                WriteToLogFile();
            }
            else
            {
                recentLog.Message += $"{obj?.ToString() ?? string.Empty}{Environment.NewLine}";
                WriteToLogFile();
            }
        }
    }

    public override void Fail(string? message)
    {
        lock (_lock)
        {
            LogData.Add(new LogData(LogType.Error, $"{message ?? string.Empty}{Environment.NewLine}"));
            WriteToLogFile();
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            if(LogData.Count == 0)
            {
                return;
            }

            this.WriteLine("");
            LogData.Clear();
        }
    }

    public override void Close()
    {
        lock (_lock)
        {
            var logData = LogData[^1];
            var logLine = $"{logData.Time}\t{logData.Type}: {logData.Message}{Environment.NewLine}";

            var buffer = Encoding.UTF8.GetBytes(logLine);

            using var stream = File.Open(_logFilePath, FileMode.Append);
            stream.Write(buffer, 0, buffer.Length);

            stream?.Close();
        }
    }

    public override void Flush()
    {
        lock (_lock)
        {
            using var stream = File.Open(_logFilePath, FileMode.Append);
            stream?.Flush();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            using var stream = File.Open(_logFilePath, FileMode.Append);
            stream?.Dispose();
        }
        base.Dispose(disposing);
    }

    private bool IsValidLogFile(string filePath)
    {
        if(!File.Exists(filePath))
        {
            return true;
        }

        var fileInfo = new FileInfo(filePath);
        return fileInfo.Length < _maxLogFileSize;
    }

    private string ResolveLogFileName()
    {
        var logFileName = $"_{_logTime.ToString(_dateFormat)}_{_suffix}.log";
        var logFilePath = Path.Combine(_logDirectory, logFileName);

        if (IsValidLogFile(logFilePath))
        {
            return logFileName;
        }

        _suffix++;
        return ResolveLogFileName();
    }

    private void WriteToLogFile()
    {
        var now = DateTime.Now.ToString(_dateFormat);
        if(_logTime.ToString(_dateFormat) != now)
        {
            Close();

            _logTime = DateTime.Now;
            _suffix = 0;
            _logDirectory = Path.Combine(FileSystem.AppDataDirectory, "logs", _logTime.ToString(_dateFormat));
            _logFileName = ResolveLogFileName();
            _logFilePath = _logFileName = Path.Combine(_logDirectory, _logFileName);

            _logDirInfo = Directory.CreateDirectory(_logDirectory);
        }

        var logData = LogData[^2];
        var logLine = $"{logData.Time}\t{logData.Type}: {logData.Message}{Environment.NewLine}";

        var buffer = Encoding.UTF8.GetBytes(logLine);

        using var stream = File.Open(_logFilePath, FileMode.Append);
        stream.Write(buffer, 0, buffer.Length);
    }
}