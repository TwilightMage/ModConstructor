using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values.SimpleValues
{
    public sealed class StringValue : SimpleValue<string>
    {
        public StringValue() : base()
        {

        }

        public StringValue(string value) : base(value)
        {

        }

        public override string AsString()
        {
            return value;
        }
    }
}
