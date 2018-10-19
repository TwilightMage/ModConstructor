using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values.SimpleValues
{
    public sealed class BooleanValue : SimpleValue<bool>
    {
        public BooleanValue() : base()
        {

        }

        public BooleanValue(bool value) : base(value)
        {

        }

        public override bool AsBoolean()
        {
            return value;
        }
    }
}
