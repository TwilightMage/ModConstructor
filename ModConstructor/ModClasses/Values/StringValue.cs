using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModConstructor.ModClasses.Values
{
    public class StringValue : SimpleValue<string>, IString
    {
        public StringValue(string s) : base(s)
        {

        }

        public StringValue() : base ("")
        {

        }

        public string AsString()
        {
            return value;
        }
    }
}
