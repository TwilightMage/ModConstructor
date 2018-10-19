using ModConstructor.ModClasses.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values.SimpleValues
{
    public class SimpleValue<T> : Value
    {
        public event ChangedEventHandler<T> ChangedSimple;

        protected T _value;
        public virtual T value
        {
            get => _value;
            set
            {
                T before = _value;
                _value = value;
                if (before.Equals(value)) return;
                ChangedSimple?.Invoke(before, value);
                PropertyChange("value");
                dirty = !value.Equals(saved);
                Change();
            }
        }

        public override bool changed => !value.Equals((property as PropertySpecialized<SimpleValue<T>>).def());

        private T _saved;
        public T saved
        {
            get => _saved;
            set
            {
                _saved = value;
                PropertyChange("saved");
            }
        }

        public SimpleValue(T value)
        {
            this.value = value;
        }

        public SimpleValue()
        {
            value = default(T);
        }

        public override bool Equals(object obj)
        {
            if (obj is SimpleValue<T>)
            {
                SimpleValue<T> sv = obj as SimpleValue<T>;
                return sv.value.Equals(value);
            }
            else return false;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public override XObject Pack(string name)
        {
            return new XAttribute(name, value.ToString());
        }

        public override XElement PackElement(string name)
        {
            return new XElement(name, value.ToString());
        }

        public override void Restore(XAttribute data)
        {
            value = (T)Convert.ChangeType(data.Value, typeof(T));
        }

        public override void Restore(XElement data)
        {
            value = (T)Convert.ChangeType(data.Value, typeof(T));
        }

        public override void Save()
        {
            saved = value;
            dirty = false;
        }

        public override void Revert()
        {
            value = saved;
        }

        protected virtual void ValueChanged(T before, T now)
        {

        }

        public static implicit operator T(SimpleValue<T> input) => input.value;
    }
}
