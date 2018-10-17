using ModConstructor.Controls;
using ModConstructor.ModClasses;
using ModConstructor.ModClasses.Values;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ModConstructor
{
    /// <summary>
    /// Логика взаимодействия для CreateMod.xaml
    /// </summary>
    public partial class CreateMod : Window
    {
        public CreateModInfo info { get; set; } = new CreateModInfo();

        public CreateMod(CreateModInfo info)
        {
            this.info = info;
            InitializeComponent();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Submit(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            DragMove();
        }

        public static bool Ask(Window sender, CreateModInfo info)
        {
            CreateMod window = new CreateMod(info);
            window.Owner = sender;
            if (window.ShowDialog() == true)
            {
                info = window.info;
                return true;
            }
            else
            {
                info = null;
                return false;
            }
        }
    }

    public class CreateModInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Property<StringValue>     modName     { get; } = new Property<StringValue>(     nameof(modName),     typeof(CreateModInfo), () => "NewMod",                validator: PropertyValidators.ClassName);
        public Property<StringValue>     displayName { get; } = new Property<StringValue>(     nameof(displayName), typeof(CreateModInfo), () => "Новый мод"              );
        public Property<StringValue>     homePage    { get; } = new Property<StringValue>(     nameof(homePage),    typeof(CreateModInfo), () => ""                       );
        public Property<StringValue>     description { get; } = new Property<StringValue>(     nameof(description), typeof(CreateModInfo), () => new StringValue("", true));
        public PropertyList<StringValue> authors     { get; } = new PropertyList<StringValue>( nameof(displayName), typeof(CreateModInfo), () => ""                       );

        public bool acceptable => !modName.hasError;

        public CreateModInfo()
        {
            authors.Add(Environment.UserName);

            modName.value.Change += propChange;
        }

        private void propChange(string before, string after)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("acceptable"));
        }
    }
}
