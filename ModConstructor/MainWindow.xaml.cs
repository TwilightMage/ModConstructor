using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModConstructor.ModClasses;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows.Media;
using ModConstructor.Controls;
using System.Runtime.InteropServices;
using System.Windows.Shapes;
using System.Windows.Interop;

using Cursors = System.Windows.Input.Cursors;
using Path = System.IO.Path;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace ModConstructor
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static MainWindow instance;

        private HwndSource _hwndSource;

        private string _title = "Mod Creator";
        public string title
        {
            get => _title;
            set
            {
                _title = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("title"));
            }
        }

        private ModInfo _mod = null;
        public ModInfo mod
        {
            get => _mod;
            set
            {
                if (_mod == value) return;
                _mod = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("mod"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("displayInit"));
                title = (mod != null) ? $"Mod Creator • {mod.name}" : "Mod Creator";
                Errors.Clear();
                Dirty.Clear();
            }
        }

        public Visibility displayInit => mod == null ? Visibility.Visible : Visibility.Collapsed;

        #region init
        public ObservableCollection<string> mods { get; set; } = new ObservableCollection<string>();

        public string modloaderPath { get; set; } = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\My Games\Terraria\ModLoader";
        public string modSourcesPath => $@"{modloaderPath}\Mod Sources";
        public string modPath => $@"{modSourcesPath}\{mod.name}";
        public string modItems => $@"{modPath}\Items";
        public string modProjectiles => $@"{modPath}\Projectiles";

        public string programFile => $@"{modloaderPath}\constructor.xml";
        public string modFile => $@"{modPath}\{mod.name}.mtf";
        public string modScript => $@"{modPath}\{mod.name}.cs";
        public string modWorldScript => $@"{modPath}\{mod.name}_World.cs";
        public string modPlayerScript => $@"{modPath}\{mod.name}_Player.cs";
        public string modBuildFile => $@"{modPath}\build.txt";
        public string modDescriptionFile => $@"{modPath}\description.txt";

        public string ItemFile(string name, string subdir) => $@"{modItems}\{subdir}\{name}";
        public string ItemScript(string name, string subdir) => $@"{ItemFile(name, subdir)}.cs";
        public string ItemSprite(string name, string subdir) => $@"{ItemFile(name, subdir)}.png";

        private int _currentTab = 0;
        public int currentTab
        {
            get => _currentTab;
            set
            {
                _currentTab = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("currentTab"));
            }
        }

        private int _browseMod = -1;
        public int browseMod
        {
            get => _browseMod;
            set
            {
                _browseMod = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("browseMod"));

                canOpen = value >= 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("canOpen"));

                try
                {
                    showModInfo = canOpen ? Visibility.Visible : Visibility.Collapsed;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("showModInfo"));

                    browseModName = "";
                    browseModVersion = "";
                    browseModAuthor = "";
                    browseModHomepage = "";
                    browseModDescription = "";

                    if (value < 0) return;

                    using (StreamReader sr = new StreamReader($@"{modSourcesPath}\{mods[value]}\build.txt"))
                    {
                        string nameRegex = "^displayName = (?<value>.+)$";
                        string versionRegex = "^version = (?<value>.+)$";
                        string authorRegex = "^author = (?<value>.+)$";
                        string homepageRegex = "homepage = (?<value>.+)";

                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            Match match;

                            match = Regex.Match(line, nameRegex);
                            if (match.Success) browseModName = match.Groups["value"].Value;
                            else
                            {
                                match = Regex.Match(line, versionRegex);
                                if (match.Success) browseModVersion = match.Groups["value"].Value;
                                else
                                {
                                    match = Regex.Match(line, authorRegex);
                                    if (match.Success) browseModAuthor = match.Groups["value"].Value;
                                    else
                                    {
                                        match = Regex.Match(line, homepageRegex);
                                        if (match.Success) browseModHomepage = match.Groups["value"].Value;
                                    }
                                }
                            }
                        }
                    }

                    using (StreamReader sr = new StreamReader($@"{modSourcesPath}\{mods[value]}\description.txt"))
                    {
                        browseModDescription = sr.ReadToEnd();
                    }

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("browseModName"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("browseModVersion"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("browseModAuthor"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("browseModHomepage"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("browseModDescription"));
                }
                catch
                {
                    showModInfo = Visibility.Collapsed;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("showModInfo"));
                    Message.Inform(this, "Ошибка", "Не удалось прочитать информацию о моде");
                }
            }
        }

        public string browseModName { get; private set; }
        public string browseModVersion { get; private set; }
        public string browseModAuthor { get; private set; }
        public string browseModHomepage { get; private set; }
        public string browseModDescription { get; private set; }
        public bool canOpen { get; private set; } = false;
        public Visibility showModInfo { get; private set; } = Visibility.Collapsed;

        private void CreateMod_Click(object sender, RoutedEventArgs e)
        {
            CreateModInfo info = new CreateModInfo();
            bool repeat;
            do
            {
                repeat = false;
                if (CreateMod.Ask(this, info))
                {
                    if (Directory.Exists($@"{modSourcesPath}\{info.modName}"))
                    {
                        if (Message.Ask(this, "Внимание", "Мод с таким именем уже существует. Вы хотите удалить его?"))
                        {
                            DeleteMod(info.modName.value);
                        }
                        else
                        {
                            repeat = true;
                            continue;
                        }
                    }

                    mod = new ModInfo(info);
                    mod.Init();
                    mods.Add(info.modName.value);
                }
            }
            while (repeat);
        }

        private void OpenMod_Click(object sender, RoutedEventArgs e)
        {
            OpenMod(mods[browseMod]);
        }

        private void DeleteMod_Click(object sender, RoutedEventArgs e)
        {
            if (Message.Ask(this, "Внимание", $@"Вы действительно хотите удалить мод ""{mods[browseMod]}"""))
            {
                DeleteMod(mods[browseMod]);
            }
        }

        private void RefreshMods_Click(object sender, RoutedEventArgs e)
        {
            RefreshModlist();
        }

        private void modBrowser_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (browseMod >= 0) OpenMod(mods[browseMod]);
        }

        private void RefreshModlist()
        {
            mods.Clear();
            string[] dirs = Directory.GetDirectories(modSourcesPath);
            foreach (string dir in dirs)
            {
                string modname = Path.GetFileName(dir);
                if (File.Exists($@"{dir}\{modname}.mtf")) mods.Add(modname);
            }
        }

        private void Homepage_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(browseModHomepage);
        }
        #endregion

        public static ObservableCollection<IProperty> Errors { get; set; } = new ObservableCollection<IProperty>();
        public static ObservableCollection<IProperty> Dirty { get; set; } = new ObservableCollection<IProperty>();

        public static string SimplifyString(string input)
        {
            string result = "";
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (i > 0 && Regex.IsMatch(ch.ToString(), "[A-Z]")) result += $" {ch.ToString().ToLower()}";
                else result += ch;
            }
            return result;
        }

        public void OpenMod(string name)
        {
            string path = $@"{modSourcesPath}\{name}\{name}.mtf";
            using (StreamReader sr = new StreamReader(path))
            {
                mod = ModInfo.Parse(sr.ReadToEnd());
                GeneralValue.LaunchAssign();
                browseMod = -1;
                try
                {
                    
                }
                catch (Exception exc)
                {
                    mod = null;
                    Message.Inform(this, "Ошибка", $@"Не удалось открыть мод ""{name}""{"\n"}Ошибка: {exc.Message}");
                }
            }
        }

        private void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }

        public void DeleteMod(string name)
        {
            string path = $@"{modSourcesPath}\{name}";
            clearFolder(path);
            Directory.Delete(path);
            mods.Remove(name);
        }

        public MainWindow()
        {
            instance = this;

            if (!Directory.Exists(modloaderPath))
            {
                if (!Message.Ask(this, "Ошибка", "Папка модлоадера не найдена. Хотите выбрать ее вручную?"))
                {
                    App.Current.Shutdown();
                    return;
                }
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    App.Current.Shutdown();
                    return;
                }
                modloaderPath = dialog.SelectedPath;
            }

            if (!File.Exists(programFile))
            {
                using (StreamWriter sw = new StreamWriter(programFile))
                {
                    sw.Write(new XElement("configuration"));
                }
            }
            try
            {
                using (StreamReader sr = new StreamReader(programFile))
                {
                    XElement config = XElement.Parse(sr.ReadToEnd());
                    foreach (XElement elem in config.Elements())
                    {
                        switch (elem.Name.LocalName)
                        {
                            case "mainColor":
                                {
                                    // vs colors:
                                    // 028 151 234   1C97EA
                                    // 000 122 204   007ACC
                                    Color col = Color.FromRgb(byte.Parse(elem.Attribute("r").Value), byte.Parse(elem.Attribute("g").Value), byte.Parse(elem.Attribute("b").Value));
                                    App.Current.Resources["MainColor"] = col;
                                    App.Current.Resources["SecondaryColor"] = col * 0.75f;
                                }
                                break;
                            case "secondaryColor":
                                {
                                    Color col = Color.FromRgb(byte.Parse(elem.Attribute("r").Value), byte.Parse(elem.Attribute("g").Value), byte.Parse(elem.Attribute("b").Value));
                                    App.Current.Resources["SecondaryColor"] = col;
                                }
                                break;
                        }
                    }
                }
            }
            catch
            {
                using (StreamWriter sw = new StreamWriter(programFile))
                {
                    sw.Write(new XElement("configuration"));
                }
                Message.Inform(this, "Ошибка", "Не удалось корректно прочитать файл конфигурации. Он был перезаписан.");
            }

            InitializeComponent();

            Window_StateChanged(null, null);

            RefreshModlist();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        protected void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name)
            {
                case "top":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "bottom":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "left":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "right":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "topLeft":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "topRight":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "bottomLeft":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "bottomRight":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                default:
                    break;
            }
        }

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
            if (e.ClickCount==2)
            {
                if (WindowState == WindowState.Normal)
                {
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    WindowState = WindowState.Normal;
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    maximize.ToolTip = "Восстановить";
                    OuterGlow.Visibility = Visibility.Collapsed;
                    Resize.Visibility = Visibility.Collapsed;
                    OuterBorder.BorderThickness = new Thickness(0);

                    //Screen screen = Screen.AllScreens.FirstOrDefault(scr => scr.Bounds.Contains(new System.Drawing.Point((int)Left, (int)Top)));
                    MaxHeight = SystemParameters.WorkArea.Height + 7 * 2;
                    MaxWidth = SystemParameters.WorkArea.Width + 7 * 2;
                    break;
                case WindowState.Minimized:

                    break;
                case WindowState.Normal:
                    maximize.ToolTip = "Развернуть";
                    OuterGlow.Visibility = Visibility.Visible;
                    Resize.Visibility = Visibility.Visible;
                    OuterBorder.BorderThickness = new Thickness(1);

                    MaxHeight = int.MaxValue;
                    MaxWidth = int.MaxValue;
                    break;
            }
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Tabs.SelectedIndex)
            {
                case 0:
                   
                    break;
            }
        }

        private void MenuHome_Click(object sender, RoutedEventArgs e)
        {
            if (Dirty.Count > 0)
            {
                Message.MessageResult save = Message.Ensure(this, "Внимание", "Вы желаете сохранить изменения?");
                if (save == Message.MessageResult.Cancel) return;
                else if (save == Message.MessageResult.Yes) Save_Executed(null, null);
                mod = null;
            }
        }

        private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemProperties.Children.Clear();
            if (ItemList.SelectedItem != null) (ItemList.SelectedItem as Item).Represent(ItemProperties.Children);
        }

        private void MenuAddItem_Click(object sender, RoutedEventArgs e)
        {
            mod.items.Add();
        }

        public void EnsureDir(string path)
        {
            string[] pathParts = path.Split('\\');
            string part = pathParts[0];

            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                if (i > 0) part = $@"{part}\{pathParts[i]}";

                if (!Directory.Exists(part)) Directory.CreateDirectory(part);
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (mod == null) return;

            mod.PushBuild();
            mod.PushDescription();
            mod.PushMod();

            clearFolder(modItems);
            foreach (Item item in mod.items)
            {
                EnsureDir(ItemScript(item.className.value, item.GetPath())); File.WriteAllText(ItemScript(item.className.value, item.GetPath()), "");
                if (!item.isAbstract.value)
                {
                    EnsureDir(ItemSprite(item.className.value, item.GetPath()));
                    item.texture.value.GetScaled().Save(ItemSprite(item.className.value, item.GetPath()), System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.DataContext = this;
            settings.ShowDialog();
        }
    }
}
