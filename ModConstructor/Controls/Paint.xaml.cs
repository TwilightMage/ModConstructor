using ModConstructor.ModClasses;
using ModConstructor.ModClasses.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
using Xceed.Wpf.Toolkit;

using Bitmap = System.Drawing.Bitmap;
using Color = System.Drawing.Color;
using Path = System.IO.Path;

namespace ModConstructor.Controls
{
    /// <summary>
    /// Логика взаимодействия для Paint.xaml
    /// </summary>
    public partial class Paint : UserControl, INotifyPropertyChanged
    {
        interface ITool
        {
            void LMB_Click();
            void LMB_Move(Vector delta);
            void RMB_Click();
            void RMB_Move(Vector delta);
            Bitmap cursor { get; }
            Paint owner { get; }
        }

        class ToolProperty<T> : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private T _value;
            public T value
            {
                get => _value;
                set
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("value"));
                }
            }

            public string name { get; set; }
            public int minimum { get; set; }
            public int maximum { get; set; }

            public static implicit operator T(ToolProperty<T> prop) => prop.value;

            public ToolProperty(string name, T value, int minimum = 0, int maximum = 0)
            {
                this.value = value;
                this.name = name;
                this.minimum = minimum;
                this.maximum = maximum;
            }
        }

        class Pencil : ITool
        {
            public Bitmap cursor => Properties.Resources.Pen;
            public Paint owner { get; }

            public void LMB_Click()
            {
                if (!owner.mouseOverImage) return;

                int x = (int)(owner.mouse.X / owner.scale);
                int y = (int)(owner.mouse.Y / owner.scale);
                owner.Draw(x, y, owner.main);
            }

            public void LMB_Move(Vector delta)
            {
                if (!owner.mouseOverImage) return;

                int x = (int)(owner.mouse.X / owner.scale);
                int y = (int)(owner.mouse.Y / owner.scale);
                owner.Draw(x, y, owner.main);
            }

            public void RMB_Click()
            {
                if (!owner.mouseOverImage) return;

                int x = (int)(owner.mouse.X / owner.scale);
                int y = (int)(owner.mouse.Y / owner.scale);
                owner.Draw(x, y, owner.secondary);
            }

            public void RMB_Move(Vector delta)
            {
                if (!owner.mouseOverImage) return;

                int x = (int)(owner.mouse.X / owner.scale);
                int y = (int)(owner.mouse.Y / owner.scale);
                owner.Draw(x, y, owner.secondary);
            }

            public Pencil(Paint owner)
            {
                this.owner = owner;
            }
        }

        class Fill : ITool
        {
            public Bitmap cursor => Properties.Resources.Fill;
            public Paint owner { get; }

            public ToolProperty<byte> tolerance { get; set; } = new ToolProperty<byte>("Допуск", 10, 0, 255);

            bool MatchesTolerance(Color A, Color B) => Math.Abs(A.R - B.R) <= tolerance && Math.Abs(A.G - B.G) <= tolerance && Math.Abs(A.B - B.B) <= tolerance && Math.Abs(A.A - B.A) <= tolerance;

            void Test(int x, int y, Color from, ref bool[,] matrix, ref bool[,] tested)
            {
                matrix[x, y] = tested[x, y] = true;
                if (x > 0 && !tested[x - 1, y])
                {
                    if (MatchesTolerance(owner.texture.GetPixel(x - 1, y), from)) Test(x - 1, y, from, ref matrix, ref tested);
                    else tested[x - 1, y] = true;
                }
                if (x < owner.width - 1 && !tested[x + 1, y])
                {
                    if (MatchesTolerance(owner.texture.GetPixel(x + 1, y), from)) Test(x + 1, y, from, ref matrix, ref tested);
                    else tested[x + 1, y] = true;
                }
                if (y > 0 && !tested[x, y - 1])
                {
                    if (MatchesTolerance(owner.texture.GetPixel(x, y - 1), from)) Test(x, y - 1, from, ref matrix, ref tested);
                    else tested[x, y - 1] = true;
                }
                if (y < owner.height - 1 && !tested[x, y + 1])
                {
                    if (MatchesTolerance(owner.texture.GetPixel(x, y + 1), from)) Test(x, y + 1, from, ref matrix, ref tested);
                    else tested[x, y + 1] = true;
                }
            }

            public void LMB_Click()
            {
                if (!owner.mouseOverImage) return;

                bool[,] matrix = new bool[owner.width, owner.height];
                bool[,] tested = new bool[owner.width, owner.height];
                int x = (int)(owner.mouse.X / owner.scale);
                int y = (int)(owner.mouse.Y / owner.scale);
                Color from = owner.texture.GetPixel(x, y);
                Test(x, y, from, ref matrix, ref tested);
                for (int i = 0; i < owner.width; i++)
                {
                    for (int j = 0; j < owner.height; j++)
                    {
                        if (matrix[i, j]) owner.Draw(i, j, owner.main);
                    }
                }
            }

            public void LMB_Move(Vector delta)
            {
                
            }

            public void RMB_Click()
            {
                if (!owner.mouseOverImage) return;

                bool[,] matrix = new bool[owner.width, owner.height];
                bool[,] tested = new bool[owner.width, owner.height];
                int x = (int)(owner.mouse.X / owner.scale);
                int y = (int)(owner.mouse.Y / owner.scale);
                Color from = owner.texture.GetPixel(x, y);
                Test(x, y, from, ref matrix, ref tested);
                for (int i = 0; i < owner.width; i++)
                {
                    for (int j = 0; j < owner.height; j++)
                    {
                        if (matrix[i, j]) owner.Draw(i, j, owner.secondary);
                    }
                }
            }

            public void RMB_Move(Vector delta)
            {
                
            }

            public Fill(Paint owner)
            {
                this.owner = owner;
            }
        }

        class ColorPicker : ITool
        {
            public Bitmap cursor => Properties.Resources.ColorPicker;
            public Paint owner { get; }

            public void LMB_Click()
            {
                if (!owner.mouseOverImage) return;

                int x = (int)(owner.mouse.X / owner.scale);
                int y = (int)(owner.mouse.Y / owner.scale);
                owner.main.Set(owner.texture.GetPixel(x, y));
            }

            public void LMB_Move(Vector delta)
            {
                if (!owner.mouseOverImage) return;

                int x = (int)(owner.mouse.X / owner.scale);
                int y = (int)(owner.mouse.Y / owner.scale);
                owner.main.Set(owner.texture.GetPixel(x, y));
            }

            public void RMB_Click()
            {
                if (!owner.mouseOverImage) return;

                int x = (int)(owner.mouse.X / owner.scale);
                int y = (int)(owner.mouse.Y / owner.scale);
                owner.secondary.Set(owner.texture.GetPixel(x, y));
            }

            public void RMB_Move(Vector delta)
            {
                if (!owner.mouseOverImage) return;

                int x = (int)(owner.mouse.X / owner.scale);
                int y = (int)(owner.mouse.Y / owner.scale);
                owner.secondary.Set(owner.texture.GetPixel(x, y));
            }

            public ColorPicker(Paint owner)
            {
                this.owner = owner;
            }
        }

        public class SelectRect : ITool
        {
            public Bitmap cursor => Properties.Resources.SelectRect;
            public Paint owner { get; }

            RectangleGeometry rect;

            public void LMB_Click()
            {
                owner.selection.Data = new RectangleGeometry();
                int x = owner.ClampW((int)(owner.mouse.X / owner.scale));
                int y = owner.ClampH((int)(owner.mouse.Y / owner.scale));
                owner.selectionFrom = new Dot(x, y);
                owner.selectionTo = new Dot(x, y);
            }

            public void LMB_Move(Vector delta)
            {
                int x = owner.ClampW((int)(owner.mouse.X / owner.scale));
                int y = owner.ClampH((int)(owner.mouse.Y / owner.scale));
                owner.selectionTo = new Dot(x, y);
            }

            public void RMB_Click()
            {
                
            }

            public void RMB_Move(Vector delta)
            {
                
            }

            public SelectRect(Paint owner)
            {
                this.owner = owner;
            }
        }

        private ITool _tool = null;
        private ITool tool
        {
            get => _tool;
            set
            {
                _tool = value;
                cursorSprite.Source = BitmapToSource(value.cursor);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("tool"));
                parameters.Children.Clear();
                List<PropertyInfo> props = new List<PropertyInfo>(value.GetType().GetProperties().Where(prop => prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(ToolProperty<>)));
                foreach (var property in props)
                {
                    object prop = property.GetValue(value);

                    Label label = new Label();
                    label.DataContext = prop;
                    label.ContentStringFormat = "{0}:";
                    label.SetBinding(Label.ContentProperty, "name");
                    label.VerticalAlignment = VerticalAlignment.Center;
                    parameters.Children.Add(label);

                    Type propt = property.PropertyType.GenericTypeArguments[0];
                    if (propt == typeof(int) || propt == typeof(float) || propt == typeof(byte) || propt == typeof(double))
                    {
                        DoubleUpDown dud = new DoubleUpDown();
                        dud.DataContext = prop;
                        dud.SetBinding(DoubleUpDown.MinimumProperty, "minimun");
                        dud.SetBinding(DoubleUpDown.MaximumProperty, "maximum");
                        dud.SetBinding(DoubleUpDown.ValueProperty, "value");
                        dud.Width = 60;
                        dud.Margin = new Thickness(0, 0, 0, 2);
                        dud.VerticalAlignment = VerticalAlignment.Center;
                        parameters.Children.Add(dud);

                        Slider slider = new Slider();
                        slider.DataContext = prop;
                        slider.SetBinding(Slider.MinimumProperty, "minimun");
                        slider.SetBinding(Slider.MaximumProperty, "maximum");
                        slider.SetBinding(Slider.ValueProperty, "value");
                        slider.VerticalAlignment = VerticalAlignment.Center;
                        slider.Width = 300;
                        parameters.Children.Add(slider);
                    }
                    else
                    {
                        Label error = new Label();
                        error.Content = "ERROR";
                        error.Foreground = new SolidColorBrush(Colors.Red);
                        parameters.Children.Add(error);
                    }
                }
                //parametersContainer.Visibility = props.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public class Coler : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public void HsvToRgb(int h, int S, int V, out byte r, out byte g, out byte b)
            {
                double inS = S / 100d;
                double inV = V / 100d;

                double H = h;
                while (H < 0) { H += 360; };
                while (H >= 360) { H -= 360; };
                double R, G, B;
                if (inV <= 0)
                { R = G = B = 0; }
                else if (inS <= 0)
                {
                    R = G = B = inV * 255;
                }
                else
                {
                    double hf = H / 60.0;
                    int i = (int)Math.Floor(hf);
                    double f = hf - i;
                    double pv = inV * (1 - inS);
                    double qv = inV * (1 - inS * f);
                    double tv = inV * (1 - inS * (1 - f));
                    switch (i)
                    {
                        // Red is the dominant color
                        case 0:
                            R = inV * 255;
                            G = tv * 255;
                            B = pv * 255;
                            break;

                        // Green is the dominant color
                        case 1:
                            R = qv * 255;
                            G = inV * 255;
                            B = pv * 255;
                            break;
                        case 2:
                            R = pv * 255;
                            G = inV * 255;
                            B = tv * 255;
                            break;

                        // Blue is the dominant color
                        case 3:
                            R = pv * 255;
                            G = qv * 255;
                            B = inV * 255;
                            break;
                        case 4:
                            R = tv * 255;
                            G = pv * 255;
                            B = inV * 255;
                            break;

                        // Red is the dominant color
                        case 5:
                            R = inV * 255;
                            G = pv * 255;
                            B = qv * 255;
                            break;

                        // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
                        case 6:
                            R = inV * 255;
                            G = tv * 255;
                            B = pv * 255;
                            break;
                        case -1:
                            R = inV * 255;
                            G = pv * 255;
                            B = qv * 255;
                            break;

                        // The color is not defined, we should throw an error.
                        default:
                            //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                            R = G = B = inV; // Just pretend its black/white
                            break;
                    }
                }
                r = (byte)R;
                g = (byte)G;
                b = (byte)B;
            }

            private byte _R;
            public byte R
            {
                get => _R;
                set
                {
                    _R = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("R"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("mediacolor"));

                    Color color = this;
                    _H = (int)Math.Round(color.GetHue());
                    _S = (int)Math.Round(color.GetSaturation() * 100);
                    _V = (int)Math.Round(color.GetBrightness() * 100);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("H"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("S"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("V"));
                }
            }

            private byte _G;
            public byte G
            {
                get => _G;
                set
                {
                    _G = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("G"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("mediacolor"));

                    Color color = this;
                    _H = (int)Math.Round(color.GetHue());
                    _S = (int)Math.Round(color.GetSaturation() * 100);
                    _V = (int)Math.Round(color.GetBrightness() * 100);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("H"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("S"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("V"));
                }
            }

            private byte _B;
            public byte B
            {
                get => _B;
                set
                {
                    _B = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("mediacolor"));

                    Color color = this;
                    _H = (int)Math.Round(color.GetHue());
                    _S = (int)Math.Round(color.GetSaturation() * 100);
                    _V = (int)Math.Round(color.GetBrightness() * 100);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("H"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("S"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("V"));
                }
            }

            private int _H;
            public int H
            {
                get => _H;
                set
                {
                    _H = value;
                    while (_H >= 360) _H -= 360;
                    while (_H < 0) _H += 360;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("H"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("mediacolor"));

                    HsvToRgb(H, S, V, out _R, out _G, out _B);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("R"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("G"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B"));
                }
            }

            private int _S;
            public int S
            {
                get => _S;
                set
                {
                    _S = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("S"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("mediacolor"));

                    HsvToRgb(H, S, V, out _R, out _G, out _B);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("R"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("G"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B"));
                }
            }

            private int _V;
            public int V
            {
                get => _V;
                set
                {
                    _V = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("V"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("mediacolor"));

                    HsvToRgb(H, S, V, out _R, out _G, out _B);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("R"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("G"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B"));
                }
            }

            private int _A;
            public int A
            {
                get => _A;
                set
                {
                    _A = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("A"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("mediacolor"));
                }
            }

            public Coler(byte R, byte G, byte B, int A)
            {
                this.R = R;
                this.G = G;
                this.B = B;
                this.A = A;
            }

            public void Set(Color col)
            {
                R = col.R;
                G = col.G;
                B = col.B;
                A = (int)Math.Round(col.A / 2.55);
            }

            public System.Windows.Media.Color mediacolor => this;

            /// <summary>
            /// Returns new Coler from presented HSVA
            /// </summary>
            /// <param name="H">Hue 0-360</param>
            /// <param name="S">Saturation 0-100</param>
            /// <param name="V">Value 0-100</param>
            /// <param name="A">Alpha 0-100</param>
            /// <returns></returns>
            public static Coler FromHSVA(int H, int S, int V, int A)
            {
                Coler coler = new Coler(0, 0, 0, A);
                coler.H = H;
                coler.S = S;
                coler.V = V;
                return coler;
            }

            //public System.Windows.Media.Color mediacolor => this;

            public static implicit operator Color(Coler col)
            {
                return Color.FromArgb((byte)Math.Round(col.A * 2.55), col.R, col.G, col.B);
            }

            public static implicit operator Coler(Color col)
            {
                return new Coler(col.R, col.G, col.B, (int)Math.Round(col.A / 2.55));
            }

            public static implicit operator System.Windows.Media.Color(Coler col)
            {
                return System.Windows.Media.Color.FromArgb((byte)Math.Round(col.A * 2.55), col.R, col.G, col.B);
            }

            public static implicit operator Coler(System.Windows.Media.Color col)
            {
                return new Coler(col.R, col.G, col.B, (int)Math.Round(col.A / 2.55));
            }
        }

        public struct Dot
        {
            public int X;
            public int Y;

            public Dot(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static bool operator ==(Dot a, Dot b)
            {
                return a.X == b.X && a.Y == b.Y;
            }

            public static bool operator !=(Dot a, Dot b)
            {
                return !(a == b);
            }

            public static implicit operator Point(Dot dot)
            {
                return new Point(dot.X, dot.Y);
            }

            public Dot Incremented()
            {
                return new Dot(X + 1, Y + 1);
            }
        }

        private Dot _selectionFrom = new Dot();
        private Dot selectionFrom
        {
            get => _selectionFrom;
            set
            {
                if (_selectionFrom == value) return;
                _selectionFrom = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("selectionFrom"));
                selectionFromReal = new Dot(Min(selectionFrom.X, selectionTo.X), Min(selectionFrom.Y, selectionTo.Y));
                selectionToReal = new Dot(Max(selectionFrom.X, selectionTo.X), Max(selectionFrom.Y, selectionTo.Y));
            }
        }

        private Dot _selectionTo = new Dot();
        private Dot selectionTo
        {
            get => _selectionTo;
            set
            {
                if (_selectionTo == value) return;
                _selectionTo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("selectionTo"));
                selectionFromReal = new Dot(Min(selectionFrom.X, selectionTo.X), Min(selectionFrom.Y, selectionTo.Y));
                selectionToReal = new Dot(Max(selectionFrom.X, selectionTo.X), Max(selectionFrom.Y, selectionTo.Y));
            }
        }

        private Dot _selectionFromReal = new Dot();
        public Dot selectionFromReal
        {
            get => _selectionFromReal;
            set
            {
                if (_selectionFromReal == value) return;
                _selectionFromReal = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("selectionFromReal"));
                selectedMatr = CalcSelectedMatr();
                UpdateSelection();
            }
        }

        private Dot _selectionToReal = new Dot();
        public Dot selectionToReal
        {
            get => _selectionToReal;
            set
            {
                if (_selectionToReal == value) return;
                _selectionToReal = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("selectionToReal"));
                selectedMatr = CalcSelectedMatr();
                UpdateSelection();
            }
        }

        private bool[,] _selectedMatr;
        public bool[,] selectedMatr
        {
            get => _selectedMatr;
            set
            {
                _selectedMatr = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("selectedMatr"));
            }
        }

        public bool hasSelection => selection.Data != null;

        void UpdateSelection()
        {
            selection.Data = new RectangleGeometry(new Rect(new Point(selectionFromReal.X * scale + 1, selectionFromReal.Y * scale + 1), new Point(selectionToReal.X * scale + scale - 1, selectionToReal.Y * scale + scale - 1)));
        }

        public int ClampW(int input) => input < 0 ? 0 : (input >= width ? width - 1 : input);
        public int ClampH(int input) => input < 0 ? 0 : (input >= height ? height - 1 : input);
        int Min(int a, int b) => a < b ? a : b;
        int Max(int a, int b) => a < b ? b : a;

        public bool[,] CalcSelectedMatr()
        {
            bool[,] result = new bool[width, height];
            Dot from = new Dot(Min(selectionFrom.X, selectionTo.X), Min(selectionFrom.Y, selectionTo.Y));
            Dot to = new Dot(Max(selectionFrom.X, selectionTo.X), Max(selectionFrom.Y, selectionTo.Y));
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result[x, y] = x >= from.X && x <= to.X && y >= from.Y && y <= to.Y;
                }
            }

            return result;
        }

        public void ClearSelection()
        {
            selectionFrom = selectionTo = new Dot(0, 0);
            selection.Data = null;
        }

        public Bitmap Crop(Rect rect)
        {
            Bitmap result = new Bitmap((int)rect.Width, (int)rect.Height);
            for (int x = 0; x < rect.Width; x++)
            {
                for (int y = 0; y < rect.Height; y++)
                {
                    result.SetPixel(x, y, texture.GetPixel(x + (int)rect.X, y + (int)rect.Y));
                }
            }
            return result;
        }

        Pencil pencil;
        Fill fill;
        ColorPicker colorPicker;
        SelectRect selectRect;
        Dictionary<Key, ITool> bindings;

        public int width => source?.texture.Width ?? 0;
        public int height => source?.texture.Height ?? 0;
        Bitmap texture => source?.texture;
        public BitmapSource IS => source?.source;
        
        public bool doubleScale
        {
            get => source?.scale ?? false;
            set
            {
                source.scale = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("doubleScale"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Coler main { get; } = Color.Black;
        public Coler secondary { get; } = Color.White;
        private Coler _curEditColer;
        public Coler curEditColer
        {
            get => _curEditColer;
            set
            {
                if (_curEditColer != null) _curEditColer.PropertyChanged -= ColerColorChanged;
                _curEditColer = value;
                if (_curEditColer != null) _curEditColer.PropertyChanged += ColerColorChanged;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("curEditColer"));
            }
        }
        private double paleteRadius;

        private void ColerColorChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "H":
                    ((GradientBrush)SaturationSlider.Background).GradientStops[1].Color = Coler.FromHSVA(curEditColer.H, 100, 100, 100);
                    ((GradientBrush)ValueSlider.Background).GradientStops[1].Color = Coler.FromHSVA(curEditColer.H, 100, 100, 100);
                    //Canvas.SetLeft(paleteThumb, paleteRadius + Math.Cos(curEditColer.H / 360f * Math.PI * 2) * (paleteRadius - 15));
                    //Canvas.SetTop(paleteThumb, paleteRadius + Math.Sin(curEditColer.H / 360f * Math.PI * 2) * (paleteRadius - 15));
                    break;
            }
        }

        private bool LMB = false;
        private bool RMB = false;
        private bool drag = false;
        private Point lastPos;
        private Vector delta;
        private Point mouseGlobal;
        private Point mouse;
        private bool mouseOverImage;
        public bool autoCenter = true;
        private SpriteValue _source;
        public SpriteValue source
        {
            get => _source;
            set
            {
                if (_source != null) _source.PropertyChanged -= SourceChanged;
                _source = value;
                if (_source != null) _source.PropertyChanged += SourceChanged;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("source"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("doubleScale"));
                Reload();
            }
        }

        private void SourceChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "source":
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IS"));
                    break;
                case "texture":
                    Reload();
                    break;
            }
        }

        private float _scale = maxScale;
        public float scale
        {
            get => _scale;
            set
            {
                _scale = value;
                if (_scale < 1) _scale = 1;
                else if (_scale > maxScale) _scale = maxScale;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("scale"));

                image.Width = scale * width;
                image.Height = scale * height;

                if (autoCenter) Center();
                if (hasSelection) UpdateSelection();
            }
        }

        public static float maxScale => 25;

        private bool _showBorder = true;
        public bool showBorder
        {
            get => _showBorder;
            set
            {
                _showBorder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("showBorder"));
            }
        }

        private bool _showGrid = true;
        public bool showGrid
        {
            get => _showGrid;
            set
            {
                _showGrid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("showGrid"));
            }
        }

        public static BitmapSource BitmapToSource(Bitmap bitmap) => System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        static Paint p;

        public void Reload()
        {
            if (source == null) return;

            grid.ColumnDefinitions.Clear();
            for (int i = 0; i < width; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                Border b = new Border();
                b.BorderThickness = new Thickness(1, 0, 1, 0);
                b.BorderBrush = (Brush)Resources["gridBrush"];
                grid.Children.Add(b);
                Grid.SetColumn(b, i);
                Grid.SetRowSpan(b, height);
            }

            grid.RowDefinitions.Clear();
            for (int i = 0; i < height; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());

                Border b = new Border();
                b.BorderThickness = new Thickness(0, 1, 0, 1);
                b.BorderBrush = (Brush)Resources["gridBrush"];
                grid.Children.Add(b);
                Grid.SetRow(b, i);
                Grid.SetColumnSpan(b, width);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IS"));
            Center();
        }

        public Paint()
        {
            curEditColer = main;
            pencil = new Pencil(this);
            fill = new Fill(this);
            colorPicker = new ColorPicker(this);
            selectRect = new SelectRect(this);
            bindings = new Dictionary<Key, ITool>
            {
                { Key.P, pencil },
                { Key.F, fill },
                { Key.K, colorPicker },
                { Key.S, selectRect }
            };

            InitializeComponent();

            //paleteRadius = paleteRing.ActualWidth / 2d;

            selectedMatr = CalcSelectedMatr();

            SetPencil(null, null);
        }

        public Paint(SpriteValue source) : this()
        {
            this.source = source;
        }

        public void Draw(int x, int y, Color color)
        {
            texture.SetPixel(x, y, Color.FromArgb(color.A, color.R, color.G, color.B));
            source.source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(texture.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            if (LMB || drag || RMB) return;
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    tool.LMB_Click();
                    LMB = true;
                    break;
                case MouseButton.Middle:
                    //drag = true;
                    break;
                case MouseButton.Right:
                    tool.RMB_Click();
                    RMB = true;
                    break;
            }
            e.Handled = true;
        }

        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            LMB = false;
            RMB = false;
            drag = false;
        }

        private void CanvasMouseEnter(object sender, MouseEventArgs e)
        {
            cursor.Visibility = Visibility.Visible;
        }

        private void CanvasMouseLeave(object sender, MouseEventArgs e)
        {
            LMB = false;
            RMB = false;
            drag = false;
            cursor.Visibility = Visibility.Collapsed;
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            mouse = Mouse.GetPosition(image);
            mouseOverImage = mouse.X >= 0 && mouse.X < width * scale && mouse.Y >= 0 && mouse.Y < height * scale;

            mouseGlobal = Mouse.GetPosition(canvas);
            delta = mouseGlobal - lastPos;
            lastPos = mouseGlobal;

            Canvas.SetLeft(cursor, mouseGlobal.X);
            Canvas.SetTop(cursor, mouseGlobal.Y);

            if (LMB)
            {
                tool.LMB_Move(delta);
            }
            else if (drag)
            {
                double left = Canvas.GetLeft(image);
                double top = Canvas.GetTop(image);
                Canvas.SetLeft(image, left + delta.X);
                Canvas.SetTop(image, top + delta.Y);
                autoCenter = false;
            } else if (RMB)
            {
                tool.RMB_Move(delta);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Center();
        }

        public void Center()
        {
            float w = scale * width;
            float h = scale * height;
            image.Width = w;
            image.Height = h;
            double left = (space.ActualWidth - w) / 2d;
            double top = (space.ActualHeight - h) / 2d + toolbar.ActualHeight;
            Canvas.SetLeft(image, left);
            Canvas.SetTop(image, top);
        }

        private void ChoseMain(object sender, MouseButtonEventArgs e)
        {
            curEditColer = main;
        }

        private void ChoseSecondary(object sender, MouseButtonEventArgs e)
        {
            curEditColer = secondary;
        }

        private void SetPencil(object sender, RoutedEventArgs e)
        {
            tool = pencil;
        }

        private void SetFill(object sender, RoutedEventArgs e)
        {
            tool = fill;
        }

        private void SetColorPicker(object sender, RoutedEventArgs e)
        {
            tool = colorPicker;
        }

        private void SetSelectRect(object sender, RoutedEventArgs e)
        {
            tool = selectRect;
        }

        private void SetMagicWand(object sender, RoutedEventArgs e)
        {

        }

        private void ClickCenter(object sender, RoutedEventArgs e)
        {
            Center();
            autoCenter = true;
        }

        public void Copy()
        {
            Bitmap map = hasSelection ? Crop(new Rect(selectionFromReal, selectionToReal.Incremented())) : texture;
            Bitmap mapNoTr = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixel = texture.GetPixel(x, y);
                    mapNoTr.SetPixel(x, y, Color.FromArgb(255, pixel.R, pixel.G, pixel.B));
                }
            }
            ClipboardHelper.SetClipboardImage(map, mapNoTr, null);
        }

        private void OnCopy(object sender, RoutedEventArgs e)
        {
            Copy();
        }

        public void Paste()
        {
            if (Clipboard.ContainsImage())
            {
                
            }
        }

        private void OnPaste(object sender, RoutedEventArgs e)
        {
            Paste();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            foreach (var binding in bindings)
            {
                if (binding.Key == e.Key)
                {
                    tool = binding.Value;
                    e.Handled = true;
                    break;
                }
            }
        }

        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            switch (e.Key)
            {
                case Key.Escape:
                    ClearSelection();
                    break;
                case Key.A:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
                    {
                        selectionFrom = new Dot(0, 0);
                        selectionTo = new Dot(texture.Width, texture.Height);
                    }
                    break;
                case Key.C:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
                    {
                        Copy();
                    }
                    break;
                case Key.V:
                    Paste();
                    break;
                case Key.Delete:
                    if (hasSelection)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            for (int y = 0; y < height; y++)
                            {
                                if (selectedMatr[x, y]) texture.SetPixel(x, y, Color.Transparent);
                            }
                        }
                        source.source = BitmapToSource(texture);
                        ClearSelection();
                    }
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            e.Handled = true;
        }

        private void CanvasMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scale += e.Delta / 100f;
            e.Handled = true;
        }

        private void ImportClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Файлы изображений (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Bitmap result = new Bitmap(dialog.FileName);
                if (Message.Ask(MainWindow.instance, "Внимание", "Сжать изображение?"))
                {
                    result = RescaleImage(result, new System.Drawing.Size(result.Width / 2, result.Height / 2));
                }
                source.texture = result;
            }
        }

        public static Bitmap RescaleImage(System.Drawing.Image source, System.Drawing.Size size)
        {
            // 1st bullet, pixel format
            var bmp = new Bitmap(size.Width, size.Height, source.PixelFormat);
            // 2nd bullet, resolution
            bmp.SetResolution(source.HorizontalResolution, source.VerticalResolution);
            using (var gr = System.Drawing.Graphics.FromImage(bmp))
            {
                // 3rd bullet, background
                gr.Clear(Color.Transparent);
                // 4th bullet, interpolation
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gr.DrawImage(source, new System.Drawing.Rectangle(0, 0, size.Width, size.Height));
            }
            return bmp;
        }

        private void ExportClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "PNG (*.png)|*.png|JPG (*.jpg)|*.jpg|BMP (*.bmp)|*.bmp";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                source.texture.Save(dialog.FileName);
            }
        }

        /*bool paleteDrag = false;

        private void PaleteMouseDown(object sender, MouseButtonEventArgs e)
        {
            paleteDrag = true;
        }

        private void PaleteMouseMove(object sender, MouseEventArgs e)
        {
            if (paleteDrag)
            {
                Point p = Mouse.GetPosition(palete);
                p.X -= paleteRadius;
                p.Y -= paleteRadius;
                curEditColer.H = (int)(Math.Atan2(p.Y, p.X) / (Math.PI * 2) * 360);
            }
        }

        private void PaleteMouseUp(object sender, MouseButtonEventArgs e)
        {
            paleteDrag = false;
        }

        private void PaleteMouseLeave(object sender, MouseEventArgs e)
        {
            paleteDrag = false;
        }*/
    }
}
