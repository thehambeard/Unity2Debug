using FluentValidation.Results;

namespace Unity2Debug.Common.Logging
{
    public interface ILogger
    {
        void Log(string message);
        void Warn(string message);
        void Error(string message);
        void Error(Exception exception);
        void LogValidation(ValidationFailure validationResult);
    }
}
