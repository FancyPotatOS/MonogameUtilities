

using Microsoft.Xna.Framework;
using MonogameUtilities.Util;
using System;
using System.Collections;
using System.Linq;
using System.Text.Json.Nodes;

namespace MonogameUtilities.Parsing
{

    internal class JsonWrapper : IEnumerable, IList, ICollection
    {
        private JsonNode _root;

        private bool? _isArray;
        private JsonArray _array;
        private JsonArray ArrayRef
        {
            get
            {
                if (_isArray == null)
                {
                    _isArray = true;
                    _array = _root.AsArray();
                }
                else if (!_isArray.Value)
                {
                    throw new Exception("Attempted to access index for non-array JsonWrapper.");
                }

                return _array;
            }
        }

        object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // Convert explicitly to an array
        internal T[] AsArray<T>()
        {
            if (MethodCastTypes.Contains(typeof(T)))
            {
                return ArrayRef.Cast<T>().ToArray();
            }
            else if (ImplicitCastTypes.Contains(typeof(T)))
            {
                return ArrayRef.Select(node => node.GetValue<T>()).ToArray();
            }
            else if (CompoundCastTypes.Select(tpl => tpl.Item1).Contains(typeof(T)))
            {
                Func<JsonWrapper, object> convertFunction = CompoundCastTypes.Where(tpl => tpl.Item1 == typeof(T)).Single().Item2;

                return AsCompoundArray().Select(item => convertFunction.Invoke(item)).ToArray().Cast<T>().ToArray();
            }
            else
            {
                throw new InvalidCastException($"Type {typeof(T).FullName} is not a supported {nameof(JsonWrapper)}.{nameof(AsArray)}<T>() type");
            }
        }

        internal JsonWrapper[] AsCompoundArray()
        {
            return ArrayRef.Select(node => (JsonWrapper)node).ToArray();
        }

        internal JsonWrapper(JsonNode root)
        {
            _root = root;
        }

        internal JsonWrapper this[int index]
        {
            get
            {
                return ArrayRef[index];
            }
            /** /
            set
            {
                ArrayRef[index] = value;
            }
            /**/
        }

        internal int GetArraySize()
        {
            return ArrayRef.Count;
        }

        internal JsonNode GetRaw()
        {
            return _root;
        }

        internal bool HasKey(string path)
        {
            return this[path] != null;
        }

        /// <summary>
        /// If multiple keys accessed, access the first key and recursively access the following keys <br/>
        /// Ex: <em><b>entities.rat.health => _root[entities][rat.health]</b></em> <br/>
        ///  <br/>
        /// Capable to access indicies by prefixing it with a $ <br/>
        /// Ex: <em><b>entities.rat.texture.pos.$0</b></em> <br/>
        /// </summary>
        internal JsonWrapper this[string path]
        {
            get
            {
                if (_root == null)
                {
                    return null;
                }

                // Check if there are multiple paths specified
                bool isMultiple = path.Contains('.');
                string[] splt = null;

                // Get the first key, and populate the splt array of the the paths
                string firstKey = path;
                if (isMultiple)
                {
                    splt = path.Split('.');

                    firstKey = splt[0];
                }

                // Reference to the first item accessed
                JsonWrapper firstRef;

                // If next key is an index
                bool isIndex = firstKey.StartsWith('$');
                if (isIndex)
                {
                    // Get the index
                    int index = int.Parse(firstKey[1..]);

                    // Access via int
                    firstRef = ArrayRef[index];
                }
                else
                {
                    // Otherwise, access as normal
                    firstRef = _root[firstKey];

                    if (firstRef == null || firstRef._root == null)
                    {
                        return null;
                    }
                }

                // Return the actual value
                if (isMultiple)
                {
                    // Recursive call
                    return firstRef[string.Concat(splt[1..])];
                }
                else
                {
                    // Otherwise, we found it!
                    return firstRef;
                }
            }
        }

        // Implicit equivalency
        public static implicit operator JsonWrapper(JsonNode node) { return new JsonWrapper(node); }
        public static implicit operator JsonNode(JsonWrapper node) { return node._root; }


        // Implicit type retrieval
        public static implicit operator int(JsonWrapper node) { return node._root.GetValue<int>(); }
        public static implicit operator double(JsonWrapper node) { return node._root.GetValue<double>(); }
        public static implicit operator float(JsonWrapper node) { return node._root.GetValue<float>(); }
        public static implicit operator string(JsonWrapper node) { return node._root.GetValue<string>(); }
        public static implicit operator byte(JsonWrapper node) { return node._root.GetValue<byte>(); }
        public static implicit operator uint(JsonWrapper node) { return node._root.GetValue<uint>(); }
        // (Some special operations)
        public static implicit operator char(JsonWrapper node) { return node._root.GetValue<string>()[0]; }
        public static implicit operator bool(JsonWrapper node) { return node._root.GetValue<int>() == 1; }
        public static implicit operator Point(JsonWrapper node) { int[] vals = node.AsArray<int>(); return new Point(vals[0], vals[1]); }
        public static implicit operator Color(JsonWrapper node)
        {
            byte[] vals = node.AsArray<byte>();
            return new Color(vals[0], vals[1], vals[2], vals.Length >= 4 ? vals[3] : byte.MaxValue);
        }

        public static implicit operator Dice(JsonWrapper node) { return node._root.GetValue<string>(); }

        // Native JsonNode types
        public static readonly Type[] ImplicitCastTypes = new Type[]
        {
            typeof(int),
            typeof(double),
            typeof(float),
            typeof(string),
            typeof(byte),
            typeof(uint)
        };
        // Types that technically are native, but must be casted explitly
        public static readonly Type[] MethodCastTypes = new Type[]
        {
            typeof(char),
            typeof(bool),
            typeof(Point),
            typeof(Dice),
        };
        // Types derived from compounds, and must be casted to a compound first
        public static readonly Tuple<Type, Func<JsonWrapper, object>> [] CompoundCastTypes = new Tuple<Type, Func<JsonWrapper, object>>[] {};


        // List-like interfaces

        /*  ==  IEnumerable Compliance  ==  */
        public IEnumerator GetEnumerator()
        {
            return new JsonWrapperEnumerator(this);
        }

        private class JsonWrapperEnumerator : IEnumerator
        {
            private readonly JsonWrapper _root;
            private int _index;
            private int _max;
            public JsonWrapperEnumerator(JsonWrapper root)
            {
                _root = root;
                _index = -1;

                _max = root.GetArraySize();
            }


            public object Current => _root[_index];

            public bool MoveNext()
            {
                _index++;

                return _max - 1 == _index;
            }

            public void Reset()
            {
                _index = -1;
            }
        }
        /*  == End of IEnumerable Compliance ==  */

        /*  == IList Compliance ==  */
        public bool IsFixedSize => true;

        public bool IsReadOnly => true;

        public int Count => GetArraySize();

        public bool IsSynchronized => false;

        public object SyncRoot => throw new NotImplementedException();

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return ArrayRef.Contains(value);
        }

        public int IndexOf(object value)
        {
            return ArrayRef.IndexOf((JsonNode)value);
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        /*  == End of IList Compliance ==  */
    }
}
