using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public class FloatValue : IValueFloat
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ChangeEventHandler<float> Change;

        public IProperty property { get; set; }
        public string where => property.where;

        private float _value;
        public float value
        {
            get => _value;
            set
            {
                float before = _value;
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
                Change?.Invoke(before, value);
            }
        }

        public FloatValue(float value)
        {
            this.value = value;
        }

        public FloatValue()
        {
            value = 0;
        }

        public static implicit operator float(FloatValue nv) => nv.value;

        public static implicit operator FloatValue(float f) => new FloatValue(f);

        public override bool Equals(object obj)
        {
            if (obj is FloatValue)
            {
                FloatValue fv = obj as FloatValue;
                return fv.value == value;
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
            value = float.Parse(data.Value);
        }

        public void Restore(XElement data)
        {
            value = float.Parse(data.Value);
        }

        public float AsFloat()
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
