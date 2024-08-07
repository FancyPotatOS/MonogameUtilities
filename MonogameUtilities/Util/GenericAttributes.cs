

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace MonogameUtilities.Util
{
    public partial class GenericAttributes
    {
        private readonly Dictionary<string, object> _attributes = new();

        /// <summary>
        /// Sets the attribute by its given name.<br/>
        /// This can overwrite existing elements.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <param name="value">The object to set.</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        public void SetAttribute(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            _attributes[name] = value;
        }

        /// <summary>
        /// Returns true if the attribute exists.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        public bool HasAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return _attributes.ContainsKey(name);
        }

        /// <summary>
        /// Returns the attribute by its name.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        public object GetAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return _attributes[name];
        }

        /// <summary>
        /// Returns the attribute by its name.<br/>
        /// If the attribute does not exist, it returns the default/specified value.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        public object GetAttributeOrDefault(string name, object defaultValue = default)
        {
            return HasAttribute(name) ? _attributes[name] : defaultValue;
        }

        /// <summary>
        /// Returns a list of every attribute names.
        /// </summary>
        public List<string> GetAttributeNames()
        {
            return _attributes.Keys.ToList();
        }

        /// <summary>
        /// Removes the attribute by its name.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        /// <exception cref="ArgumentException">Throws an exception if the name does not exist.</exception>
        public void RemoveAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (!HasAttribute(name)) throw new ArgumentException($"Name does not exist.");

            _attributes.Remove(name);
        }

        /// <summary>
        /// Pulls attribute data from the provided json.<br/>
        /// Expected format is as follows:<br/>
        /// <code>
        /// [<br/>
        ///   {<br/>
        ///     "type": "int|float|string|dice_rolled|dice|bool|string_list|guid_list"<br/>
        ///     "name": "string"<br/>
        ///     "value": <see cref="JsonWrapper"/>-compliant object matching the type listed in "type"<br/>
        ///   },<br/>
        ///   ...<br/>
        /// ]<br/></code><br/>
        /// 'dice_rolled' type is of object type <see cref="Dice"/>, which is returned as an int.
        /// </summary>
        /// <param name="json">The JSON to pull attributes from.</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the json is null.</exception>
        public void AttributesFromJson(JsonWrapper json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (JsonWrapper attributeJson in json.AsCompoundArray())
            {
                string name = attributeJson["name"];
                string type = attributeJson["type"];
                if (type == "int")
                {
                    SetAttribute(name, (int)attributeJson["value"]);
                }
                else if (type == "float")
                {
                    SetAttribute(name, (float)attributeJson["value"]);
                }
                else if (type == "string")
                {
                    SetAttribute(name, (string)attributeJson["value"]);
                }
                else if (type == "dice_rolled")
                {
                    SetAttribute(name, Dice.Parse((string)attributeJson["value"]).Roll());
                }
                else if (type == "dice")
                {
                    SetAttribute(name, Dice.Parse((string)attributeJson["value"]));
                }
                else if (type == "bool")
                {
                    SetAttribute(name, ((int)attributeJson["value"]) == 1);
                }
                else if (type == "string_list")
                {
                    SetAttribute(name, attributeJson["value"].AsArray<string>().ToList());
                }
                else if (type == "guid_list")
                {
                    SetAttribute(name, attributeJson["value"].AsArray<string>().Select(val => Guid.Parse(val)).ToList());
                }
                else
                {
                    throw new Exception($"Unrecognized {nameof(GenericAttributes)} type {type} with name {name}.");
                }
            }
        }

        /// <summary>
        /// Returns a string attribute by its name.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        public string GetStringAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return (string)GetAttribute(name);
        }

        /// <summary>
        /// Returns a string attribute by its name.<br/>
        /// If the attribute does not exist, it returns the default/specified value.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        public string GetStringAttributeOrDefault(string name, string defaultValue = default)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            if (!HasAttribute(name))
            {
                return defaultValue;
            }
            else
            {
                return GetStringAttribute(name);
            }
        }

        /// <summary>
        /// Returns a boolean attribute by its name.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        public bool GetBoolAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return (bool)GetAttribute(name);
        }

        /// <summary>
        /// Returns a boolean attribute by its name.<br/>
        /// If the attribute does not exist, it returns the default/specified value.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        public bool GetBoolAttributeOrDefault(string name, bool defaultValue = default)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            if (!HasAttribute(name))
            {
                return defaultValue;
            }
            else
            {
                return GetBoolAttribute(name);
            }
        }

        /// <summary>
        /// Returns an int attribute by its name.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        public int GetIntAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return (int)GetAttribute(name);
        }

        /// <summary>
        /// Returns an int attribute by its name.<br/>
        /// If the attribute does not exist, it returns the default/specified value.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        public int GetIntAttributeOrDefault(string name, int defaultValue = default)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            if (!HasAttribute(name))
            {
                return defaultValue;
            }
            else
            {
                return GetIntAttribute(name);
            }
        }

        /// <summary>
        /// Returns a float attribute by its name.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        public float GetFloatAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return (float)GetAttribute(name);
        }

        /// <summary>
        /// Returns a float attribute by its name.<br/>
        /// If the attribute does not exist, it returns the default/specified value.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        public float GetFloatAttributeOrDefault(string name, float defaultValue = default)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            if (!HasAttribute(name))
            {
                return defaultValue;
            }
            else
            {
                return GetFloatAttribute(name);
            }
        }

        /// <summary>
        /// Returns the <see cref="AttributesFromJson"/>-type name of the type of the attribute.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws an exception if the name is null or empty.</exception>
        /// <exception cref="ArgumentException">Throws an exception if the type of the object matching that name is not recognized.</exception>
        internal string GetTypeOf(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            object value = GetAttribute(name);

            if (value is int)
            {
                return "int";
            }
            else if (value is float)
            {
                return "float";
            }
            else if (value is string)
            {
                return "string";
            }
            else if (value is Dice)
            {
                return "dice";
            }
            else if (value is bool)
            {
                return "bool";
            }
            else if (value is List<string>)
            {
                return "string_list";
            }
            else if (value is List<Guid>)
            {
                return "guid_list";
            }
            else
            {
                throw new ArgumentException($"Unrecognized {nameof(GenericAttributes)} type for attribute.");
            }
        }
    }
}
