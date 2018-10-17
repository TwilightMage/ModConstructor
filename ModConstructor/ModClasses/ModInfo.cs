using ModConstructor.ModClasses.Values;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses
{
    [Serializable]
    public class ModInfo : ValueSolution
    {
        public class Version
        {
            public int[] versions;

            public Version(params int[] versions)
            {
                this.versions = versions;
            }

            public void Increment(int pos)
            {
                versions[pos]++;
            }

            public override string ToString()
            {
                return String.Join(".", versions);
            }

            public static Version Parse(string input)
            {
                Version result = new Version();
                string[] parts = input.Split('.');
                result.versions = new int[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                {
                    result.versions[i] = int.Parse(parts[i]);
                }
                return result;
            }

            public static bool operator >(Version A, Version B)
            {
                for (int i = 0; i < Math.Min(A.versions.Length, B.versions.Length); i++)
                {
                    if (A.versions[i] > B.versions[i]) return true;
                }
                return false;
            }

            public static bool operator <(Version A, Version B)
            {
                for (int i = 0; i < Math.Min(A.versions.Length, B.versions.Length); i++)
                {
                    if (A.versions[i] < B.versions[i]) return true;
                }
                return false;
            }
        }

        public override string where => name.value;

        MainWindow window;

        public Property<StringValue> name             { get; } = new Property<StringValue>(     nameof(name),          typeof(ModInfo), () => "",                                   true);
                                                                                                                                                                                   
        public PropertyList<StringValue> authors      { get; } = new PropertyList<StringValue>( nameof(authors),       typeof(ModInfo), () => "",                                   true);
        public Property<StringValue> version          { get; } = new Property<StringValue>(     nameof(version),       typeof(ModInfo), () => (new Version(0, 0, 0, 1)).ToString(), true);
        public Property<StringValue> displayName      { get; } = new Property<StringValue>(     nameof(displayName),   typeof(ModInfo), () => "",                                   true);
        public Property<StringValue> homePage         { get; } = new Property<StringValue>(     nameof(homePage),      typeof(ModInfo), () => "",                                   true);
        public Property<BooleanValue> hideCode        { get; } = new Property<BooleanValue>(    nameof(hideCode),      typeof(ModInfo), () => true,                                 true);
        public Property<BooleanValue> hideResources   { get; } = new Property<BooleanValue>(    nameof(hideResources), typeof(ModInfo), () => true,                                 true);
        public Property<BooleanValue> includeSource   { get; } = new Property<BooleanValue>(    nameof(includeSource), typeof(ModInfo), () => false,                                true);
        public PropertyList<StringValue> buildIgnores { get; } = new PropertyList<StringValue>( nameof(buildIgnores),  typeof(ModInfo), () => "",                                   true);
        public Property<StringValue> description      { get; } = new Property<StringValue>(     nameof(description),   typeof(ModInfo), () => new StringValue("", true),            true);
                                                                                                                                                                                   
        public PropertyList<Item> items               { get; } = new PropertyList<Item>(        nameof(items),         typeof(ModInfo), () => new Item(),                           true);

        public ObservableCollection<StringValueLocalizable> localization { get; } = new ObservableCollection<StringValueLocalizable>();

        private int _curLang = 0;
        public int curLang
        {
            get => _curLang;
            set
            {
                _curLang = value;
                PropertyChange("curLang");
                StringValueLocalizable.SetLanguage((StringValueLocalizable.Language)curLang);
            }
        }

        public ModInfo(string name)
        {
            window = MainWindow.instance;

            this.name.value = name;
        }

        public ModInfo(CreateModInfo info)
        {
            window = MainWindow.instance;

            name.value = info.modName.value;

            foreach (var author in info.authors) authors.Add(new StringValue(author));
            version.value = (new Version(0, 0, 0, 1)).ToString();
            displayName.value = info.displayName.value;
            homePage.value = info.homePage.value;
            hideCode.value = true;
            hideResources.value = true;
            includeSource.value = false;
            description.value = info.description.value;
        }

        public void PushMod()
        {
            XElement output = new XElement("mod");
            foreach (var prop in GetType().GetProperties().Where(prop => prop.PropertyType.GetInterfaces().Any(face => face == typeof(IProperty))))
            {
                IProperty property = (IProperty)prop.GetValue(this);
                if (property.changed) output.Add(property.Pack(prop.Name));
            }

            using (StreamWriter sw = new StreamWriter(window.modFile))
            {
                sw.Write(output);
            }
        }

        public void PushBuild()
        {
            using (StreamWriter sw = new StreamWriter(window.modBuildFile))
            {
                sw.WriteLine($"author = {String.Join(", ", authors)}");
                sw.WriteLine($"version = {version}");
                sw.WriteLine($"displayName = {displayName}");
                sw.WriteLine($"homepage = {homePage}");
                sw.WriteLine($"hideCode = {hideCode}");
                sw.WriteLine($"hideResources = {hideResources}");
                sw.WriteLine($"includeSource = {includeSource}");
                sw.WriteLine($"buildIgnore = {String.Join(", ", buildIgnores)}");
                sw.WriteLine($"includePDB = true");
            }
        }

        public void PushDescription()
        {
            using (StreamWriter sw = new StreamWriter(window.modDescriptionFile))
            {
                sw.Write(description);
            }
        }

        private string ParseScript(Match match)
        {
            switch(match.Groups[1].Value)
            {
                case "usings":
                    return "using System;";
                case "modName":
                    return name.value;
                case "notDedServLoadCode":

                    break;
                case "dedServLoadCode":

                    break;
                case "notDedServUnloadCode":

                    break;
                case "dedServUnloadCode":
                    break;
            }
            return "";
        }

        public void PushScript()
        {
            using (StreamWriter sw = new StreamWriter(window.modScript))
            {
                sw.Write(Regex.Replace(Properties.Resources.PatternScript, @"{\$(.*)}", ParseScript));
            }
        }

        public void PushWorld()
        {

        }

        public void PushPlayer()
        {

        }

        public void PushLocalization()
        {
            for (int i=0; i< Enum.GetNames(typeof(StringValueLocalizable.Language)).Count(); i++)
            {
                MainWindow.EnsureDir(MainWindow.instance.LocalizationFile((StringValueLocalizable.Language)i));
                using (StreamWriter sw = new StreamWriter(MainWindow.instance.LocalizationFile((StringValueLocalizable.Language)i)))
                {
                    foreach (StringValueLocalizable loc in MainWindow.instance.mod.localization) sw.WriteLine(loc.TakeLocalizationString((StringValueLocalizable.Language)i));
                }
            }
        }

        public void Init()
        {
            Directory.CreateDirectory(window.modPath);
            buildIgnores.Add(@"*.csproj");
            buildIgnores.Add(@"*.user");
            buildIgnores.Add(@"obj\*");
            buildIgnores.Add(@"bin\*");
            buildIgnores.Add(@".vs\*");
            buildIgnores.Add(@"*.mtf");
            PushMod();
            PushBuild();
            PushDescription();
            PushScript();
            PushLocalization();
        }
    }
}
