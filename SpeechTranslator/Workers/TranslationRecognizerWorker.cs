namespace SpeechTranslator.Worker;

public class TranslationRecognizerWorker: TranslationRecognizerWorkerBase
{
    public override void OnRecognizing(TranslationRecognitionEventArgs e)
    {
        throw new NotImplementedException();
    }

    public override void OnRecognized(TranslationRecognitionEventArgs e)
    {
        throw new NotImplementedException();
    }

    public override void OnCanceled(TranslationRecognitionCanceledEventArgs e)
    {
        throw new NotImplementedException();
    }

    public override void OnSpeechStartDetected(RecognitionEventArgs e)
    {
        throw new NotImplementedException();
    }

    public override void OnSpeechEndDetected(RecognitionEventArgs e)
    {
        throw new NotImplementedException();
    }

    public override void OnSessionStarted(SessionEventArgs e)
    {
        throw new NotImplementedException();
    }

    public override void OnSessionStopped(SessionEventArgs e)
    {
        throw new NotImplementedException();
    }
}