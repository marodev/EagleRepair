using System;
using System.Runtime.InteropServices;

namespace EagleRepair.Ast.Url
{
    public static class SonarQube
    {
        public const string BaseUrl = "https://rules.sonarsource.com/csharp/RSPEC-";
        public const string ToolName = "SonarQube";

        public static readonly string RuleSpecification3881Url = $"{BaseUrl}3881";

        public static readonly string RuleSpecification3881Message =
            @$"{ToolName}: ""IDisposable"" should be implemented correctly. See {RuleSpecification3881Url}";

        public static readonly string RuleSpecification4201Url = $"{BaseUrl}4201";

        public static readonly string RuleSpecification4201Message =
            @$"{ToolName}: Null checks should not be used with ""is"". See {RuleSpecification4201Url}";

        public static readonly string RuleSpecification2971Url = $"{BaseUrl}2971";

        public static readonly string RuleSpecification2971Message =
            @$"{ToolName} ""IEnumerable"" LINQs should be simplified. See {RuleSpecification2971Url}";

        public static readonly string RuleSpecification3247Url = $"{BaseUrl}3247";

        public static readonly string RuleSpecification3247Message =
            @$"{ToolName}: Duplicate casts should not be made. See {RuleSpecification3247Url}";
        
        public static readonly string RuleSpecification3256Url = $"{BaseUrl}3256";

        public static readonly string RuleSpecification3256Message =
            @$"{ToolName}: ""string.IsNullOrEmpty"" should be used. See {RuleSpecification3256Url}";
        
        public static readonly string RuleSpecification1155Url = $"{BaseUrl}1155";
        
        public static readonly string RuleSpecification1155Message =
            @$"{ToolName}: ""Any()"" should be used to test for emptiness. See {RuleSpecification1155Url}";
    }
}
