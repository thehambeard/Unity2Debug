using ILSpyAutomation;

namespace Unity2Debug.Common.Logging
{
    internal class DecompileLogger(ILogger logger) : LoggerBase(logger), IDecompileLogger
    {
        public void Log(string message) => _logger.Log(message);
    }
}
