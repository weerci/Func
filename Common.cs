namespace Func
{
    public static class Common
    {
        public static TOut Exec<TIn, TOut>(Func<TIn, TOut> func, TIn input) => func(input);
        public static TOut Exec<TOut>(Func<TOut> func) => func();

        /// <summary>
        /// Добавляет элемент в список и возвращает обновленный список
        /// </summary>
        public static List<T> AddEx<T>(this List<T> list, T t)
        {
            list.Add(t);
            return list;
        }

        /// <summary>
        /// Приблуда для индексации foreach
        /// foreach (var (item, index) in collection.WithIndex())
        /// {
        ///     DoSomething(item, index);
        /// }
        /// </summary>
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index + 1));
        }

        /// <summary>
        /// Удаляет все пробелы из строки
        /// </summary>
        public static string RemoveSpaces(this string s)
        {
            return string.Concat(s.Where(c => !char.IsWhiteSpace(c)));
        }

        /// <summary>
        /// Получает значение enum по строковому представлению
        /// </summary>
        public static T ToEnum<T>(this string value) where T : struct, IConvertible
        {
            if (!Enum.TryParse(value, true, out T result))
                return default;

            if (!Enum.IsDefined(typeof(T), result))
                return default;

            return result;
        }

        /// <summary>
        /// Другая приблуда для индексации foreach
        /// var strings = new List<string>();
        /// strings.Each((str, n) =>
        /// {
        ///    DoSomething(str, n);
        /// });
        ///</summary>
        public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }
    }
}
