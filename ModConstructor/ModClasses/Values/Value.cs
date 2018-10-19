using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public delegate void ChangedEventHandler<T>(T before, T now);
    public delegate void ChangedEventHandler();

    public abstract class Value
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ChangedEventHandler Changed;
        
        public Property property { get; set; }
        public string where => property.WhereAmI(this);
        public bool initialized { get; protected set; }

        protected string _error = "";
        public string error
        {
            get => _error;
            set
            {
                if (_error == value) return;
                _error = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("error"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("hasError"));
                if (property.global)
                {
                    if (hasError)
                    {
                        MainWindow.Errors.AddUnique(this);
                    }
                    else
                    {
                        MainWindow.Errors.Remove(this);
                    }
                }
            }
        }

        public bool hasError => !String.IsNullOrWhiteSpace(error);

        public abstract bool changed { get; }

        protected bool _dirty = false;
        public bool dirty
        {
            get => _dirty;
            set
            {
                if (_dirty == value) return;
                _dirty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("dirty"));
                if (property.global)
                {
                    if (dirty)
                    {
                        MainWindow.Dirty.AddUnique(this);
                    }
                    else
                    {
                        MainWindow.Dirty.Remove(this);
                    }
                }
            }
        }

        public void PropertyChange(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void Change()
        {
            Changed?.Invoke();
        }

        public virtual void Remove()
        {

        }

        public virtual void Initialize(Property property)
        {
            this.property = property;
            initialized = true;
        }

        public virtual string AsString() => "";
        public virtual int AsNumber() => 0;
        public virtual float AsFloat() => 0;
        public virtual bool AsBoolean() => false;

        public abstract XObject Pack(string name);

        public abstract XElement PackElement(string name);

        public abstract void Restore(XAttribute data);

        public abstract void Restore(XElement data);

        public abstract void Save();

        public abstract void Revert();
    }
}
