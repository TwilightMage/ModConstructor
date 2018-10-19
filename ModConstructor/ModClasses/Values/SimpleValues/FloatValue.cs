using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values.SimpleValues
{
    public sealed class FloatValue : SimpleValue<float>
    {
        public FloatValue() : base()
        {

        }

        public FloatValue (float value) : base(value)
        {

        }

        public override float AsFloat()
        {
            return value;
        }
    }
}
