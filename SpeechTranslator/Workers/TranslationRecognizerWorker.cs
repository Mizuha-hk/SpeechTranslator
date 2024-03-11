using SpeechTranslator.Services;

namespace SpeechTranslator.Worker;

public class TranslationRecognizerWorker: TranslationRecognizerWorkerBase
{
    private readonly IAppLogger _logger;

    public TranslationRecognizerWorker(IAppLogger logger)
    {
        _logger = logger;
    }

    public override void OnRecognizing(TranslationRecognitionEventArgs e)
    {
        _logger.Write("...");
    }

    public override void OnRecognized(TranslationRecognitionEventArgs e)
    {
        _logger.Write("\n\n");
        
        var result = e.Result;

        if(result.Reason == ResultReason.TranslatedSpeech)
        {
            _logger.WriteLine($"TRANSLATED: {result.Text}");

            foreach(var translation in result.Translations)
            {
                _logger.WriteLine($"TRANSLATED: {translation.Key}-> {translation.Value}");
            }
        }
        else if(result.Reason == ResultReason.RecognizedSpeech)
        {
            _logger.WriteLine($"RECOGNIZED: {result.Text}");
        }
        else if(result.Reason == ResultReason.NoMatch)
        {
            _logger.WriteLine($"NOMATCH: Speech could not be recognized.");
        }

        _logger.WriteLine("");
    }

    public override void OnCanceled(TranslationRecognitionCanceledEventArgs e)
    {
        _logger.WriteLine($"CANCELED: Reason={e.Reason}");

        if(e.Reason == CancellationReason.Error)
        {
            _logger.Fail($"CANCELED: ErrorCode={e.ErrorCode}");
            _logger.Fail($"CANCELED: ErrorDetails={e.ErrorDetails}");
            _logger.Fail($"CANCELED: Did you update the subscription info?");
        }

        _logger.WriteLine("");
    }

    public override void OnSpeechStartDetected(RecognitionEventArgs e)
    {
        _logger.WriteLine($"Speech start detected: {e.SessionId}");
    }

    public override void OnSpeechEndDetected(RecognitionEventArgs e)
    {
        _logger.WriteLine($"Speech end detected: {e.SessionId}");
    }

    public override void OnSessionStarted(SessionEventArgs e)
    {
        _logger.WriteLine($"Session started: {e.SessionId}");
    }

    public override void OnSessionStopped(SessionEventArgs e)
    {
        _logger.WriteLine($"Session stopped: {e.SessionId}");
    }
}