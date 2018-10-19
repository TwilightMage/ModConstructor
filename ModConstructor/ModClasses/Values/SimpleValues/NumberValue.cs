using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values.SimpleValues
{
    public sealed class NumberValue : SimpleValue<int>
    {
        public NumberValue() : base()
        {

        }

        public NumberValue(int value) : base(value)
        {

        }

        public override int AsNumber()
        {
            return value;
        }
    }
}
