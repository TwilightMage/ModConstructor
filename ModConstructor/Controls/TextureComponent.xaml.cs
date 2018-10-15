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
    /// Логика взаимодействия для TextureComponent.xaml
    /// </summary>
    public partial class TextureComponent : UserControl
    {
        public TextureComponent(General target, SpriteValue sprite)
        {
            DataContext = target;

            InitializeComponent();

            paint.source = sprite;
        }

        public TextureComponent()
        {
            InitializeComponent();
        }
    }
}
