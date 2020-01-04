using System.Collections.Generic;

namespace Ffxiv.Common
{
    public static class IDictionaryExtensions
    {
        public static bool TryGetNonNullValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, out TValue value)
        {
            return dict.TryGetValue(key, out value) && value != null;
        }
    }
}