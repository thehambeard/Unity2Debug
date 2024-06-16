namespace Unity2Debug.Common.Logging
{
    internal abstract class LoggerBase(ILogger logger)
    {
        protected readonly ILogger _logger = logger;
    }
}
