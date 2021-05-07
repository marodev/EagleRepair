/*
    https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54
    
    Copyright 2015 Daniel Wolf

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Text;
using System.Threading;

namespace EagleRepair.Monitor
{
    /// <summary>
    ///     An ASCII progress bar
    /// </summary>
    public sealed class ProgressBar : IDisposable, IProgressBar
    {
        private const int BlockCount = 10;
        private const string Animation = @"|/-\";

        private int _animationIndex;

        private double _currentProgress;
        private string _currentText = string.Empty;
        private bool _disposed;

        public void Dispose()
        {
            _disposed = true;
            UpdateText(string.Empty);
        }

        public void Report(double value, string message = "")
        {
            if (Console.IsOutputRedirected || _disposed)
            {
                return;
            }

            // Make sure value is in [0..1] range
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref _currentProgress, value);

            var progressBlockCount = (int)(_currentProgress * BlockCount);
            var percent = (int)(_currentProgress * 100);
            var text =
                $"[{new string('#', progressBlockCount)}{new string('-', BlockCount - progressBlockCount)}] {percent,3}% {Animation[_animationIndex++ % Animation.Length]}";

            if (!string.IsNullOrEmpty(message))
            {
                text += $" {message}";
            }

            UpdateText(text);
        }

        private void UpdateText(string text)
        {
            // Get length of common portion
            var commonPrefixLength = 0;
            var commonLength = Math.Min(_currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == _currentText[commonPrefixLength])
            {
                commonPrefixLength++;
            }

            // Backtrack to the first differing character
            var outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', _currentText.Length - commonPrefixLength);

            // Output new suffix
            outputBuilder.Append(text.Substring(commonPrefixLength));

            // If the new text is shorter than the old one: delete overlapping characters
            var overlapCount = _currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.Write(outputBuilder);
            _currentText = text;
        }
    }
}
