using ModConstructor.ModClasses.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values.ComplexValues
{
    public class ComplexValue : Value
    {
        public override bool changed => GetProperties().Any(prop => prop.changed);

        public override XObject Pack(string name)
        {
            return PackElement(name);
        }

        public override  XElement PackElement(string name)
        {
            return new XElement(name, GetProperties().Where(prop => prop.values.Any(val => val.changed)).Select(prop => prop.Pack(prop.name)));
        }

        public override void Restore(XAttribute data)
        {
            Message.Inform(MainWindow.instance, "Ошибка", "Восстановление комлексного значения из аттрибута невозможно");
        }

        public override void Restore(XElement data)
        {
            Dictionary<string, Property> dictionary = GetProperties().ToDictionary(prop => prop.name);

            foreach (var attr in data.Attributes())
            {
                if (dictionary.ContainsKey(attr.Name.LocalName)) dictionary[attr.Name.LocalName].Restore(attr);
            }

            foreach (var elem in data.Elements())
            {
                if (dictionary.ContainsKey(elem.Name.LocalName)) dictionary[elem.Name.LocalName].Restore(elem);
            }
        }

        public IEnumerable<Property> GetProperties() => GetType().GetProperties().Where(prop => prop.PropertyType.IsSubclassOf(typeof(Property))).Select(prop => prop.GetValue(this) as Property);

        public override void Remove()
        {
            foreach (var prop in GetProperties()) prop.Remove();
        }

        public override void Initialize(Property property)
        {
            base.Initialize(property);
            foreach (var prop in GetProperties())
            {
                prop.Initialize(this);
            }
        }

        public override void Save()
        {
            foreach (var prop in GetProperties()) prop.Save();
        }

        public override void Revert()
        {
            foreach (var prop in GetProperties()) prop.Revert();
        }
    }
}
