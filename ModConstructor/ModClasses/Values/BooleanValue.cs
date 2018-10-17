using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public class BooleanValue : IValueBoolean
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ChangeEventHandler<bool> Change;

        public IProperty property { get; set; }
        public string where => property.where;

        private bool _value;
        public bool value
        {
            get => _value;
            set
            {
                bool before = _value;
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
                Change?.Invoke(before, value);
            }
        }

        public BooleanValue(bool value)
        {
            this.value = value;
        }

        public BooleanValue()
        {
            value = false;
        }

        public static implicit operator bool(BooleanValue bv) => bv.value;

        public static implicit operator BooleanValue(bool b) => new BooleanValue(b);

        public override bool Equals(object obj)
        {
            if (obj is BooleanValue)
            {
                BooleanValue bv = obj as BooleanValue;
                return bv.value == value;
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
            value = bool.Parse(data.Value);
        }

        public void Restore(XElement data)
        {
            value = bool.Parse(data.Value);
        }

        public bool AsBoolean()
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
