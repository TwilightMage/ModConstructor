using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public class NumberValue : IValueNumber
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ChangeEventHandler<int> Change;

        public IProperty property { get; set; }
        public string where => property.where;

        private int _value;
        public int value
        {
            get => _value;
            set
            {
                int before = _value;
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
                Change?.Invoke(before, value);
            }
        }

        public NumberValue(int value)
        {
            this.value = value;
        }

        public NumberValue()
        {
            value = 0;
        }

        public static implicit operator int(NumberValue nv) => nv.value;

        public static implicit operator NumberValue(int n) => new NumberValue(n);

        public override bool Equals(object obj)
        {
            if (obj is NumberValue)
            {
                NumberValue nv = obj as NumberValue;
                return nv.value == value;
            }
            else return false;
        }

        public override string ToString()
        {
            return value.ToString();
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

        public void Remove()
        {

        }

        public void Initialize(IProperty property)
        {
            this.property = property;
        }
    }
}
