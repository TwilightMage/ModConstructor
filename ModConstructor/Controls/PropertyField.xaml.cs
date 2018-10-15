using ModConstructor.ModClasses;
using System;
using System.Collections.Generic;
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
    public class PropertySplitterBehaviour : Behavior<Grid>
    {
        public Grid ParentGrid { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            ParentGrid = this.AssociatedObject as Grid;
            ParentGrid.SizeChanged += parent_SizeChanged;
        }

        void parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ParentGrid.ColumnDefinitions.Count == 4)
            {
                Double maxW = e.NewSize.Width - ParentGrid.ColumnDefinitions[3].MinWidth - ParentGrid.ColumnDefinitions[2].ActualWidth - ParentGrid.ColumnDefinitions[0].ActualWidth;
                ParentGrid.ColumnDefinitions[1].MaxWidth = maxW;
            }
            else throw new Exception("Invalid column count");
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (ParentGrid != null)
            {
                ParentGrid.SizeChanged -= parent_SizeChanged;
            }
        }
    }

    /// <summary>
    /// Логика взаимодействия для PropertyField.xaml
    /// </summary>
    public partial class PropertyField : UserControl
    {
        public enum DataType
        {
            String,
            StringLocalizable,
            Integer,
            Real,
            Boolean,
            Enumerable,
            Code,
            Collor,
            Money
        }

        public enum DataMap
        {
            Single,
            List,
            Dictionary,
            Matrix
        }

        public string Error
        {
            get => (String)GetValue(ErrorProperty);
            set => SetValue(ErrorProperty, value);
        }
        public static readonly DependencyProperty ErrorProperty = DependencyProperty.Register(nameof(Error), typeof(string), typeof(PropertyField), new PropertyMetadata("", ErrorChanged));

        public static void ErrorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PropertyField pf = sender as PropertyField;
            string value = e.NewValue as string;
            pf.ErrorIndicator.Visibility = string.IsNullOrWhiteSpace(value) ? Visibility.Collapsed : Visibility.Visible;
        }


        public string Header
        {
            get => (String)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(PropertyField), new PropertyMetadata(""));


        public IProperty Target
        {
            get => (IProperty)GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(nameof(Target), typeof(IProperty), typeof(PropertyField), new PropertyMetadata(null, TargetChanged));

        public static void TargetChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PropertyField s = sender as PropertyField;
            if (e.OldValue != null)
            {
                IProperty prop = e.OldValue as IProperty;
                BindingOperations.ClearBinding(s, ErrorProperty);
            }
            if (e.NewValue != null)
            {
                IProperty prop = e.NewValue as IProperty;
                s.SetBinding(ErrorProperty, "Target.error");
            }
        }

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(PropertyField), new FrameworkPropertyMetadata(null));


        public DataType Datatype
        {
            get => (DataType)GetValue(DatatypeProperty);
            set => SetValue(DatatypeProperty, value);
        }
        public static readonly DependencyProperty DatatypeProperty = DependencyProperty.Register(nameof(Datatype), typeof(DataType), typeof(PropertyField), new PropertyMetadata(DataType.String, DatatypeChanged));

        public static void DatatypeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PropertyField pf = sender as PropertyField;
            pf.Input.InputTemplate = pf.Resources[$"PropertyInput_{e.NewValue.ToString()}"] as ControlTemplate;
        }


        public DataMap Datamap
        {
            get => (DataMap)GetValue(DatamapProperty);
            set => SetValue(DatamapProperty, value);
        }
        public static readonly DependencyProperty DatamapProperty = DependencyProperty.Register(nameof(Datamap), typeof(DataMap), typeof(PropertyField), new PropertyMetadata(DataMap.Single, DatamapChanged));

        public static void DatamapChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PropertyField pf = sender as PropertyField;
            pf.Input.Template = pf.Resources[$"PropertyMap_{e.NewValue.ToString()}"] as ControlTemplate;
            pf.Input.ApplyTemplate();
        }


        public bool NoHeader
        {
            get => (bool)GetValue(NoHeaderProperty);
            set => SetValue(NoHeaderProperty, value);
        }
        public static readonly DependencyProperty NoHeaderProperty = DependencyProperty.Register(nameof(NoHeader), typeof(bool), typeof(PropertyField), new PropertyMetadata(false, NoHeaderChanged));

        public static void NoHeaderChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PropertyField pf = sender as PropertyField;
            bool value = (bool)e.NewValue;
            Visibility vis = value ? Visibility.Collapsed : Visibility.Visible;
            pf.Name.Visibility = pf.Splitter.Visibility = vis;
        }


        public void AddClick(object sender, RoutedEventArgs e)
        {
            IPropertyList list = (IPropertyList)Target;
            list.Add();
        }

        public void ClearClick(object sender, RoutedEventArgs e)
        {
            IPropertyList list = (IPropertyList)Target;
            list.Clear();
        }

        public void RemoveClick(object sender, RoutedEventArgs e)
        {
            IPropertyList list = (IPropertyList)Target;
            list.GetType().GetMethod("Delete").Invoke(list, new object[] { ((ContentPresenter)((FrameworkElement)sender).TemplatedParent).Content });
        }

        public void OpenLocalizator(object sender, RoutedEventArgs e)
        {
            MainWindow.instance.localizator.SelectedItem = null;
            foreach (StringValueLocalizable.Presenter pres in MainWindow.instance.localizator.Items)
            {
                if (pres.owner == (sender as FrameworkElement).DataContext)
                {
                    MainWindow.instance.localizator.SelectedItem = pres;
                    break;
                }
            }
            MainWindow.instance.Tabs.SelectedItem = MainWindow.instance.LocalizationTab;

        }

        /*MainWindow mw => ModConstructor.MainWindow.instance;

        private void PropertyFieldListAdd_Click(object sender, RoutedEventArgs e)
        {
            IPropertyList propertyList = (IPropertyList)((Button)sender).DataContext;
            propertyList.Add();
        }

        private void PropertyFieldListClear_Click(object sender, RoutedEventArgs e)
        {
            IPropertyList propertyList = (IPropertyList)((Button)sender).DataContext;
            propertyList.Clear();
        }

        private void PropertyFieldListRemove_Click(object sender, RoutedEventArgs e)
        {
            IPropertyList list = (IPropertyList)((FrameworkElement)((FrameworkElement)sender).DataContext).DataContext;
            list.GetType().GetMethod("Delete").Invoke(list, new object[] { ((FrameworkElement)((FrameworkElement)sender).TemplatedParent).DataContext });
            //((IPropertyList)((FrameworkElement)((Visual)((FrameworkElement)((FrameworkElement)sender).TemplatedParent).TemplatedParent).VisualParent).DataContext).list
        }

        private void OpenLocalization(object sender, RoutedEventArgs e)
        {
            mw.localizator.SelectedItem = null;
            foreach (StringValueLocalizable.Presenter pres in mw.localizator.Items)
            {
                if (pres.owner == (sender as FrameworkElement).DataContext)
                {
                    mw.localizator.SelectedItem = pres;
                    break;
                }
            }
            // mw.localizator.SelectedItem = mw.localizator.Items.FirstOrDefault(pres => pres.owner == (sender as FrameworkElement).DataContext);
            mw.Tabs.SelectedIndex = 3;
        }*/

        public PropertyField()
        {
            InitializeComponent();
        }
    }
}
