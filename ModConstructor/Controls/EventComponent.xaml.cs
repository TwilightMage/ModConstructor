﻿using ModConstructor.ModClasses;
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
    /// Логика взаимодействия для EventComponent.xaml
    /// </summary>
    public partial class EventComponent : UserControl
    {
        public EventComponent(General target, string header)
        {
            DataContext = target;

            InitializeComponent();
            expander.Header = header;
        }

        public EventComponent()
        {
            InitializeComponent();
        }
    }
}
