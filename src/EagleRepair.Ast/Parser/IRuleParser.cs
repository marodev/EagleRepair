using System.Collections.Generic;

namespace Ast.Parser
{
    public interface IRuleParser
    {
        ICollection<Rule> Parse(ICollection<string> rules);
    }
}