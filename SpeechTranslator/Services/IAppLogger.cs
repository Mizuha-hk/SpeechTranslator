using System.Collections.ObjectModel;

namespace SpeechTranslator.Services;

public interface IAppLogger
{
    ObservableCollection<LogData> LogData { get; }

    event LogDataChangedEventHandler LogDataChanged;

    void Write(string? message);

    void WriteLine(string? message);

    void WriteLine(object? obj);

    void Fail(string? message);

    void Clear();

    void Close();

    void Flush();

    delegate void LogDataChangedEventHandler(object sender, LogDataChangedEventArgs e);
}
