using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EagleRepair.Monitor
{
    public class FaultTracker : IFaultTracker
    {
        public static readonly string EmptyDiagnosticsMessage =
            Environment.NewLine + "No errors reported." + Environment.NewLine;

        private readonly IList<string> _messages;

        public FaultTracker()
        {
            _messages = new List<string>();
        }

        public void Add(string transformerName, string filePath, string faultMessage, string originalTree,
            string transformedTree, string semanticDiagnosticsBefore = "", string semanticDiagnosticsAfter = "")
        {
            var errorMessageNr = _messages.Count + 1;
            var builder = new StringBuilder();
            builder.Append(Environment.NewLine);
            builder.Append("######################################################################");
            builder.Append(Environment.NewLine);
            builder.Append("Nr: " + errorMessageNr + " - " + transformerName);
            builder.Append(Environment.NewLine);
            builder.Append("Filepath: " + filePath);
            builder.Append(Environment.NewLine);
            builder.Append("Description: " + faultMessage);
            builder.Append(Environment.NewLine);
            builder.Append("------------------------------------------------------------------------");
            builder.Append(Environment.NewLine);
            builder.Append("---- Original Tree ----");
            builder.Append(Environment.NewLine);
            builder.Append(originalTree);
            builder.Append(Environment.NewLine);
            builder.Append("---- Transformed Tree ----");
            builder.Append(Environment.NewLine);
            builder.Append(transformedTree);
            builder.Append(Environment.NewLine);
            builder.Append("---- Semantic diagnostics *before* transformation ----");
            builder.Append(Environment.NewLine);
            builder.Append(semanticDiagnosticsBefore);
            builder.Append(Environment.NewLine);
            builder.Append("---- Semantic diagnostics *after* transformation ----");
            builder.Append(Environment.NewLine);
            builder.Append(semanticDiagnosticsAfter);
            builder.Append(Environment.NewLine);
            builder.Append("######################################################################");
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);

            _messages.Add(builder.ToString());
        }

        public Tuple<int, string> ToDisplayString()
        {
            var displayMessage = _messages
                .Aggregate(string.Empty, (current, message) => current + message);

            if (string.IsNullOrEmpty(displayMessage))
            {
                displayMessage = EmptyDiagnosticsMessage;
            }

            return new Tuple<int, string>(_messages.Count, displayMessage);
        }
    }
}
