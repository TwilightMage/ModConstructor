using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses
{
    public class Localizator : IValue
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _key;
        public string key
        {
            get => _key;
            set
            {
                _key = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("key"));
            }
        }

        private string _En;
        public string En
        {
            get => _En;
            set
            {
                _En = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("En"));
            }
        }

        private string _Ru;
        public string Ru
        {
            get => _Ru;
            set
            {
                _Ru = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ru"));
            }
        }

        private string _Fr;
        public string Fr
        {
            get => _Fr;
            set
            {
                _Fr = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Fr"));
            }
        }

        private string _De;
        public string De
        {
            get => _De;
            set
            {
                _De = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("De"));
            }
        }

        public XObject Pack(string name)
        {
            return PackElement(name);
        }

        public XElement PackElement(string name)
        {
            return new XElement(name,
                new XAttribute("key", key),
                new XAttribute("En", En),
                new XAttribute("Ru", Ru),
                new XAttribute("Fr", Fr),
                new XAttribute("De", De)
                );
        }

        public void Restore(XAttribute data)
        {
            key = data.Value;
            En = "";
            Ru = "";
            Fr = "";
            De = "";
        }

        public void Restore(XElement data)
        {
            key = data.Attribute("key")?.Value ?? data.Element("key")?.Value ?? "";
            key = data.Attribute("En")?.Value ?? data.Element("En")?.Value ?? "";
            key = data.Attribute("Ru")?.Value ?? data.Element("Ru")?.Value ?? "";
            key = data.Attribute("Fr")?.Value ?? data.Element("Fr")?.Value ?? "";
            key = data.Attribute("De")?.Value ?? data.Element("De")?.Value ?? "";
        }
    }
}
