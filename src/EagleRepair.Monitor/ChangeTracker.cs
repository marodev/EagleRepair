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
            var consoleMessage = "";
            const string Offset = "    ";

            var totalFixedReSharperIssues = 0;
            var totalFixedSonarQubeIssues = 0;
            var totalFixedIssues = 0;
            foreach (var projectNameMessages in _messages)
            {
                consoleMessage += $"Project: {projectNameMessages.Key}\n";
                var messagesPerProject = projectNameMessages.Value.OrderBy(m => m.Path).ThenBy(m => m.Line).ToImmutableList();
                totalFixedReSharperIssues += messagesPerProject.Count(m => m.Text.Contains("ReSharper"));
                totalFixedSonarQubeIssues += messagesPerProject.Count(m => m.Text.Contains("SonarQube"));
                totalFixedIssues += messagesPerProject.Count;

                consoleMessage = messagesPerProject.Aggregate(consoleMessage, (current, message) => current + $"{Offset}Path: {message.Path}, Line: {message.Line}, Message: {message.Text}\n");
            }

            consoleMessage += "\n\n\n" + "--- Summary ---\n";
            consoleMessage += "Fixed ReSharper: " + totalFixedReSharperIssues + "\n";
            consoleMessage += "Fixed SonarQube: " + totalFixedSonarQubeIssues + "\n";
            consoleMessage += "Fixed Total: " + totalFixedIssues + "\n";

            return consoleMessage;
        }
    }
}
