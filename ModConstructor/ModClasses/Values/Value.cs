using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public abstract class Value
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ChangedEventHandler Changed;
        
        public IProperty property { get; set; }
        public string where => property.whereAmI(this);

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

        public virtual void Initialize(IProperty property)
        {
            this.property = property;
        }

        public abstract XObject Pack(string name);

        public abstract XElement PackElement(string name);

        public abstract void Restore(XAttribute data);

        public abstract void Restore(XElement data);

        public abstract void Save();

        public abstract void Revert();
    }
}
