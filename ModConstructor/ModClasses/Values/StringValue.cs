using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public class StringValue : IValueString
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ChangeEventHandler<string> Change;

        public IProperty property { get; set; }
        public string where => property.where;

        private bool _big;
        public bool big
        {
            get => _big;
            set
            {
                _big = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("big"));
            }
        }

        private string _value;
        public string value
        {
            get => _value;
            set
            {
                string before = _value;
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
                Change?.Invoke(before, value);
            }
        }

        public StringValue(string value, bool big = false)
        {
            this.value = value;
            this.big = big;
        }

        public StringValue()
        {
            value = "";
        }

        public static implicit operator string(StringValue sv) => sv.value;

        public static implicit operator StringValue(string s) => new StringValue(s);

        public override bool Equals(object obj)
        {
            if (obj is StringValue)
            {
                StringValue sv = obj as StringValue;
                return sv.value == value;
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
            value = data.Value;
        }

        public void Restore(XElement data)
        {
            value = data.Value;
        }

        public string AsString()
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
