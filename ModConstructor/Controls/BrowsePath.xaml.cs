using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UserControl = System.Windows.Controls.UserControl;

namespace ModConstructor.Controls
{
    /// <summary>
    /// Логика взаимодействия для BrowsePath.xaml
    /// </summary>
    public partial class BrowsePath : UserControl
    {
        public enum PathType
        {
            File,
            Dirrectory
        }

        public static readonly DependencyProperty _path = DependencyProperty.RegisterAttached(nameof(path), typeof(string), typeof(BrowsePath), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string path
        {
            get => (string)GetValue(_path);
            set => SetValue(_path, value);
        }

        public static readonly DependencyProperty _pathType = DependencyProperty.RegisterAttached(nameof(pathType), typeof(PathType), typeof(BrowsePath), new FrameworkPropertyMetadata(PathType.File, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public PathType pathType
        {
            get => (PathType)GetValue(_pathType);
            set => SetValue(_pathType, value);
        }

        public BrowsePath()
        {
            InitializeComponent();
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            if (pathType == PathType.File)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.FileName = path;
                if (dialog.ShowDialog() != DialogResult.OK) return;
                path = dialog.FileName;
            }
            else if (pathType == PathType.Dirrectory)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.SelectedPath = path;
                if (dialog.ShowDialog() != DialogResult.OK) return;
                path = dialog.SelectedPath;
            }
        }
    }
}
