using System;
using System.Collections.Generic;
using System.Linq;

namespace EagleRepair.Statistics
{
    public static class StaticAnalysisRules
    {
        public static Dictionary<string, int> All()
        {
            var merged = new Dictionary<string, int>();
            var resharper = ReSharperRules();
            var sonarqube = SonarQubeRules();

            foreach (var (key, value) in resharper)
            {
                merged.Add(key, value);
            }

            foreach (var (key, value) in sonarqube)
            {
                merged.Add(key, value);
            }

            return merged;
        }

        private static Dictionary<string, int> ReSharperRules()
        {
            return Enum.GetValues(typeof(ReSharperRule))
                .Cast<ReSharperRule>()
                .ToDictionary(t => t.ToString(), _ => 0);
        }

        private static Dictionary<string, int> SonarQubeRules()
        {
            return Enum.GetValues(typeof(SonarQubeRule))
                .Cast<SonarQubeRule>()
                .ToDictionary(t => t.ToString(), _ => 0);
        }
    }
}
