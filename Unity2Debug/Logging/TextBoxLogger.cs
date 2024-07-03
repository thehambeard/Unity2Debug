using FluentValidation;
using FluentValidation.Results;
using System.Windows.Controls;
using Unity2Debug.Common.Logging;

namespace Unity2Debug.Logging
{
    public class TextBoxLogger : ILogger
    {
        private readonly TextBox _textBox;
        private readonly List<string> _logBuffer = new List<string>();
        private readonly object _lock = new object();
        private bool _isFlushing = false;

        public TextBoxLogger(TextBox richTextBox)
        {
            _textBox = richTextBox;
        }

        public void Clear()
        {
            lock (_lock)
            {
                _textBox.Dispatcher.Invoke(_textBox.Clear);
                _logBuffer.Clear();
            }
        }

        public void LogToTextBox(string message)
        {
            lock (_lock)
            {
                _logBuffer.Add(message);
                if (!_isFlushing)
                {
                    _isFlushing = true;
                    FlushBufferToTextBox();
                }
            }
        }

        private void FlushBufferToTextBox()
        {
            _textBox.Dispatcher.BeginInvoke(new Action(() =>
            {
                lock (_lock)
                {
                    foreach (var message in _logBuffer)
                    {
                        _textBox.AppendText(message + Environment.NewLine);
                    }
                    _textBox.ScrollToEnd();
                    _logBuffer.Clear();
                    _isFlushing = false;
                }
            }));
        }

        public void Error(string message)
        {
            LogToTextBox("ERROR: " + message);
        }

        public void Error(Exception exception)
        {
            LogToTextBox("ERROR: " + exception.Message + exception.StackTrace);
        }

        public void Log(string message)
        {
            LogToTextBox(message);
        }

        public void Warn(string message)
        {
            LogToTextBox("WARNING: " + message);
        }

        public void LogValidation(ValidationFailure validationFailure)
        {
            switch (validationFailure.Severity)
            {
                case Severity.Error:
                    Error(validationFailure.ErrorMessage);
                    break;
                case Severity.Warning:
                    Warn(validationFailure.ErrorMessage);
                    break;
                case Severity.Info:
                    Log(validationFailure.ErrorMessage);
                    break;
                default:
                    Log(validationFailure.ErrorMessage);
                    break;
            }
        }
    }
}
