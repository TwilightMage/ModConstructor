using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModConstructor.ModClasses
{
    class CompositeString : IList<object>, INotifyPropertyChanged
    {
        public string glue;
        private ObservableCollection<object> elements = new ObservableCollection<object>();

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count => elements.Count;
        public bool IsReadOnly => false;

        public object this[int index]
        {
            get => elements[index];
            set
            {
                elements[index] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
            }
        }

        public CompositeString(string glue = ".")
        {
            this.glue = glue;
        }

        public static implicit operator string(CompositeString cs) => String.Join(cs.glue, cs.elements);
        public override string ToString() => this;
        public string value => this;

        public int IndexOf(object item) => elements.IndexOf(item);

        public void Insert(int index, object item)
        {
            if (item.GetType().GetInterfaces().Any(face => face == typeof(INotifyPropertyChanged))) (item as INotifyPropertyChanged).PropertyChanged += ItemChanged;
            elements.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            object item = elements[index];
            if (item.GetType().GetInterfaces().Any(face => face == typeof(INotifyPropertyChanged))) (item as INotifyPropertyChanged).PropertyChanged -= ItemChanged;
            elements.RemoveAt(index);
        }

        public void Add(object item)
        {
            if (item.GetType().GetInterfaces().Any(face => face == typeof(INotifyPropertyChanged))) (item as INotifyPropertyChanged).PropertyChanged += ItemChanged;
            elements.Add(item);
        }

        public void Clear()
        {
            foreach (var item in elements) if (item.GetType().GetInterfaces().Any(face => face == typeof(INotifyPropertyChanged))) (item as INotifyPropertyChanged).PropertyChanged -= ItemChanged;
            elements.Clear();
        }

        public bool Contains(object item) => elements.Contains(item);
        public void CopyTo(object[] array, int arrayIndex) => elements.CopyTo(array, arrayIndex);

        public bool Remove(object item)
        {
            if (item != null && item.GetType().GetInterfaces().Any(face => face == typeof(INotifyPropertyChanged))) (item as INotifyPropertyChanged).PropertyChanged -= ItemChanged;
            return elements.Remove(item);
        }

        public IEnumerator<object> GetEnumerator() => elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => elements.GetEnumerator();

        private void ItemChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
        }
    }
}
