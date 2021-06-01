namespace EagleRepair.Monitor
{
    public class Message
    {
        public string ReSharperId
        {
            get;
            init;
        }

        public string SonarQubeId
        {
            get;
            init;
        }

        public string RuleId
        {
            get;
            init;
        }

        public string ProjectName
        {
            get;
            init;
        }

        public string FilePath
        {
            get;
            init;
        }

        public string LineNr
        {
            get;
            init;
        }

        public string Text
        {
            get;
            init;
        }
    }
}
