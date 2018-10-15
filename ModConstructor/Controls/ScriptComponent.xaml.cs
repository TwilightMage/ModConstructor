using System.Windows.Controls;
using ModConstructor.ModClasses;
using System.Windows.Markup;
using System.Windows;

namespace ModConstructor.Controls
{
    /// <summary>
    /// Логика взаимодействия для ScriptComponent.xaml
    /// </summary>
    [ContentProperty("Content")]
    public partial class ScriptComponent : UserControl
    {
        public bool OpenState
        {
            get { return (bool)GetValue(OpenStateProperty); }
            set { SetValue(OpenStateProperty, value); }
        }
        public static readonly DependencyProperty OpenStateProperty = DependencyProperty.Register(nameof(OpenState), typeof(bool), typeof(ScriptComponent), new PropertyMetadata(true));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(ScriptComponent), new PropertyMetadata(""));

        private void ScriptComponentHeader_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool curstate = ((Controls.ScriptComponent)((FrameworkElement)sender).TemplatedParent).OpenState;
            ((Controls.ScriptComponent)((FrameworkElement)sender).TemplatedParent).OpenState = !curstate;
        }

        public ScriptComponent()
        {
            InitializeComponent();
        }
    }
}
