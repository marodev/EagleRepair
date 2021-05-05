using System.Collections.Generic;

namespace EagleRepair.Ast.Extensions
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dict,
            IDictionary<TKey, TValue> otherDict)
        {
            var dictResult = new Dictionary<TKey, TValue>();

            foreach (var (key, value) in dict)
            {
                dictResult.Add(key, value);
            }

            foreach (var (key, value) in otherDict)
            {
                dictResult.Add(key, value);
            }

            return dictResult;
        }
    }
}
