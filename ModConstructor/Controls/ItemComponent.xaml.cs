using ModConstructor.ModClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModConstructor.Controls
{
    /// <summary>
    /// Логика взаимодействия для ItemComponent.xaml
    /// </summary>
    public partial class ItemComponent : UserControl
    {
        public ItemComponent(Item target)
        {
            DataContext = target;

            InitializeComponent();

            target.preset.value.Change += delegate { Revis(target); };
            Revis(target);
        }

        void Revis(Item target)
        {
            for (int i = 0; i < presets.Children.Count; i++)
            {
                presets.Children[i].Visibility = (((string)((FrameworkElement)presets.Children[i]).Tag)[target.preset.value.value] == '1') ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public ItemComponent()
        {
            InitializeComponent();
        }
    }
}
