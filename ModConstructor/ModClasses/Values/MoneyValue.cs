using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public class MoneyValue : IValueFloat
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IProperty property { get; set; }
        public string where => property.where;

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

                _copper = value % 100;
                _silver = value / 100 % 100;
                _golden = value / 100 / 100 % 100;
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

        public void Remove()
        {

        }

        public void Initialize(IProperty property)
        {
            this.property = property;
        }
    }
}
