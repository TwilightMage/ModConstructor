using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public class BooleanValue : SimpleValue<bool>, IBoolean
    {
        public BooleanValue(bool b) : base(b)
        {

        }

        public BooleanValue() : base (false)
        {

        }

        public bool AsBoolean()
        {
            return value;
        }
    }
}
