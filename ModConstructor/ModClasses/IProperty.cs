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
    public interface IProperty : IValue
    {
        string error { get; set; }
        bool hasError { get; }
        bool global { get; }
        bool changed { get; set; }
        string shortname { get; }
        string name { get; }

        void Remove();
        void Restore(XAttribute data);
        void Restore(XElement data);
    }

    public sealed class Property<T> : IProperty where T : IValue
    {
        private class PropertyData
        {
            public Func<T> def;
            public Func<string, T, string> validator;
            public Func<Property<T>, string[]> where;

            public PropertyData(Func<T> def, Func<string, T, string> validator, Func<Property<T>, string[]> where)
            {
                this.def = def;
                this.validator = validator;
                this.where = where;
            }
        }

        private static Dictionary<string, PropertyData> dictionary = new Dictionary<string, PropertyData>();

        public event PropertyChangedEventHandler PropertyChanged;

        public string shortname { get; }
        public string name { get; }
        public bool global { get; }

        public T def() => dictionary[name].def();
        public string validator() => dictionary[name].validator?.Invoke(shortname, value) ?? "";
        public void SetValidator(Func<string, T, string> validator) => dictionary[name].validator = validator;
        public string[] where() => dictionary[name].where(this);

        private string _error = "Unknown error";
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

        private T _value;
        public T value
        {
            get => _value;
            set
            {
                T before = _value;
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
                ValueUpdated();
            }
        }

        private void ValueUpdated()
        {
            error = validator();
            changed = !value.Equals(def());
        }

        private bool _dirty = false;
        public bool changed
        {
            get => _dirty;
            set
            {
                _dirty = value;
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

        public Property(string name, Type owner, Func<T> def, Func<Property<T>, string[]> where, bool global = false, Func<string, T, string> validator = null)
        {
            shortname = name;
            string fullname = $"{owner.FullName}.{name}";
            this.name = fullname;
            this.global = global;

            dictionary[fullname] = new PropertyData(def, validator, where);

            value = def();

            value.PropertyChanged += UpdateNotifier;
        }

        private void UpdateNotifier(object sender, PropertyChangedEventArgs e)
        {
            ValueUpdated();
        }

        public static implicit operator T(Property<T> property)
        {
            return property.value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public void Reset()
        {
            value = def();
        }

        public XObject Pack(string name)
        {
            return value.Pack(shortname);
        }

        public XElement PackElement(string name)
        {
            return value.PackElement(name);
        }

        public void Restore(XAttribute data)
        {
            value.Restore(data);
        }

        public void Restore(XElement data)
        {
            value.Restore(data);
        }

        public void Remove()
        {
            if (global)
            {
                MainWindow.Errors.Remove(this);
                MainWindow.Dirty.Remove(this);
            }
        }
    }

    public interface IPropertyList : IProperty, INotifyCollectionChanged
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
            public Func<PropertyList<T>, string[]> where;

            public PropertyData(Func<T> def, Func<string, T, string> validator, Func<PropertyList<T>, string[]> where)
            {
                this.def = def;
                this.validator = validator;
                this.where = where;
            }
        }

        private static Dictionary<string, PropertyData> dictionary = new Dictionary<string, PropertyData>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public string shortname { get; }
        public string name { get; }
        public bool global { get; }

        public T def() => dictionary[name].def();
        public string validator(int index) => dictionary[name].validator?.Invoke(shortname, list[index]) ?? "";
        public void SetValidator(Func<string, T, string> validator) => dictionary[name].validator = validator;
        public string[] where() => dictionary[name].where(this);

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

        public PropertyList(string name, Type owner, Func<T> def, Func<PropertyList<T>, string[]> where, bool global = false, Func<string, T, string> validator = null)
        {
            shortname = name;
            string fullname = $"{owner.FullName}.{name}";
            this.name = fullname;
            this.global = global;

            dictionary[fullname] = new PropertyData(def, validator, where);

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

            /*asd
            error = validator();
            dirty = !list.Equals(def());*/
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
            list.Add(def());
        }

        public void Add(T val)
        {
            list.Add(val);
        }

        public void Fill(PropertyList<T> proplist)
        {
            foreach (var item in proplist) list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public void Delete(T val)
        {
            list.Remove(val);
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
    }

    public static class PropertyValidators
    {
        public static string ClassName<T>(string name, T property) where T : IValueString
        {
            string value = property.AsString();
            if (String.IsNullOrWhiteSpace(value)) return $"Необходимо указать {name}.";
            else if (Regex.IsMatch(value, @"[^a-zA-Z\d_]")) return $@"{name} может состоять только из латинских букв, цифр и знаков ""_"".";
            else if (Regex.IsMatch(value, @"^\d")) return $@"{name} не может начинаться с цифры.";
            else return "";
        }
    }
}
