using ModConstructor.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace ModConstructor.ModClasses
{
    public interface IValue : INotifyPropertyChanged
    {
        XObject Pack(string name);
        XElement PackElement(string name);
        void Restore(XAttribute data);
        void Restore(XElement data);
    }

    public interface IValueString : IValue
    {
        string AsString();
    }

    public interface IValueFloat : IValue
    {
        float AsFloat();
    }

    public interface IValueBoolean : IValue
    {
        bool AsBoolean();
    }

    public class EnumerableValue : IValueFloat
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event General.ChangeEventHandler<int> Change;

        public static Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();

        public string name { get; }

        private int _value = 0;
        public int value
        {
            get => _value;
            set
            {
                int before = _value;
                _value = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("value"));
                Change?.Invoke(before, value);
            }
        }

        public string[] items => dictionary[name];

        public static void Register(string name, params string[] items)
        {
            dictionary[name] = items;
        }

        public static void Register(string name, Type enumerable)
        {
            dictionary[name] = Enum.GetNames(enumerable);
        }

        public EnumerableValue(string name, int def = 0)
        {
            if (!dictionary.ContainsKey(name)) throw new Exception("Given name not registered");
            _value = def;
            this.name = name;
        }

        public EnumerableValue(string name, int def = 0, params string[] items)
        {
            dictionary[name] = items;
            this.name = name;
            _value = def;
        }

        public override bool Equals(object obj)
        {
            if (obj is EnumerableValue)
            {
                EnumerableValue ev = obj as EnumerableValue;
                return ev.name == name && ev.value == value;
            }
            else return false;
        }

        public override string ToString()
        {
            if (value >= 0 && value < items.Length) return items[value];
            else return "{Enumerable value error}";
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
            value = int.Parse(data.Value);
        }

        public void Restore(XElement data)
        {
            value = int.Parse(data.Value);
        }

        public float AsFloat()
        {
            return value;
        }

        public static implicit operator int(EnumerableValue value) => value.value;
    }

    public class StringValue : IValueString
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event General.ChangeEventHandler<string> Change;

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
    }

    public class StringValueLocalizable : IValue, IValueString
    {
        public enum Language
        {
            English,
            Deutch,
            Italiano,
            French,
            Espanian,
            Russian,
            Chinese,
            Brasilian,
            Polska
        }

        public delegate void LanguageChangedHandler(Language newLanguage);

        public new event PropertyChangedEventHandler PropertyChanged;
        public static event LanguageChangedHandler LanguageChanged;

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

        private Language _language;
        public Language language
        {
            get => _language;
            private set
            {
                _language = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("language"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized"));
            }
        }

        private string _En = "";
        public string En
        {
            get => _En;
            set
            {
                _En = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("En"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized"));
            }
        }

        private string _De = "";
        public string De
        {
            get => _De;
            set
            {
                _De = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("De"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized"));
            }
        }

        private string _It = "";
        public string It
        {
            get => _It;
            set
            {
                _It = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("It"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized"));
            }
        }

        private string _Fr = "";
        public string Fr
        {
            get => _Fr;
            set
            {
                _Fr = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Fr"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized"));
            }
        }

        private string _Es = "";
        public string Es
        {
            get => _Es;
            set
            {
                _Es = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Es"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized"));
            }
        }

        private string _Ru = "";
        public string Ru
        {
            get => _Ru;
            set
            {
                _Ru = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ru"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized"));
            }
        }

        private string _Ch = "";
        public string Ch
        {
            get => _Ch;
            set
            {
                _Ch = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ch"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized"));
            }
        }

        private string _Br = "";
        public string Br
        {
            get => _Br;
            set
            {
                _Br = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Br"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized"));
            }
        }

        private string _Po = "";
        public string Po
        {
            get => _Po;
            set
            {
                _Po = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Po"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized"));
            }
        }

        public string localized
        {
            get
            {
                switch (language)
                {
                    case Language.English:
                        return En;
                    case Language.Russian:
                        return Ru;
                    case Language.French:
                        return Fr;
                    case Language.Deutch:
                        return De;
                    default:
                        return "";
                }
            }
        }

        public static void SetLanguage(Language newLanguage)
        {
            LanguageChanged?.Invoke(newLanguage);
        }

        void OnlanguageChanged(Language newLanguage)
        {
            language = newLanguage;
        }

        public StringValueLocalizable(bool big = false)
        {
            LanguageChanged += OnlanguageChanged;
            this.big = big;
        }

        public override bool Equals(object obj)
        {
            if (obj is StringValueLocalizable)
            {
                StringValueLocalizable svl = obj as StringValueLocalizable;
                return svl.En == En && svl.De == De && svl.It == It && svl.Fr == Fr && svl.Es == Es && svl.Ru == Ru && svl.Ch == Ch && svl.Br == Br && svl.Po == Po;
            }
            return false;
        }

        public XObject Pack(string name)
        {
            return PackElement(name);
        }

        public XElement PackElement(string name)
        {
            return new XElement(name,
                new XAttribute("En", En),
                new XAttribute("De", De),
                new XAttribute("It", It),
                new XAttribute("Fr", Fr),
                new XAttribute("Es", Es),
                new XAttribute("Ru", Ru),
                new XAttribute("Ch", Ch),
                new XAttribute("Br", Br),
                new XAttribute("Po", Po)
                );
        }

        public void Restore(XAttribute data)
        {
            
        }

        public void Restore(XElement data)
        {
            En = data.Attribute("En")?.Value ?? data.Element("En")?.Value ?? "";
            De = data.Attribute("De")?.Value ?? data.Element("De")?.Value ?? "";
            It = data.Attribute("It")?.Value ?? data.Element("It")?.Value ?? "";
            Fr = data.Attribute("Fr")?.Value ?? data.Element("Fr")?.Value ?? "";
            Es = data.Attribute("Es")?.Value ?? data.Element("Es")?.Value ?? "";
            Ru = data.Attribute("Ru")?.Value ?? data.Element("Ru")?.Value ?? "";
            Ch = data.Attribute("Ch")?.Value ?? data.Element("Ch")?.Value ?? "";
            Br = data.Attribute("Br")?.Value ?? data.Element("Br")?.Value ?? "";
            Po = data.Attribute("Po")?.Value ?? data.Element("Po")?.Value ?? "";
        }

        public string AsString()
        {
            return localized;
        }

        public class Presenter : INotifyPropertyChanged
        {
            public new event PropertyChangedEventHandler PropertyChanged;

            public StringValueLocalizable owner;

            private string _header;
            public string header
            {
                get => _header;
                set
                {
                    _header = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("header"));
                }
            }

            public string En { get => owner.En; set => owner.En = value; }
            public string De { get => owner.De; set => owner.De = value; }
            public string It { get => owner.It; set => owner.It = value; }
            public string Fr { get => owner.Fr; set => owner.Fr = value; }
            public string Es { get => owner.Es; set => owner.Es = value; }
            public string Ru { get => owner.Ru; set => owner.Ru = value; }
            public string Ch { get => owner.Ch; set => owner.Ch = value; }
            public string Br { get => owner.Br; set => owner.Br = value; }
            public string Po { get => owner.Po; set => owner.Po = value; }

            public Presenter(General owner, Property<StringValueLocalizable> property)
            {
                header = $"{owner.className}.{property.shortname}";
                owner.PropertyChanged += ownerChanged;
                property.PropertyChanged += propertyChanged;
                this.owner = property.value;
            }

            private void ownerChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "className") PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("header"));
            }

            private void propertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "En" || e.PropertyName == "Ru" || e.PropertyName == "Fr" || e.PropertyName == "De") PropertyChanged?.Invoke(this, e);
                else if (e.PropertyName == "shortname") PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("header"));
            }
        }
    }

    public class NumberValue : IValueFloat
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event General.ChangeEventHandler<int> Change;

        private int _value;
        public int value
        {
            get => _value;
            set
            {
                int before = _value; 
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
                Change?.Invoke(before, value);
            }
        }

        public NumberValue(int value)
        {
            this.value = value;
        }

        public NumberValue()
        {
            value = 0;
        }

        public static implicit operator int(NumberValue nv) => nv.value;

        public static implicit operator NumberValue(int n) => new NumberValue(n);

        public override bool Equals(object obj)
        {
            if (obj is NumberValue)
            {
                NumberValue nv = obj as NumberValue;
                return nv.value == value;
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
            value = int.Parse(data.Value);
        }

        public void Restore(XElement data)
        {
            value = int.Parse(data.Value);
        }

        public float AsFloat()
        {
            return value;
        }
    }

    public class BooleanValue : IValueBoolean
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event General.ChangeEventHandler<bool> Change;

        private bool _value;
        public bool value
        {
            get => _value;
            set
            {
                bool before = _value;
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
                Change?.Invoke(before, value);
            }
        }

        public BooleanValue(bool value)
        {
            this.value = value;
        }

        public BooleanValue()
        {
            value = false;
        }

        public static implicit operator bool(BooleanValue bv) => bv.value;

        public static implicit operator BooleanValue(bool b) => new BooleanValue(b);

        public override bool Equals(object obj)
        {
            if (obj is BooleanValue)
            {
                BooleanValue bv = obj as BooleanValue;
                return bv.value == value;
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
            value = bool.Parse(data.Value);
        }

        public void Restore(XElement data)
        {
            value = bool.Parse(data.Value);
        }

        public bool AsBoolean()
        {
            return value;
        }
    }

    public class FloatValue : IValueFloat
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event General.ChangeEventHandler<float> Change;

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
    }

    public class SpriteValue : IValue
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event General.ChangeEventHandler<Bitmap> Change;

        private Bitmap _texture = new Bitmap(16, 16);
        public Bitmap texture
        {
            get => _texture;
            set
            {
                Bitmap before = _texture;
                _texture = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("texture"));
                source = Paint.BitmapToSource(_texture);
                Change?.Invoke(before, value);
            }
        }

        private BitmapSource _source;
        public BitmapSource source
        {
            get => _source;
            set
            {
                _source = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("source"));
            }
        }

        private bool _scale = true;
        public bool scale
        {
            get => _scale;
            set
            {
                _scale = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("scale"));
            }
        }

        public byte[] ToByteArray()
        {
            byte[] result = new byte[texture.Width * texture.Height * 4];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    Color color = texture.GetPixel(x, y);
                    result[x * texture.Height * 4 + y * 4 + 0] = color.R;
                    result[x * texture.Height * 4 + y * 4 + 1] = color.G;
                    result[x * texture.Height * 4 + y * 4 + 2] = color.B;
                    result[x * texture.Height * 4 + y * 4 + 3] = color.A;
                }
            }
            return result;
        }

        public Bitmap GetScaled()
        {
            if (!scale) return texture;

            Bitmap result = new Bitmap(texture.Width * 2, texture.Height * 2);
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    Color col = texture.GetPixel(x, y);
                    result.SetPixel(x * 2, y * 2, col);
                    result.SetPixel(x * 2 + 1, y * 2, col);
                    result.SetPixel(x * 2, y * 2 + 1, col);
                    result.SetPixel(x * 2 + 1, y * 2 + 1, col);
                }
            }
            return result;
        }

        public XObject Pack(string name)
        {
            return PackElement(name);
        }

        public XElement PackElement(string name)
        {
            return new XElement(name,
                new XAttribute("width", texture.Width),
                new XAttribute("height", texture.Height),
                new XAttribute("scale", scale),
                new XAttribute("data", String.Join(" ", ToByteArray().Select(b => String.Format("{0:X}", b))))
                );
        }

        public void Restore(XAttribute data)
        {

        }

        public void Restore(XElement data)
        {
            int width = int.Parse(data.Attribute("width")?.Value ?? "16");
            int height = int.Parse(data.Attribute("height")?.Value ?? "16");
            string[] bytes = data.Attribute("data")?.Value.Split(' ') ?? new string[0];

            Bitmap result = new Bitmap(width, height);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    string r = bytes[x * result.Height * 4 + y * 4 + 0];
                    string g = bytes[x * result.Height * 4 + y * 4 + 1];
                    string b = bytes[x * result.Height * 4 + y * 4 + 2];
                    string a = bytes[x * result.Height * 4 + y * 4 + 3];
                    Color color = Color.FromArgb(Convert.ToByte(a, 16), Convert.ToByte(r, 16), Convert.ToByte(g, 16), Convert.ToByte(b, 16));
                    result.SetPixel(x, y, color);
                }
            }

            texture = result;

            scale = bool.Parse(data.Attribute("scale")?.Value ?? "true");
        }
    }

    public class GeneralValue : IValue
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event General.ChangeEventHandler<General> Change;
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
            if (!String.IsNullOrWhiteSpace(cachedKey) && item == null) Message.Inform(MainWindow.instance, "Ошибка", $"Идентификатор {cachedKey} не найден.");
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
    }

    public class MoneyValue : IValueFloat
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _copper;
        public int copper
        {
            get => _copper;
            set
            {
                _copper = value;
                if (_copper < 0)
                {
                    if (_silver > 0 || _golden > 0 || _platinum > 0)
                    {
                        silver--;
                        _copper = 99;
                    }
                    else _copper = 0;
                }
                if (_copper >= 100)
                {
                    if (_silver < 100 || _golden < 100 || _platinum < 100)
                    {
                        silver++;
                        _copper = 0;
                    }
                    else _copper = 100;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("copper"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
            }
        }

        private int _silver;
        public int silver
        {
            get => _silver;
            set
            {
                _silver = value;
                if (_silver < 0)
                {
                    if (_golden > 0 || _platinum > 0)
                    {
                        golden--;
                        _silver = 99;
                    }
                    else _silver = 0;
                }
                if (_silver >= 100)
                {
                    if (_golden < 100 || _platinum < 100)
                    {
                        golden++;
                        _silver = 0;
                    }
                    else _silver = 100;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("silver"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
            }
        }

        private int _golden;
        public int golden
        {
            get => _golden;
            set
            {
                _golden = value;
                if (_golden < 0)
                {
                    if (_platinum > 0)
                    {
                        platinum--;
                        _golden = 99;
                    }
                    else _golden = 0;
                }
                if (_golden >= 100)
                {
                    if (_platinum < 100)
                    {
                        platinum++;
                        _golden = 0;
                    }
                    else _golden = 100;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("golden"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
            }
        }

        private int _platinum;
        public int platinum
        {
            get => _platinum;
            set
            {
                _platinum = value;
                if (_platinum < 0)
                {
                    _platinum = 0;
                }
                if (_platinum > 100)
                {
                    _platinum = 100;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("platinum"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
            }
        }

        public int value
        {
            get => copper + silver * 100 + golden * 100 * 100 + platinum * 100 * 100 * 100;
            set
            {
                if (value < 0) value = 0;
                else if (value > 101010100) value = 101010100;

                _copper =   value % 100;
                _silver =   value / 100 % 100;
                _golden =   value / 100 / 100 % 100;
                _platinum = value / 100 / 100 / 100;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("copper"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("silver"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("golden"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("platinum"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
            }
        }

        public static implicit operator int(MoneyValue mv)
        {
            return mv.value;
        }

        public static implicit operator MoneyValue(int value)
        {
            return new MoneyValue(value);
        }

        public MoneyValue(int value = 0)
        {
            this.value = value;
        }

        public MoneyValue(int copper, int silver, int golden, int platinum)
        {
            this.copper = copper;
            this.silver = silver;
            this.golden = golden;
            this.platinum = platinum;
        }

        public float AsFloat()
        {
            return value;
        }

        public XObject Pack(string name)
        {
            return new XAttribute(name, value);
        }

        public XElement PackElement(string name)
        {
            return new XElement(name, value);
        }

        public void Restore(XAttribute data)
        {
            value = int.Parse(data.Value);
        }

        public void Restore(XElement data)
        {
            value = int.Parse(data.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is MoneyValue)
            {
                MoneyValue mv = obj as MoneyValue;
                return copper == mv.copper && silver == mv.silver && golden == mv.golden && platinum == mv.platinum;
            }
            return false;
        }
    }
}
