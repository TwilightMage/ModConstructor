using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public class NumberValue : SimpleValue<int>, INumber
    {
        public NumberValue(int f) : base(f)
        {

        }

        public NumberValue() : base (0)
        {

        }

        public int AsNumber()
        {
            return value;
        }
    }
}
