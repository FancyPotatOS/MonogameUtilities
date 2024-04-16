
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonogameUtilities.Util
{
    /// <summary>
    /// This class will contain a set of different possible values, and will cast into a random one every attempt.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Provider<T>
    {
        private List<T> _values;

        public int Length { get { return _values.Count; } }
        public int Count { get { return _values.Count; } }

        public static Random RNG = new Random();

        public Provider(T value)
        {
            _values = new List<T>()
            { value };
        }

        public Provider(params T[] values)
        {
            _values = values.ToList();
        }

        public Provider(List<T> values)
        {
            _values = new List<T>();
            _values.AddRange(values);
        }

        public static implicit operator T(Provider<T> provider)
        {
            if (!provider._values.Any())
            {
                return default;
            }
            else
            {
                int maxIndex = provider._values.Count;

                int choice = RNG.Next(maxIndex);

                return provider._values[choice];
            }
        }

        public static implicit operator Provider<T>(T[] vals)
        {
            if (vals == null || !vals.Any())
            {
                return default;
            }
            else
            {
                return new Provider<T>(vals.ToList());
            }
        }

        public T Pop()
        {
            if (!_values.Any())
            {
                return default;
            }
            else
            {
                int maxIndex = _values.Count;

                int choice = RNG.Next(maxIndex);

                T ret = _values[choice];
                _values.RemoveAt(choice);

                return ret;
            }
        }
    }
}
