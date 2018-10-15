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
using System.Windows.Shapes;

namespace ModConstructor
{
    public partial class Message : Window
    {
        public enum MessageResult
        {
            Ok,
            Yes,
            No,
            Cancel
        }

        public MessageResult result;

        public Message()
        {
            InitializeComponent();
        }

        public static bool Ask(Window sender, string title, string content)
        {
            Message mes = new Message();
            mes.Owner = sender;
            mes.title.Content = title;
            mes.content.Text = content;

            mes.choice.Visibility = Visibility.Visible;
            mes.ShowDialog();

            return mes.result == MessageResult.Yes;
        }

        public static void Inform(Window sender, string title, string content)
        {
            Message mes = new Message();
            mes.Owner = sender;
            mes.title.Content = title;
            mes.content.Text = content;

            mes.inform.Visibility = Visibility.Visible;
            mes.ShowDialog();
        }

        public static MessageResult Ensure(Window sender, string title, string content)
        {
            Message mes = new Message();
            mes.Owner = sender;
            mes.title.Content = title;
            mes.content.Text = content;

            mes.ensure.Visibility = Visibility.Visible;
            mes.ShowDialog();

            return mes.result;
        }

        private void Ok(object sender, RoutedEventArgs e)
        {
            result = MessageResult.Ok;
            Close();
        }

        private void Yes(object sender, RoutedEventArgs e)
        {
            result = MessageResult.Yes;
            Close();
        }

        private void No(object sender, RoutedEventArgs e)
        {
            result = MessageResult.No;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            result = MessageResult.Cancel;
            Close();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
