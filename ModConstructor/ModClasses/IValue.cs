using ModConstructor.Controls;
using ModConstructor.ModClasses.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
namespace ModConstructor.ModClasses
{
    public delegate void ChangedEventHandler<T>(T before, T now);
    public delegate void ChangedEventHandler();

    public interface IString
    {
        string AsString();
    }

    public interface INumber
    {
        int AsNumber();
    }

    public interface IFloat
    {
        float AsFloat();
    }

    public interface IBoolean
    {
        bool AsBoolean();
    }
}
