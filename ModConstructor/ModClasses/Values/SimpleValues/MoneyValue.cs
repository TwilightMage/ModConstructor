using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values.SimpleValues
{
    public sealed class MoneyValue : SimpleValue<int>
    {
        private int Clamp(int min, int max, int value)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public const int maxC = 100;
        public const int maxS = 100 * 100;
        public const int maxG = 100 * 100 * 100;
        public const int maxP = 100 * 100 * 100 * 100;
        public const int max = maxC + maxS + maxG + maxP;

        public int copper
        {
            get => value % 100 % 100 % 100 % 100;
            set
            {
                this.value = Clamp(0, 100, value) + silver * 100 + golden * 100 * 100 + platinum * 100 * 100 * 100;
                PropertyChange("copper");
                PropertyChange("silver");
                PropertyChange("golden");
                PropertyChange("platinum");
                PropertyChange("value");
            }
        }

        public int silver
        {
            get => value % 100 % 100 % 100;
            set
            {
                this.value = copper + Clamp(0, 100, value) * 100 + golden * 100 * 100 + platinum * 100 * 100 * 100;
                PropertyChange("silver");
                PropertyChange("golden");
                PropertyChange("platinum");
                PropertyChange("value");
            }
        }

        public int golden
        {
            get => value % 100 % 100;
            set
            {
                this.value = copper + silver * 100 + Clamp(0, 100, value) * 100 * 100 + platinum * 100 * 100 * 100;
                PropertyChange("golden");
                PropertyChange("platinum");
                PropertyChange("value");
            }
        }

        public int platinum
        {
            get => value % 100;
            set
            {
                this.value = copper + silver * 100 + golden * 100 * 100 + Clamp(0, 100, value) * 100 * 100 * 100;
                PropertyChange("platinum");
                PropertyChange("value");
            }
        }

        public override int value
        {
            get => _value;
            set
            {
                base.value = Clamp(0, max, value);

                PropertyChange("copper");
                PropertyChange("silver");
                PropertyChange("golden");
                PropertyChange("platinum");
                PropertyChange("value");
            }
        }

        public MoneyValue() : base()
        {

        }

        public MoneyValue(int value) : base(value)
        {

        }

        public MoneyValue(int copper, int silver, int golden, int platinum)
        {
            this.copper = copper;
            this.silver = silver;
            this.golden = golden;
            this.platinum = platinum;
        }

        public override int AsNumber()
        {
            return value;
        }
    }
}
