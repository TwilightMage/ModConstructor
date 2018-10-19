using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values.SimpleValues
{
    public sealed class EnumerableValue : SimpleValue<int>
    {
        public static Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();

        public string name { get; }

        public string[] items => dictionary[name];

        public static void Register(string name, params string[] items)
        {
            dictionary[name] = items;
        }

        public static void Register(string name, Type enumerable)
        {
            dictionary[name] = Enum.GetNames(enumerable);
        }

        public EnumerableValue(string name, int value = 0) : base(value)
        {
            if (!dictionary.ContainsKey(name)) throw new Exception("Given name not registered");
            this.name = name;
        }

        public EnumerableValue(string name, int value = 0, params string[] items) : base(value)
        {
            dictionary[name] = items;
            this.name = name;
        }

        public override int AsNumber()
        {
            return value;
        }
    }
}
