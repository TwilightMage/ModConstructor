using ModConstructor.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public class SpriteValue : IValue
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ChangeEventHandler<Bitmap> Change;

        public IProperty property { get; set; }
        public string where => property.where;

        private Bitmap _texture = new Bitmap(16, 16);
        public Bitmap texture
        {
            get => _texture;
            set
            {
                Bitmap before = _texture;
                _texture = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("texture"));
                source = Paint.BitmapToSource(_texture);
                Change?.Invoke(before, value);
            }
        }

        private BitmapSource _source;
        public BitmapSource source
        {
            get => _source;
            set
            {
                _source = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("source"));
            }
        }

        private bool _scale = true;
        public bool scale
        {
            get => _scale;
            set
            {
                _scale = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("scale"));
            }
        }

        public byte[] ToByteArray()
        {
            byte[] result = new byte[texture.Width * texture.Height * 4];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    Color color = texture.GetPixel(x, y);
                    result[x * texture.Height * 4 + y * 4 + 0] = color.R;
                    result[x * texture.Height * 4 + y * 4 + 1] = color.G;
                    result[x * texture.Height * 4 + y * 4 + 2] = color.B;
                    result[x * texture.Height * 4 + y * 4 + 3] = color.A;
                }
            }
            return result;
        }

        public Bitmap GetScaled()
        {
            if (!scale) return texture;

            Bitmap result = new Bitmap(texture.Width * 2, texture.Height * 2);
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    Color col = texture.GetPixel(x, y);
                    result.SetPixel(x * 2, y * 2, col);
                    result.SetPixel(x * 2 + 1, y * 2, col);
                    result.SetPixel(x * 2, y * 2 + 1, col);
                    result.SetPixel(x * 2 + 1, y * 2 + 1, col);
                }
            }
            return result;
        }

        public XObject Pack(string name)
        {
            return PackElement(name);
        }

        public XElement PackElement(string name)
        {
            return new XElement(name,
                new XAttribute("width", texture.Width),
                new XAttribute("height", texture.Height),
                new XAttribute("scale", scale),
                new XAttribute("data", String.Join(" ", ToByteArray().Select(b => String.Format("{0:X}", b))))
                );
        }

        public void Restore(XAttribute data)
        {

        }

        public void Restore(XElement data)
        {
            int width = int.Parse(data.Attribute("width")?.Value ?? "16");
            int height = int.Parse(data.Attribute("height")?.Value ?? "16");
            string[] bytes = data.Attribute("data")?.Value.Split(' ') ?? new string[0];

            Bitmap result = new Bitmap(width, height);

            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    string r = bytes[x * result.Height * 4 + y * 4 + 0];
                    string g = bytes[x * result.Height * 4 + y * 4 + 1];
                    string b = bytes[x * result.Height * 4 + y * 4 + 2];
                    string a = bytes[x * result.Height * 4 + y * 4 + 3];
                    Color color = Color.FromArgb(Convert.ToByte(a, 16), Convert.ToByte(r, 16), Convert.ToByte(g, 16), Convert.ToByte(b, 16));
                    result.SetPixel(x, y, color);
                }
            }

            texture = result;

            scale = bool.Parse(data.Attribute("scale")?.Value ?? "true");
        }

        public void Remove()
        {
            source = null;
        }

        public void Initialize(IProperty property)
        {
            this.property = property;
        }
    }
}
