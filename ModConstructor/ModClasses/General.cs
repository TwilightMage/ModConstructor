using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using ModConstructor.Controls;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using ModConstructor.ModClasses.Values;

namespace ModConstructor.ModClasses
{
    public class General : ValueObject
    {
        public class NullKeyException : Exception { }

        protected static string GenerateIndex()
        {
            long key = DateTime.UtcNow.Ticks;

            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(key.ToString());

            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }

        public ModInfo mod => MainWindow.instance.mod;

        public string key = GenerateIndex();

        public Property<StringValue>  className  { get; } = new Property<StringValue>( nameof(className),  typeof(General), () => "NewClass",         true, PropertyValidators.ClassName);
        public Property<GeneralValue> parent     { get; } = new Property<GeneralValue>(nameof(parent),     typeof(General), () => new GeneralValue(), true);
        public Property<BooleanValue> isAbstract { get; } = new Property<BooleanValue>(nameof(isAbstract), typeof(General), () => false,              true);
        public Property<BooleanValue> isRemoved  { get; } = new Property<BooleanValue>(nameof(isRemoved),  typeof(General), () => false,              true);

        public static General general;

        static General()
        {
            general = new General();
            general.className.value.value = general.key = "General";
        }

        public General()
        {
            parent.value.filters.Add(GeneralValue.Except(this));
        }

        public override string where => $"{base.where}.{className.value}";

        public bool IsChildOf(General parent)
        {
            return this.parent.value.item == parent ? true : this.parent.value.item != null ? this.parent.value.item.IsChildOf(parent) : false;
        }

        public static IEnumerable<General> userCreated => MainWindow.instance.mod.items;
        public static IEnumerable<General> all => (new List<General> { general, Item.item }).Union(userCreated);

        public override string ToString()
        {
            return className.value;
        }

        public virtual void Represent(UIElementCollection components)
        {
            components.Add(new GeneralComponent(this));
        }

        public override XElement PackElement(string name)
        {
            XElement result = base.PackElement(name);
            result.Add(new XAttribute("key", key));
            return result;
        }

        public override void Restore(XElement data)
        {
            key = data.Attribute("key")?.Value ?? data.Element("key")?.Value ?? GenerateIndex();
            base.Restore(data);
        }

        public void Remove()
        {
            foreach (var prop in GetType().GetProperties().Where(prop => prop.PropertyType.GetInterfaces().Any(face => face == typeof(IProperty))))
            {
                ((IProperty)prop.GetValue(this)).Remove();
            }
            OnRemove();
        }

        public virtual void OnRemove()
        {

        }

        public virtual string GetPath()
        {
            return (parent.value.item?.GetPath() ?? @"\") + (isAbstract.value ? $@"{(string)className.value}\" : @"\");
        }
    }
}
