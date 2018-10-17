using ModConstructor.Controls;
using ModConstructor.ModClasses.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace ModConstructor.ModClasses
{
    public delegate void ChangeEventHandler<T>(T before, T now);

    public interface IValue : INotifyPropertyChanged
    {
        IProperty property { get; set; }
        string where { get; }

        XObject Pack(string name);
        XElement PackElement(string name);
        void Restore(XAttribute data);
        void Restore(XElement data);
        void Remove();
        void Initialize(IProperty property);
    }

    public interface IValueString : IValue
    {
        string AsString();
    }

    public interface IValueNumber : IValue
    {
        int AsNumber();
    }

    public interface IValueFloat : IValue
    {
        float AsFloat();
    }

    public interface IValueBoolean : IValue
    {
        bool AsBoolean();
    }
}
