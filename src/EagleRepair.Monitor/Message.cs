namespace Monitor
{
    public record Message
    {
        public string Project { get; init; }
        public string Path { get; init; }
        public string Line { get; init; }
        public string Text { get; init; }
    }
}