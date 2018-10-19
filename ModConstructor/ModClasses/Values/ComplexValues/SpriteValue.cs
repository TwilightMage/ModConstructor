using ModConstructor.Controls;
using ModConstructor.ModClasses.Values.SimpleValues;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values.ComplexValues
{
    public class SpriteValue : ComplexValue
    {
        public readonly SingleProperty<StringValue>  hash =       new SingleProperty<StringValue>( nameof(hash),       true, () => new StringValue(""));
        public readonly SingleProperty<NumberValue>  width =      new SingleProperty<NumberValue>( nameof(width),      true, () => new NumberValue(16));
        public readonly SingleProperty<NumberValue>  height =     new SingleProperty<NumberValue>( nameof(height),     true, () => new NumberValue(16));
        public readonly SingleProperty<BooleanValue> saveScaled = new SingleProperty<BooleanValue>(nameof(saveScaled), true, () => new BooleanValue(true));

        private Bitmap _texture;
        public Bitmap texture
        {
            get => _texture;
            set
            {
                _texture = value;
                PropertyChange("texture");
                source = Paint.BitmapToSource(_texture);
                hash.value.value = String.Join(" ", ToByteArray().Select(b => String.Format("{0:X}", b)));
            }
        }

        private BitmapSource _source;
        public BitmapSource source
        {
            get => _source;
            set
            {
                _source = value;
                PropertyChange("source");
            }
        }

        public SpriteValue()
        {
            hash.value.Changed += HashChanged;
        }

        private void HashChanged()
        {
            string[] bytes = hash.value.value.Split(' ');
            Bitmap result = new Bitmap(width.value.value, height.value.value);
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
            _texture = result;
            PropertyChange("texture");
            source = Paint.BitmapToSource(_texture);
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
            if (!saveScaled.value.value) return texture;

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

        public override void Remove()
        {
            base.Remove();
            source = null;
        }
    }
}
