using System;

namespace EagleRepair.Monitor
{
    public interface IFaultTracker
    {
        public void Add(string transformerName, string filePath, string faultMessage, string originalTree,
            string transformedTree, string semanticDiagnosticsBefore = "", string semanticDiagnosticsAfter = "");

        public Tuple<int, string> ToDisplayString();
    }
}
