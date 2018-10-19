using ModConstructor.ModClasses.Values;
using ModConstructor.ModClasses.Values.ComplexValues;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses
{
    /*public interface IProperty : INotifyPropertyChanged
    {
        bool global { get; }
        bool changed { get; set; }
        string shortname { get; }
        string name { get; }
        ComplexValue owner { get; set; }
        string where { get; }

        void Remove();
        void Restore(XAttribute data);
        void Restore(XElement data);
        XObject Pack(string name);
        XElement PackElement(string name);
        void Initialize(ComplexValue owner);
    }*/

    public abstract class Property : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool global { get; }
        public string name { get; }

        public ComplexValue owner { get; protected set; }

        public string where => $"{owner.where}.{name}";
        public bool changed => values.Any(val => val.changed);

        public abstract IEnumerable<Value> values { get; }

        public abstract void Restore(XAttribute data);
        public abstract void Restore(XElement data);
        public abstract XObject Pack(string name);
        public abstract XElement PackElement(string name);
        public abstract string WhereAmI(Value val);

        public Property(string name, bool global)
        {
            this.name = name;
            this.global = global;
        }

        public virtual void Remove()
        {
            foreach (var value in values) value.Remove();
        }

        public virtual void Initialize(ComplexValue owner)
        {
            this.owner = owner;
            if (owner != null) owner.PropertyChanged += OwnerChanged;
            foreach (var value in values) value.Initialize(this);
        }

        public virtual void Save()
        {
            foreach (var value in values) value.Save();
        }

        public virtual void Revert()
        {
            foreach (var value in values) value.Revert();
        }

        public void PropertyChange(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void OwnerChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "where":
                    PropertyChange("where");
                    break;
            }
        }
    }

    public abstract class PropertySpecialized<T> : Property where T : Value
    {
        public Func<T> def;
        public Func<PropertySpecialized<T>, T, string> validator;

        public PropertySpecialized(string name, bool global, Func<T> def, Func<PropertySpecialized<T>, T, string> validator = null) : base(name, global)
        {
            this.def = def;
            this.validator = validator;
        }
    }

    /*public class PropertySolution : IProperty
    {
        public bool global { get; protected set; }

        private bool _changed = false;
        public bool changed
        {
            get => _changed;
            set
            {
                _changed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("changed"));
                if (global)
                {
                    if (changed)
                    {
                        if (!MainWindow.Dirty.Contains(this)) MainWindow.Dirty.Add(this);
                    }
                    else
                    {
                        if (MainWindow.Dirty.Contains(this)) MainWindow.Dirty.Remove(this);
                    }
                }
            }
        }

        public string shortname { get; protected set; }

        public string name { get; protected set; }

        public IValue owner { get; set; }

        public string where => $"{owner.where}.{shortname}";

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Initialize(IValue owner)
        {
            
        }

        public virtual XObject Pack(string name)
        {
            return new XAttribute(name, "PACKING NOT IMPLEMENTED");
        }

        public virtual XElement PackElement(string name)
        {
            return new XElement(name, "PACKING NOT IMPLEMENTED");
        }

        public virtual void Remove()
        {
            
        }

        public virtual void Restore(XAttribute data)
        {
            
        }

        public virtual void Restore(XElement data)
        {
            
        }

        public void PropertyChange(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }*/

    public sealed class SingleProperty<T> : PropertySpecialized<T> where T : Value
    {
        /*private class PropertyData
        {
            public Func<T> def;
            public Func<string, T, string> validator;

            public PropertyData(Func<T> def, Func<string, T, string> validator)
            {
                this.def = def;
                this.validator = validator;
            }
        }

        private static Dictionary<string, PropertyData> dictionary = new Dictionary<string, PropertyData>();

        public T def() => dictionary[name].def();
        public string Validate() => dictionary[name].validator?.Invoke(shortname, value) ?? "";
        public void SetValidator(Func<string, T, string> validator) => dictionary[name].validator = validator;

        private T _value;
        public T value
        {
            get => _value;
            set
            {
                T before = _value;
                if (_value != null)
                {
                    _value.Remove();
                    _value.PropertyChanged -= UpdateNotifier;
                }
                _value = value;
                if (_value != null)
                {
                    _value.Initialize(this);
                    _value.PropertyChanged += UpdateNotifier;
                }
                PropertyChange("value");
                ValueUpdated();
            }
        }

        private void ValueUpdated()
        {
            error = Validate();
            changed = !value.Equals(def());
        }

        public SingleProperty(string name, Type owner, Func<T> def, bool global = false, Func<string, T, string> validator = null)
        {
            shortname = name;
            string fullname = $"{owner.FullName}.{name}";
            this.name = fullname;
            this.global = global;

            dictionary[fullname] = new PropertyData(def, validator);

            _value = def();
            _value.PropertyChanged += UpdateNotifier;
            error = Validate();

            value.PropertyChanged += UpdateNotifier;
        }

        private void UpdateNotifier(object sender, PropertyChangedEventArgs e) => ValueUpdated();
        public static implicit operator T(SingleProperty<T> property) => property.value;
        public override string ToString() => value.ToString();
        public XObject Pack(string name) => value.Pack(name);
        public XElement PackElement(string name) => value.PackElement(name);
        public void Reset() => value = def();
        public override void Restore(XAttribute data) => value.Restore(data);
        public override void Restore(XElement data) => value.Restore(data);

        public override void Remove()
        {
            value.Remove();
            if (global)
            {
                MainWindow.Errors.Remove(this);
                MainWindow.Dirty.Remove(this);
            }
        }

        public override void Initialize(IValue owner)
        {
            this.owner = owner;
            value.Initialize(this);
        }*/
        public override IEnumerable<Value> values => new Value[] { value };

        public T value { get; }

        public SingleProperty(string name, bool global, Func<T> def, Func<PropertySpecialized<T>, T, string> validator = null) : base(name, global, def, validator)
        {

        }

        public override XObject Pack(string name)
        {
            return value.Pack(name);
        }

        public override XElement PackElement(string name)
        {
            return value.PackElement(name);
        }

        public override void Restore(XAttribute data)
        {
            value.Restore(data);
        }

        public override void Restore(XElement data)
        {
            value.Restore(data);
        }

        public override string WhereAmI(Value val)
        {
            return where;
        }
    }

    /*public interface IPropertyList : IProperty, INotifyCollectionChanged
    {
        void Add();
        void Clear();
    }

    public class PropertyList<T> : IPropertyList, IEnumerable<T> where T : IValue
    {
        private class PropertyData
        {
            public Func<T> def;
            public Func<string, T, string> validator;

            public PropertyData(Func<T> def, Func<string, T, string> validator)
            {
                this.def = def;
                this.validator = validator;
            }
        }

        private static Dictionary<string, PropertyData> dictionary = new Dictionary<string, PropertyData>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IValue owner { get; set; }

        public string shortname { get; }
        public string name { get; }
        public bool global { get; }

        public T def() => dictionary[name].def();
        public string validator(int index) => dictionary[name].validator?.Invoke(shortname, list[index]) ?? "";
        public void SetValidator(Func<string, T, string> validator) => dictionary[name].validator = validator;
        public string where => $"{owner.where}.{shortname}";

        private string _error = "";
        public string error
        {
            get => _error;
            set
            {
                if (_error == value) return;
                _error = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("error"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("hasError"));
                if (global)
                {
                    if (hasError)
                    {
                        if (!MainWindow.Errors.Contains(this)) MainWindow.Errors.Add(this);
                    }
                    else
                    {
                        if (MainWindow.Errors.Contains(this)) MainWindow.Errors.Remove(this);
                    }
                }
            }
        }

        public bool hasError => !String.IsNullOrWhiteSpace(error);

        public ObservableCollection<T> list { get; }

        private bool _changed = false;
        public bool changed
        {
            get => _changed;
            set
            {
                _changed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("changed"));
                if (global)
                {
                    if (changed)
                    {
                        if (!MainWindow.Dirty.Contains(this)) MainWindow.Dirty.Add(this);
                    }
                    else
                    {
                        if (MainWindow.Dirty.Contains(this)) MainWindow.Dirty.Remove(this);
                    }
                }
            }
        }

        public PropertyList(string name, Type owner, Func<T> def, bool global = false, Func<string, T, string> validator = null)
        {
            shortname = name;
            string fullname = $"{owner.FullName}.{name}";
            this.name = fullname;
            this.global = global;

            dictionary[fullname] = new PropertyData(def, validator);

            list = new ObservableCollection<T>();

            list.CollectionChanged += CollectionUpdated;
        }

        private void CollectionUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        (item as IValue).PropertyChanged += UpdateNotifier;
                    }
                    break;
                case NotifyCollectionChangedAction.Move:

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        (item as IValue).PropertyChanged -= UpdateNotifier;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:

                    break;
                case NotifyCollectionChangedAction.Reset:

                    break;
            }

            CollectionChanged?.Invoke(this, e);

            changed = list.Count > 0;
        }

        private void UpdateNotifier(object sender, PropertyChangedEventArgs e)
        {
            //ValueUpdated();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public override string ToString()
        {
            return $"elements: {list.Count}";
        }

        public void Reset()
        {
            for (int i = 0; i < list.Count; i++) list[i] = def();
        }

        public void Add()
        {
            T val = def();
            list.Add(val);
            val.Initialize(this);
        }

        public void Add(T val)
        {
            list.Add(val);
            val.Initialize(this);
        }

        public void Clear()
        {
            foreach (T val in list) val.Remove();
            list.Clear();
        }

        public void Delete(T val)
        {
            if (list.Contains(val))
            {
                val.Remove();
                list.Remove(val);
            }
        }

        public XObject Pack(string name)
        {
            return PackElement(name);
        }

        public XElement PackElement(string name)
        {
            return new XElement(name, list.Select(item => item.PackElement("element")));
        }

        public void Restore(XAttribute data)
        {
            
        }

        public void Restore(XElement data)
        {
            list.Clear();
            foreach (XElement el in data.Elements())
            {
                IValue val = (IValue)Activator.CreateInstance(typeof(T));
                val.Restore(el);
                list.Add((T)val);
                val.Initialize(this);
            }
        }

        public void Remove()
        {
            if (global)
            {
                MainWindow.Errors.Remove(this);
                MainWindow.Dirty.Remove(this);
            }
        }

        public void Initialize(IValue owner)
        {
            this.owner = owner;
        }
    }*/

    public sealed class ListProperty<T> : PropertySpecialized<T> where T : Value
    {
        public ObservableCollection<T> list = new ObservableCollection<T>();

        public override IEnumerable<Value> values => list;

        public ListProperty(string name, bool global, Func<T> def, Func<PropertySpecialized<T>, T, string> validator = null) : base(name, global, def, validator)
        {

        }

        public override XObject Pack(string name)
        {
            return PackElement(name);
        }

        public override XElement PackElement(string name)
        {
            return new XElement(name, values.Select(val => val.Pack("element")));
        }

        public override void Restore(XAttribute data)
        {
            Message.Inform(MainWindow.instance, "Ошибка", "Восстановление списка данных из аттрибута невозможно");
        }

        public override void Restore(XElement data)
        {
            foreach (var val in list) val.Remove();
            list.Clear();
            foreach (var dat in data.Elements())
            {
                T value = def();
                value.Restore(dat);
                value.Initialize(this);
                list.Add(value);
            }
        }

        public override string WhereAmI(Value val)
        {
            return $"{where}[{list.IndexOf((T)val)}]";
        }

        public void Add()
        {
            T val = def();
            val.Initialize(this);
            list.Add(val);
        }

        public void Append(T val)
        {
            if (val.initialized) return;
            val.Initialize(this);
            list.Add(val);
        }

        public void Remove(T val)
        {
            if (!list.Contains(val)) return;
            list.Remove(val);
        }

        public void Clear()
        {
            foreach (var val in list) val.Remove();
            list.Clear();
        }
    }

    public static class PropertyValidators
    {
        public static string ClassName<T>(PropertySpecialized<T> specialized, T property) where T : Value
        {
            string value = property.AsString();
            if (String.IsNullOrWhiteSpace(value)) return $"Необходимо указать {specialized.name}.";
            else if (Regex.IsMatch(value, @"[^a-zA-Z\d_]")) return $@"{specialized.name} может состоять только из латинских букв, цифр и знаков ""_"".";
            else if (Regex.IsMatch(value, @"^\d")) return $@"{specialized.name} не может начинаться с цифры.";
            else return "";
        }
    }
}
