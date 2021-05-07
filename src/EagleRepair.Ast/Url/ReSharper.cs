namespace EagleRepair.Ast.Url
{
    public static class ReSharper
    {
        public const string BaseUrl = "https://www.jetbrains.com/help/resharper";
        public const string ToolName = "ReSharper";

        public static readonly string MergeSequentialChecksUrl = $"{BaseUrl}/MergeSequentialChecks.html";

        public static readonly string MergeSequentialChecksMessage =
            $"{ToolName}: Merge sequential checks in && or || expressions. See {MergeSequentialChecksUrl}";

        public static readonly string ReplaceWithOfType2Message = $"{ToolName}: ReplaceWithOfType.2.";

        public static readonly string MergeCastWithTypeCheckUrl = $"{BaseUrl}/MergeCastWithTypeCheck.html";

        public static readonly string MergeCastWithTypeCheckMessage =
            $"{ToolName}: Type check and casts can be merged. See {MergeCastWithTypeCheckUrl}";

        public static readonly string UseNullPropagationUrl = $"{BaseUrl}/UseNullPropagation.html";

        public static readonly string UseNullPropagationMessage =
            $"{ToolName}: Replace if statement with null-propagating code. See {UseNullPropagationUrl}";

        public static readonly string UsePatternMatchingUrl = $"{BaseUrl}/UsePatternMatching.html";

        public static readonly string UsePatternMatchingMessage =
            $"{ToolName}: Convert 'as' expression type check and the following null check into pattern matching. See {UsePatternMatchingUrl}";

        public static readonly string UseStringInterpolationUrl = $"{BaseUrl}/UseStringInterpolation.html";

        public static readonly string UseStringInterpolationMessage =
            $"{ToolName}: Use string interpolation expression. See {UseStringInterpolationUrl}";

        public static readonly string ReplaceWithStringIsNullOrEmptyUrl =
            $"{BaseUrl}/ReplaceWithStringIsNullOrEmpty.html";

        public static readonly string ReplaceWithStringIsNullOrEmptyMessage =
            $"{ToolName}: Use 'String.IsNullOrEmpty'. See {ReplaceWithStringIsNullOrEmptyUrl}";

        public static readonly string UseMethodAnyMessage =
            $"{ToolName}: Use '.Any()' to test whether this IEnumerable is empty or not.";

        // ReplaceWithSingleCallToAny
        // ReplaceWithSingleCallToCount
        // ReplaceWithSingleCallToFirstOrDefault
        // ReplaceWithSingleCallToSingleOrDefault
        // ReplaceWithSingleCallToFirst
        // ReplaceWithSingleCallToSingle
        // ReplaceWithSingleCallToLast
        public static string ReplaceWith(string invokedMethodName)
        {
            return $"{ToolName}: ReplaceWithSingleCallTo{invokedMethodName}.";
        }
    }
}
