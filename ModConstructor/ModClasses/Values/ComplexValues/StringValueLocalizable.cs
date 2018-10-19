using ModConstructor.ModClasses.Values.SimpleValues;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values.ComplexValues
{
    public class StringValueLocalizable : ComplexValue, IString
    {
        public enum Language
        {
            English,
            Deutsch,
            Italian,
            French,
            Spanish,
            Russian,
            Chinese,
            Brazilian,
            Polish
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

        public SingleProperty<StringValue> En { get; } = new SingleProperty<StringValue>(nameof(En), typeof(StringValueLocalizable), () => "", true);
        public SingleProperty<StringValue> De { get; } = new SingleProperty<StringValue>(nameof(De), typeof(StringValueLocalizable), () => "", true);
        public SingleProperty<StringValue> It { get; } = new SingleProperty<StringValue>(nameof(It), typeof(StringValueLocalizable), () => "", true);
        public SingleProperty<StringValue> Fr { get; } = new SingleProperty<StringValue>(nameof(Fr), typeof(StringValueLocalizable), () => "", true);
        public SingleProperty<StringValue> Es { get; } = new SingleProperty<StringValue>(nameof(Es), typeof(StringValueLocalizable), () => "", true);
        public SingleProperty<StringValue> Ru { get; } = new SingleProperty<StringValue>(nameof(Ru), typeof(StringValueLocalizable), () => "", true);
        public SingleProperty<StringValue> Ch { get; } = new SingleProperty<StringValue>(nameof(Ch), typeof(StringValueLocalizable), () => "", true);
        public SingleProperty<StringValue> Br { get; } = new SingleProperty<StringValue>(nameof(Br), typeof(StringValueLocalizable), () => "", true);
        public SingleProperty<StringValue> Po { get; } = new SingleProperty<StringValue>(nameof(Po), typeof(StringValueLocalizable), () => "", true);

        public string FromLanguage(Language lang)
        {
            switch (lang)
            {
                case Language.English:
                    return En.value;
                case Language.Deutsch:
                    return De.value;
                case Language.Italian:
                    return It.value;
                case Language.French:
                    return Fr.value;
                case Language.Spanish:
                    return Es.value;
                case Language.Russian:
                    return Ru.value;
                case Language.Chinese:
                    return Ch.value;
                case Language.Brazilian:
                    return Br.value;
                case Language.Polish:
                    return Po.value;
                default:
                    return "";
            }
        }

        public string localized => FromLanguage(language);

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
            En.PropertyChanged += delegate { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized")); };
            De.PropertyChanged += delegate { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized")); };
            It.PropertyChanged += delegate { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized")); };
            Fr.PropertyChanged += delegate { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized")); };
            Es.PropertyChanged += delegate { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized")); };
            Ru.PropertyChanged += delegate { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized")); };
            Ch.PropertyChanged += delegate { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized")); };
            Br.PropertyChanged += delegate { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized")); };
            Po.PropertyChanged += delegate { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("localized")); };
        }

        public override bool Equals(object obj)
        {
            if (obj is StringValueLocalizable)
            {
                StringValueLocalizable svl = obj as StringValueLocalizable;
                return svl.En.Equals(En) && svl.De.Equals(De) && svl.It.Equals(It) && svl.Fr.Equals(Fr) && svl.Es.Equals(Es) && svl.Ru.Equals(Ru) && svl.Ch.Equals(Ch) && svl.Br.Equals(Br) && svl.Po.Equals(Po);
            }
            return false;
        }

        public string AsString()
        {
            return localized;
        }

        public override void Remove()
        {
            MainWindow.instance.mod.localization.Remove(this);
        }

        public override void Initialize(IProperty property)
        {
            base.Initialize(property);
            MainWindow.instance.mod.localization.Add(this);
        }

        public override void Restore(XElement data)
        {
            base.Restore(data);
        }

        public virtual string TakeLocalizationString(Language lang) => $"{where}={FromLanguage(lang)}";

        public class Bastard : StringValueLocalizable
        {
            public string bastard;

            public Bastard(string bastard, bool big = false) : base(big)
            {
                this.bastard = bastard;
            }

            public override string TakeLocalizationString(Language lang)
            {
                return $"{bastard}.{(property.owner as General).className}={FromLanguage(lang)}";
            }
        }
    }
}
