using NBitcoin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;



namespace GOBCommon
{
    public static class MerkleTree
    {
        public static bool If<T>(this T v, Func<T, bool> predicate, Action<T> action)
        {
            bool ret = predicate(v);

            if (ret)
            {
                action(v);
            }

            return ret;
        }

        public static bool If(this bool b, Action action)
        {
            if (b)
            {
                action();
            }

            return b;
        }

        public static void IfElse(this bool b, Action ifTrue, Action ifFalse)
        {
            if (b) ifTrue(); else ifFalse();
        }

        // Type is...
        public static bool Is<T>(this object obj, Action<T> action)
        {
            bool ret = obj is T;

            if (ret)
            {
                action((T)obj);
            }

            return ret;
        }

        // ---------- if-then-else as lambda expressions --------------

        // If the test returns true, execute the action.
        // Works with objects, not value types.
        public static void IfTrue<T>(this T obj, Func<T, bool> test, Action<T> action)
        {
            if (test(obj))
            {
                action(obj);
            }
        }

        /// <summary>
        /// Returns true if the object is null.
        /// </summary>
        public static bool IfNull<T>(this T obj)
        {
            return obj == null;
        }

        /// <summary>
        /// If the object is null, performs the action and returns true.
        /// </summary>
        public static bool IfNull<T>(this T obj, Action action)
        {
            bool ret = obj == null;

            if (ret) { action(); }

            return ret;
        }

        /// <summary>
        /// Returns true if the object is not null.
        /// </summary>
        public static bool IfNotNull<T>(this T obj)
        {
            return obj != null;
        }

        /// <summary>
        /// Return the result of the func if 'T is not null, passing 'T to func.
        /// </summary>
        public static R IfNotNullReturn<T, R>(this T obj, Func<T, R> func)
        {
            if (obj != null)
            {
                return func(obj);
            }
            else
            {
                return default(R);
            }
        }

        /// <summary>
        /// Return the result of func if 'T is null.
        /// </summary>
        public static R ElseIfNullReturn<T, R>(this T obj, Func<R> func)
        {
            if (obj == null)
            {
                return func();
            }
            else
            {
                return default(R);
            }
        }

        /// <summary>
        /// If the object is not null, performs the action and returns true.
        /// </summary>
        public static bool IfNotNull<T>(this T obj, Action<T> action)
        {
            bool ret = obj != null;

            if (ret) { action(obj); }

            return ret;
        }

        /// <summary>
        /// If not null, return the evaluation of the function, otherwise return the default value.
        /// </summary>
        public static R IfNotNull<T, R>(this T obj, R defaultValue, Func<T, R> func)
        {
            R ret = defaultValue;

            if (obj != null)
            {
                ret = func(obj);
            }

            return ret;
        }

        /// <summary>
        /// If the boolean is true, performs the specified action.
        /// </summary>
        public static bool Then(this bool b, Action f)
        {
            if (b) { f(); }

            return b;
        }

        /// <summary>
        /// If the boolean is false, performs the specified action and returns the complement of the original state.
        /// </summary>
        public static void Else(this bool b, Action f)
        {
            if (!b) { f(); }
        }

        // ---------- Dictionary --------------

        /// <summary>
        /// Return the key for the dictionary value or throws an exception if more than one value matches.
        /// </summary>
        public static TKey KeyFromValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue val)
        {
            // from: http://stackoverflow.com/questions/390900/cant-operator-be-applied-to-generic-types-in-c
            // "Instead of calling Equals, it's better to use an IComparer<T> - and if you have no more information, EqualityComparer<T>.Default is a good choice: Aside from anything else, this avoids boxing/casting."
            return dict.Single(t => EqualityComparer<TValue>.Default.Equals(t.Value, val)).Key;
        }

        // ---------- DBNull value --------------

        // Note the "where" constraint, only value types can be used as Nullable<T> types.
        // Otherwise, we get a bizzare error that doesn't really make it clear that T needs to be restricted as a value type.
        public static object AsDBNull<T>(this Nullable<T> item) where T : struct
        {
            // If the item is null, return DBNull.Value, otherwise return the item.
            return item as object ?? DBNull.Value;
        }

        // ---------- ForEach iterators --------------

        /// <summary>
        /// Implements a ForEach for generic enumerators.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        /// <summary>
        /// ForEach with an index.
        /// </summary>
        public static void ForEachWithIndex<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            int n = 0;

            foreach (var item in collection)
            {
                action(item, n++);
            }
        }

        /// <summary>
        /// Implements ForEach for non-generic enumerators.
        /// </summary>
        // Usage: Controls.ForEach<Control>(t=>t.DoSomething());
        public static void ForEach<T>(this IEnumerable collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
        }

        public static void ForEach(this int n, Action action)
        {
            for (int i = 0; i < n; i++)
            {
                action();
            }
        }

        public static void ForEach(this int n, Action<int> action)
        {
            for (int i = 0; i < n; i++)
            {
                action(i);
            }
        }

        public static IEnumerable<int> Range(this int n)
        {
            return Enumerable.Range(0, n);
        }

        public static T Single<T>(this IEnumerable collection, Func<T, bool> expr)
        {
            T ret = default(T);
            bool found = false;

            foreach (T item in collection)
            {
                if (expr(item))
                {
                    ret = item;
                    found = true;
                    break;
                }
            }

            found.Else(() => { throw new ApplicationException("Collection does not contain item in qualifier."); });

            return ret;
        }

        public static bool Contains<T>(this IEnumerable collection, Func<T, bool> expr)
        {
            bool found = false;

            foreach (T item in collection)
            {
                if (expr(item))
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        public static T SingleOrDefault<T>(this IEnumerable collection, Func<T, bool> expr)
        {
            T ret = default(T);

            foreach (T item in collection)
            {
                if (expr(item))
                {
                    ret = item;
                    break;
                }
            }

            return ret;
        }

        public static void ForEach(this DataTable dt, Action<DataRow> action)
        {
            foreach (DataRow row in dt.Rows)
            {
                action(row);
            }
        }

        public static void ForEach(this DataView dv, Action<DataRowView> action)
        {
            foreach (DataRowView drv in dv)
            {
                action(drv);
            }
        }

        /// <summary>
        /// Returns a new dictionary having merged the two source dictionaries.
        /// </summary>
        public static Dictionary<T, U> Merge<T, U>(this Dictionary<T, U> dict1, Dictionary<T, U> dict2)
        {
            return (new[] { dict1, dict2 }).SelectMany(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        // ---------- events --------------

        /// <summary>
        /// Encapsulates testing for whether the event has been wired up.
        /// </summary>
        public static void Fire<TEventArgs>(this EventHandler<TEventArgs> theEvent, object sender, TEventArgs e = null) where TEventArgs : EventArgs
        {
            if (theEvent != null)
            {
                theEvent(sender, e);
            }
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T newItem, Func<T, T, bool> equater)
        {
            List<T> result = new List<T>();

            foreach (T item in source)
            {
                if (!equater(item, newItem))
                {
                    result.Add(item);
                }
            }

            result.Add(newItem);

            return result;
        }

        public static void AddIfUnique<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Add then item if the comparer returns false, indicating a unique entry.
        /// </summary>
		public static void AddIfUnique<T>(this IList<T> list, T item, Func<T, bool> comparer)
        {
            if (!list.Contains(comparer))
            {
                list.Add(item);
            }
        }

        public static void RemoveLast<T>(this IList<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }

        public static bool IsEmpty(this string s)
        {
            return s == String.Empty;
        }
    }
}
