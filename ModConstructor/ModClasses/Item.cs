using ModConstructor.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace ModConstructor.ModClasses
{
    public class Item : General
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static string[] presets = new string[]
        {
            "Общий",                    // 0
            "Оружие ближнего боя",      // 1
            "Магическое оружие",        // 2
            "Стрелковое оружие",        // 3
            "Боеприпас",                // 4
            "Метательное оружие",       // 5
            "Оружие призыва",           // 6
            "Установка турели",         // 7
            "Броня",                    // 8
            "Аксессуар",                // 9
            "Инструмент",               // 10
            "Потребляемый",             // 11
            "Блок"                      // 12
        };

        public static string[] rarity = new string[]
        {
            "Серый (-1)",
            "Белый (0)",
            "Синий (1)",
            "Зеленый (2)",
            "Оранжевый (3)",
            "Светло-красный (4)",
            "Розовый (5)",
            "Светло-фиолетовый (6)",
            "Лаймовый (7)",
            "Желтый (8)",
            "Бирюзовый (9)",
            "Красный (10)",
            "Фиолетовый (11)",
            "Радужный (эксперт)",
            "Янтарный (квестовый)"
        };

        public Property<EnumerableValue>        preset      { get; } = new Property<EnumerableValue>(       nameof(preset),      typeof(Item), () => new EnumerableValue(nameof(preset)),                (prop) => new string[] { }, true);
        public Property<GeneralValue>           ammo        { get; } = new Property<GeneralValue>(          nameof(ammo),        typeof(Item), () => new GeneralValue(GeneralValue.ChildOf(item, true)), (prop) => new string[] { }, true);
        public Property<GeneralValue>           block       { get; } = new Property<GeneralValue>(          nameof(block),       typeof(Item), () => new GeneralValue(GeneralValue.ChildOf(item, true)), (prop) => new string[] { }, true);

        public Property<SpriteValue>            texture     { get; } = new Property<SpriteValue>(           nameof(texture),     typeof(Item), () => new SpriteValue(),                                  (prop) => new string[] { }, true);
        public Property<StringValueLocalizable> displayName { get; } = new Property<StringValueLocalizable>(nameof(displayName), typeof(Item), () => new StringValueLocalizable(),                       (prop) => new string[] { }, true);
        public Property<StringValueLocalizable> description { get; } = new Property<StringValueLocalizable>(nameof(description), typeof(Item), () => new StringValueLocalizable(),                       (prop) => new string[] { }, true);
        public Property<EnumerableValue>        rare        { get; } = new Property<EnumerableValue>(       nameof(rare),        typeof(Item), () => new EnumerableValue(nameof(rare), 1),               (prop) => new string[] { }, true);
        public Property<MoneyValue>             value       { get; } = new Property<MoneyValue>(            nameof(value),       typeof(Item), () => 1000,                                               (prop) => new string[] { }, true);
        public Property<NumberValue>            maxStack    { get; } = new Property<NumberValue>(           nameof(maxStack),    typeof(Item), () => 99,                                                 (prop) => new string[] { }, true);

        public Property<NumberValue>            pick        { get; } = new Property<NumberValue>(           nameof(pick),        typeof(Item), () => 0,                                                  (prop) => new string[] { }, true);
        public Property<NumberValue>            axe         { get; } = new Property<NumberValue>(           nameof(axe),         typeof(Item), () => 0,                                                  (prop) => new string[] { }, true);
        public Property<NumberValue>            hammer      { get; } = new Property<NumberValue>(           nameof(hammer),      typeof(Item), () => 0,                                                  (prop) => new string[] { }, true);
        public Property<NumberValue>            damage      { get; } = new Property<NumberValue>(           nameof(damage),      typeof(Item), () => 0,                                                  (prop) => new string[] { }, true);
        public Property<FloatValue>             knockback   { get; } = new Property<FloatValue>(            nameof(knockback),   typeof(Item), () => 0,                                                  (prop) => new string[] { }, true);
        public Property<NumberValue>            healLife    { get; } = new Property<NumberValue>(           nameof(healLife),    typeof(Item), () => 0,                                                  (prop) => new string[] { }, true);
        public Property<NumberValue>            healMana    { get; } = new Property<NumberValue>(           nameof(healMana),    typeof(Item), () => 0,                                                  (prop) => new string[] { }, true);
        public Property<NumberValue>            defence     { get; } = new Property<NumberValue>(           nameof(defence),     typeof(Item), () => 0,                                                  (prop) => new string[] { }, true);
        public Property<NumberValue>            bait        { get; } = new Property<NumberValue>(           nameof(bait),        typeof(Item), () => 0,                                                  (prop) => new string[] { }, true);
        public Property<NumberValue>            useTime     { get; } = new Property<NumberValue>(           nameof(useTime),     typeof(Item), () => 1,                                                  (prop) => new string[] { }, true);

        public Property<BooleanValue>           questItem   { get; } = new Property<BooleanValue>(          nameof(questItem),   typeof(Item), () => false,                                              (prop) => new string[] { }, true);
        public Property<BooleanValue>           accessory   { get; } = new Property<BooleanValue>(          nameof(accessory),   typeof(Item), () => false,                                              (prop) => new string[] { }, true);
        public Property<BooleanValue>           potion      { get; } = new Property<BooleanValue>(          nameof(potion),      typeof(Item), () => false,                                              (prop) => new string[] { }, true);
        public Property<BooleanValue>           consumable  { get; } = new Property<BooleanValue>(          nameof(consumable),  typeof(Item), () => false,                                              (prop) => new string[] { }, true);
        public Property<BooleanValue>           melee       { get; } = new Property<BooleanValue>(          nameof(melee),       typeof(Item), () => false,                                              (prop) => new string[] { }, true);
        public Property<BooleanValue>           magic       { get; } = new Property<BooleanValue>(          nameof(magic),       typeof(Item), () => false,                                              (prop) => new string[] { }, true);
        public Property<BooleanValue>           ranged      { get; } = new Property<BooleanValue>(          nameof(ranged),      typeof(Item), () => false,                                              (prop) => new string[] { }, true);
        public Property<BooleanValue>           thrown      { get; } = new Property<BooleanValue>(          nameof(thrown),      typeof(Item), () => false,                                              (prop) => new string[] { }, true);
        public Property<BooleanValue>           summon      { get; } = new Property<BooleanValue>(          nameof(summon),      typeof(Item), () => false,                                              (prop) => new string[] { }, true);
        public Property<BooleanValue>           sentry      { get; } = new Property<BooleanValue>(          nameof(sentry),      typeof(Item), () => false,                                              (prop) => new string[] { }, true);

        public static Item item;

        static Item()
        {
            EnumerableValue.Register(nameof(preset), presets);
            EnumerableValue.Register(nameof(rare), rarity);

            item = new Item();
            item.className.value.value = item.key = "Item";
            item.parent.value.item = general;
        }

        public Item()
        {
            parent.value.item = item;
            ammo.value.filters.Add((items) => items.Where(item => (item as Item).preset.value == 4));
            block.value.filters.Add((items) => items.Where(item => (item as Item).preset.value == 12));
            parent.value.filters.Add(GeneralValue.ChildOf(item, true));
        }

        public override void Represent(UIElementCollection components)
        {
            base.Represent(components);
            components.Add(new ItemComponent(this));
            components.Add(new TextureComponent(this, texture));
            components.Add(new EventComponent(this, "При использовании"));
        }

        public class Recipe : IValue
        {
            public Property<BooleanValue>     enabled     { get; } = new Property<BooleanValue>(    nameof(enabled),     typeof(Recipe), () => true,                                                (prop) => new string[] { }, true);
            public PropertyList<GeneralValue> ingridients { get; } = new PropertyList<GeneralValue>(nameof(ingridients), typeof(Recipe), () => new GeneralValue(GeneralValue.ChildOf(item, false)), (prop) => new string[] { }, true);

            public event PropertyChangedEventHandler PropertyChanged;

            public XObject Pack(string name)
            {
                return PackElement(name);
            }

            public XElement PackElement(string name)
            {
                return new XElement(name,
                    enabled.Pack("enabled"),
                    ingridients.Pack("ingridients")
                    );
            }

            public void Restore(XAttribute data)
            {
                
            }

            public void Restore(XElement data)
            {
                if (data.Attribute("enabled") != null) enabled.Restore(data.Attribute("enabled"));
                else enabled.Restore(data.Element("enabled"));

                ingridients.Restore(data.Element("ingridients"));
            }
        }
    }
}
