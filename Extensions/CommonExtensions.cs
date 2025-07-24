using DynamicData;

namespace Func.Extensions
{
    public static class CommonExtensions
    {
        public static SourceList<T> ToSourceList<T>(this IEnumerable<T> source) where T : class
        {
            var sourceList = new SourceList<T>();
            sourceList.AddRange(source);
            return sourceList;
        }
    }
}
