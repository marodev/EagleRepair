using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EagleRepair.Monitor
{
    public class ChangeTracker : IChangeTracker
    {
        // <projectName, messages>
        private readonly Dictionary<string, IList<Message>> _messages;

        public ChangeTracker()
        {
            _messages = new Dictionary<string, IList<Message>>();
        }

        public void Add(Message message)
        {
            var messages = _messages.ContainsKey(message.Project)
                ? _messages[message.Project]
                : new List<Message>();
            messages.Add(message);
            _messages[message.Project] = messages;
        }

        public Dictionary<string, IList<Message>> All()
        {
            return _messages;
        }

        public string ToDisplayString()
        {
            var consoleMessage = string.Empty;
            const string Offset = "    ";

            var totalFixedReSharperIssues = 0;
            var totalFixedSonarQubeIssues = 0;
            var totalFixedIssues = 0;
            var number = 1;
            foreach (var (projectName, messages) in _messages)
            {
                consoleMessage +=
                    $"{Environment.NewLine}{Environment.NewLine}Project: {projectName}{Environment.NewLine}";
                ;
                var messagesPerProject =
                    messages.OrderBy(m => m.Path).ThenBy(m => m.Line).ToImmutableList();

                totalFixedReSharperIssues += messagesPerProject.Count(m => m.Text.Contains("ReSharper"));
                totalFixedSonarQubeIssues += messagesPerProject.Count(m => m.Text.Contains("SonarQube"));
                totalFixedIssues += messagesPerProject.Count;

                foreach (var message in messagesPerProject)
                {
                    consoleMessage +=
                        $"{Offset}#{number} Path: {message.Path}, Line: {message.Line}, Message: {message.Text}{Environment.NewLine}";
                    ;
                    number++;
                }

                consoleMessage += Environment.NewLine;
            }

            consoleMessage += $"{Environment.NewLine}--- Summary ---{Environment.NewLine}";
            consoleMessage += $"Fixed ReSharper issues: {totalFixedReSharperIssues}{Environment.NewLine}";
            consoleMessage += $"Fixed SonarQube issues: {totalFixedSonarQubeIssues}{Environment.NewLine}";
            consoleMessage += $"Total fixed issues: {totalFixedIssues}{Environment.NewLine}";

            return consoleMessage;
        }
    }
}
