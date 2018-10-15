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
    public partial class CodeInput : UserControl
    {
        public string lineNumbers
        {
            get { return (string)GetValue(lineNumbersProperty); }
            set { SetValue(lineNumbersProperty, value); }
        }
        public static readonly DependencyProperty lineNumbersProperty = DependencyProperty.RegisterAttached(nameof(lineNumbers), typeof(string), typeof(CodeInput), new PropertyMetadata("1"));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(nameof(Text), typeof(string), typeof(CodeInput), new PropertyMetadata(""));

        public CodeInput()
        {
            InitializeComponent();
        }

        private void Content_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string x = string.Empty;
            for (int i = 0; i < textBox.LineCount; i++)
            {
                x += i + 1 + "\n";
            }
            SetValue(lineNumbersProperty, x);
        }
    }
}
