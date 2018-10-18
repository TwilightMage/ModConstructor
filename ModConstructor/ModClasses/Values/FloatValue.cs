using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public class FloatValue : SimpleValue<float>, IFloat
    {
        public FloatValue(float f) : base(f)
        {

        }

        public FloatValue() : base (0)
        {

        }

        public float AsFloat()
        {
            return value;
        }
    }
}
