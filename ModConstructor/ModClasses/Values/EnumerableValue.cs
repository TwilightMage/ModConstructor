using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public class EnumerableValue : IValueNumber
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ChangeEventHandler<int> Change;

        public static Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();

        public string name { get; }

        public IProperty property { get; set; }
        public string where => property.where;

        private int _value = 0;
        public int value
        {
            get => _value;
            set
            {
                int before = _value;
                _value = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("value"));
                Change?.Invoke(before, value);
            }
        }

        public string[] items => dictionary[name];

        public static void Register(string name, params string[] items)
        {
            dictionary[name] = items;
        }

        public static void Register(string name, Type enumerable)
        {
            dictionary[name] = Enum.GetNames(enumerable);
        }

        public EnumerableValue(string name, int def = 0)
        {
            if (!dictionary.ContainsKey(name)) throw new Exception("Given name not registered");
            _value = def;
            this.name = name;
        }

        public EnumerableValue(string name, int def = 0, params string[] items)
        {
            dictionary[name] = items;
            this.name = name;
            _value = def;
        }

        public override bool Equals(object obj)
        {
            if (obj is EnumerableValue)
            {
                EnumerableValue ev = obj as EnumerableValue;
                return ev.name == name && ev.value == value;
            }
            else return false;
        }

        public override string ToString()
        {
            if (value >= 0 && value < items.Length) return items[value];
            else return "{Enumerable value error}";
        }

        public XObject Pack(string name)
        {
            return new XAttribute(name, value.ToString());
        }

        public XElement PackElement(string name)
        {
            return new XElement(name, value.ToString());
        }

        public void Restore(XAttribute data)
        {
            value = int.Parse(data.Value);
        }

        public void Restore(XElement data)
        {
            value = int.Parse(data.Value);
        }

        public int AsNumber()
        {
            return value;
        }

        public static implicit operator int(EnumerableValue value) => value.value;

        public void Remove()
        {

        }

        public void Initialize(IProperty property)
        {
            this.property = property;
        }
    }
}
