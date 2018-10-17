using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public class StringValueLocalizable : ValueSolution, IValueString
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

        public Property<StringValue> En { get; } = new Property<StringValue>(nameof(En), typeof(StringValueLocalizable), () => "", true);
        public Property<StringValue> De { get; } = new Property<StringValue>(nameof(De), typeof(StringValueLocalizable), () => "", true);
        public Property<StringValue> It { get; } = new Property<StringValue>(nameof(It), typeof(StringValueLocalizable), () => "", true);
        public Property<StringValue> Fr { get; } = new Property<StringValue>(nameof(Fr), typeof(StringValueLocalizable), () => "", true);
        public Property<StringValue> Es { get; } = new Property<StringValue>(nameof(Es), typeof(StringValueLocalizable), () => "", true);
        public Property<StringValue> Ru { get; } = new Property<StringValue>(nameof(Ru), typeof(StringValueLocalizable), () => "", true);
        public Property<StringValue> Ch { get; } = new Property<StringValue>(nameof(Ch), typeof(StringValueLocalizable), () => "", true);
        public Property<StringValue> Br { get; } = new Property<StringValue>(nameof(Br), typeof(StringValueLocalizable), () => "", true);
        public Property<StringValue> Po { get; } = new Property<StringValue>(nameof(Po), typeof(StringValueLocalizable), () => "", true);

        public string localized
        {
            get
            {
                switch (language)
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
    }
}
