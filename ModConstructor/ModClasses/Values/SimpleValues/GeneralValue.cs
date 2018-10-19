using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values.SimpleValues
{
    public class GeneralValue : SimpleValue<string>
    {
        private static event PropertyChangedEventHandler AssignLinks;

        public override string value
        {
            get => _value;
            set
            {
                if (base.value.Equals(value)) return;
                base.value = value;
                item = General.all.FirstOrDefault(i => i.key == value);
                if (!String.IsNullOrWhiteSpace(value) && item == null) Message.Inform(MainWindow.instance, "Ошибка", $"Идентификатор класса {value} в {where} не найден.");
            }
        }

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

        private General _item;
        public General item
        {
            get => _item;
            set
            {
                _item = value;
                this.value = _item?.key ?? "";
                PropertyChange("item");
            }
        }

        public GeneralValue() : base()
        {
            AssignLinks += AssignLink;
        }

        public GeneralValue(string value) : base(value)
        {
            AssignLinks += AssignLink;
        }

        public GeneralValue(params Func<IEnumerable<General>, IEnumerable<General>>[] filters) : this()
        {
            foreach (var filter in filters) this.filters.Add(filter);
        }

        private void AssignLink(object sender, PropertyChangedEventArgs e)
        {
            item = General.all.FirstOrDefault(i => i.key == value);
            if (!String.IsNullOrWhiteSpace(value) && item == null) Message.Inform(MainWindow.instance, "Ошибка", $"Идентификатор класса {value} в {where} не найден.");
            AssignLinks -= AssignLink;
        }

        public static void InitializeAssign()
        {
            AssignLinks?.Invoke(null, null);
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

        public override string AsString()
        {
            return value;
        }

        public static Func<IEnumerable<General>, IEnumerable<General>> Except(params General[] excepted) => (input) => input.Where(e => !excepted.Contains(e));
        public static Func<IEnumerable<General>, IEnumerable<General>> ChildOf(General parent, bool includeParent) => (input) => input.Where(e => e.IsChildOf(parent) || e == parent);
    }
}
