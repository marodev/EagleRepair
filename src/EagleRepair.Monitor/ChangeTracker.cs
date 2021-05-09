using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EagleRepair.Monitor
{
    public class ChangeTracker : IChangeTracker
    {
        // applied changes
        private readonly Dictionary<string, IList<Message>> _appliedChanges;

        // <projectName, messages>
        // reported changes before we know the syntax tree was created correctly
        private readonly Dictionary<string, IList<Message>> _stagedChanges;

        public ChangeTracker()
        {
            _stagedChanges = new Dictionary<string, IList<Message>>();
            _appliedChanges = new Dictionary<string, IList<Message>>();
        }

        public void Confirm()
        {
            foreach (var (_, messages) in _stagedChanges)
            {
                Add(messages, _appliedChanges);
            }
        }

        public void Revert()
        {
            _stagedChanges.Clear();
        }

        public void Stage(Message message)
        {
            Add(message, _stagedChanges);
        }

        public Dictionary<string, IList<Message>> All()
        {
            return _stagedChanges;
        }

        public string ToDisplayString()
        {
            var consoleMessage = string.Empty;
            const string Offset = "    ";

            var totalFixedReSharperIssues = 0;
            var totalFixedSonarQubeIssues = 0;
            var totalFixedIssues = 0;
            var number = 1;
            foreach (var (projectName, messages) in _appliedChanges)
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

        private static void Add(IEnumerable<Message> messages, IDictionary<string, IList<Message>> changes)
        {
            foreach (var message in messages)
            {
                Add(message, changes);
            }
        }

        private static void Add(Message message, IDictionary<string, IList<Message>> changes)
        {
            var messages = changes.ContainsKey(message.Project)
                ? changes[message.Project]
                : new List<Message>();

            messages.Add(message);
            changes[message.Project] = messages;
        }
    }
}
