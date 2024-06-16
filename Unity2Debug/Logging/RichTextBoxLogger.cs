using FluentValidation;
using FluentValidation.Results;
using System.Windows.Controls;
using Unity2Debug.Common.Logging;

namespace Unity2Debug.Logging
{
    public class RichTextBoxLogger : ILogger
    {
        private RichTextBox _textBox;

        public RichTextBoxLogger(RichTextBox richTextBox)
        {
            _textBox = richTextBox;
        }

        public void Clear()
        {
            _textBox.Dispatcher.Invoke(_textBox.Document.Blocks.Clear);
            Log("");
        }

        public void LogToTextBox(string message)
        {
            _textBox.Dispatcher.Invoke(() =>
            {
                _textBox.AppendText(message + Environment.NewLine);
                _textBox.ScrollToEnd();
            });
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
