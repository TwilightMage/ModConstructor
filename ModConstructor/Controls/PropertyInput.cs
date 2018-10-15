using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModConstructor.Controls
{
    public class PropertyInput : Control
    {
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(PropertyInput), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        
        static PropertyInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyInput), new FrameworkPropertyMetadata(typeof(PropertyInput)));
        }
    }
}
