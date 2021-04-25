using System;
using System.Collections.Generic;
using System.Linq;

namespace Ast.Parser
{
    public class RuleParser : IRuleParser
    {
        public ICollection<Rule> Parse(ICollection<string> rules)
        {
            if (rules == null || !rules.Any())
            {
                return FetchAllRules();
            }
            
            List<Rule> parsedRules = new();
            foreach (var rule in rules)
            {
                var success = Enum.TryParse(rule, true, out Rule parsedRule);
                if (success)
                {
                    parsedRules.Add(parsedRule);
                }
                else
                {
                    var possibleRules = string.Join(", ", FetchAllRules().ToArray());
                    throw new ArgumentException($"Unknown rule: {rule}. Possible values are: {possibleRules}");
                }
            }
            return parsedRules;
        }

        private static ICollection<Rule> FetchAllRules()
        {
            return Enum.GetValues(typeof(Rule)).Cast<Rule>().ToList();
        }
    }
}