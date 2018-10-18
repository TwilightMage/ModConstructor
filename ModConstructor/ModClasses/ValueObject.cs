using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses
{
    public class ValueSolution : IValue
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IProperty property { get; set; }
        public virtual string where => property.where;

        public virtual XObject Pack(string name)
        {
            return PackElement(name);
        }

        public virtual XElement PackElement(string name)
        {
            XElement result = new XElement(name);
            foreach (var prop in GetProperties(GetType()))
            {
                IProperty property = (IProperty)prop.GetValue(this);
                if (property.changed) result.Add(property.Pack(prop.Name));
            }
            return result;
        }

        public virtual void Restore(XAttribute data)
        {
            
        }

        public virtual void Restore(XElement data)
        {
            Dictionary<string, IProperty> dictionary = new Dictionary<string, IProperty>();

            foreach (var prop in GetProperties(GetType()))
            {
                IProperty property = (IProperty)prop.GetValue(this);
                dictionary.Add(property.shortname, property);
            }

            foreach (var attr in data.Attributes())
            {
                if (dictionary.ContainsKey(attr.Name.LocalName)) dictionary[attr.Name.LocalName].Restore(attr);
            }

            foreach (var elem in data.Elements())
            {
                if (dictionary.ContainsKey(elem.Name.LocalName)) dictionary[elem.Name.LocalName].Restore(elem);
            }
        }

        protected void PropertyChange(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static PropertyInfo[] GetProperties(Type type) => type.GetProperties().Where(prop => prop.PropertyType.GetInterfaces().Any(face => face == typeof(IProperty))).ToArray();

        public virtual void Remove()
        {

        }

        public virtual void Initialize(IProperty property)
        {
            this.property = property;
            foreach (var prop in GetProperties(GetType()))
            {
                ((IProperty)prop.GetValue(this)).Initialize(this);
            }
        }
    }
}
