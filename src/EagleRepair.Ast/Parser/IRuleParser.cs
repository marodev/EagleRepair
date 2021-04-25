using System.Collections.Generic;

namespace EagleRepair.Ast.Parser
{
    public interface IRuleParser
    {
        ICollection<Rule> Parse(ICollection<string> rules);
    }
}