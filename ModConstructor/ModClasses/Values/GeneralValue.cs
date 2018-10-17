using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public class GeneralValue : IValue
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ChangeEventHandler<General> Change;
        public static event EventHandler AssignLinks;

        public string cachedKey = "";

        public IEnumerable<General> items
        {
            get
            {
                IEnumerable<General> items = new List<General>(General.all);
                foreach (var filter in filters) items = filter(items);
                return items;
            }
        }

        public List<Func<IEnumerable<General>, IEnumerable<General>>> filters = new List<Func<IEnumerable<General>, IEnumerable<General>>>();

        public IProperty property { get; set; }
        public string where => property.where;

        private General _item;
        public General item
        {
            get => _item;
            set
            {
                General before = _item;
                _item = value;
                cachedKey = _item?.key ?? cachedKey;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("target"));
                Change?.Invoke(before, value);
            }
        }

        public GeneralValue()
        {
            AssignLinks += AssignLink;
        }

        public GeneralValue(params Func<IEnumerable<General>, IEnumerable<General>>[] filters) : this()
        {
            foreach (var filter in filters) this.filters.Add(filter);
        }

        void AssignLink(object sender, EventArgs e)
        {
            item = General.all.FirstOrDefault(i => i.key == cachedKey);
            if (!String.IsNullOrWhiteSpace(cachedKey) && item == null) Message.Inform(MainWindow.instance, "Ошибка", $"Идентификатор {cachedKey} класса в {where} не найден.");
            AssignLinks -= AssignLink;
        }

        public static void LaunchAssign()
        {
            AssignLinks?.Invoke(null, null);
        }

        public XObject Pack(string name)
        {
            return new XAttribute(name, item.key);
        }

        public XElement PackElement(string name)
        {
            return new XElement(name, item.key);
        }

        public void Restore(XAttribute data)
        {
            cachedKey = data.Value;
        }

        public void Restore(XElement data)
        {
            cachedKey = data.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is GeneralValue)
            {
                GeneralValue gv = obj as GeneralValue;
                return gv.item == item;
            }
            else return false;
        }

        public static Func<IEnumerable<General>, IEnumerable<General>> Except(params General[] excepted) => (input) => input.Where(e => !excepted.Contains(e));
        public static Func<IEnumerable<General>, IEnumerable<General>> ChildOf(General parent, bool includeParent) => (input) => input.Where(e => e.IsChildOf(parent) || e == parent);

        public void Remove()
        {

        }

        public void Initialize(IProperty property)
        {
            this.property = property;
        }
    }
}
