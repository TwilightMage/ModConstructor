using ModConstructor.Controls;
using ModConstructor.ModClasses.Values;
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
            "Свой",                     // 0
            "Ингридиент",               // 1
            "Оружие ближнего боя",      // 2
            "Магическое оружие",        // 3
            "Стрелковое оружие",        // 4
            "Боеприпас",                // 5
            "Метательное оружие",       // 6
            "Оружие призыва",           // 7
            "Установка турели",         // 8
            "Броня",                    // 9
            "Аксессуар",                // 10
            "Инструмент",               // 11
            "Потребляемый",             // 12
            "Блок"                      // 13
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

        public SingleProperty<EnumerableValue>                preset      { get; } = new SingleProperty<EnumerableValue>(               nameof(preset),      typeof(Item), () => new EnumerableValue(nameof(preset), 1),             true);
        public SingleProperty<GeneralValue>                   ammo        { get; } = new SingleProperty<GeneralValue>(                  nameof(ammo),        typeof(Item), () => new GeneralValue(GeneralValue.ChildOf(item, true)), true);
        public SingleProperty<GeneralValue>                   block       { get; } = new SingleProperty<GeneralValue>(                  nameof(block),       typeof(Item), () => new GeneralValue(GeneralValue.ChildOf(item, true)), true);
                                                                                                                            
        public SingleProperty<SpriteValue>                    texture     { get; } = new SingleProperty<SpriteValue>(                   nameof(texture),     typeof(Item), () => new SpriteValue(),                                  true);
        public SingleProperty<StringValueLocalizable.Bastard> displayName { get; } = new SingleProperty<StringValueLocalizable.Bastard>(nameof(displayName), typeof(Item), () => new StringValueLocalizable.Bastard("ItemName"),     true);
        public SingleProperty<StringValueLocalizable.Bastard> description { get; } = new SingleProperty<StringValueLocalizable.Bastard>(nameof(description), typeof(Item), () => new StringValueLocalizable.Bastard("ItemTooltip"),  true);
        public SingleProperty<EnumerableValue>                rare        { get; } = new SingleProperty<EnumerableValue>(               nameof(rare),        typeof(Item), () => new EnumerableValue(nameof(rare), 1),               true);
        public SingleProperty<MoneyValue>                     value       { get; } = new SingleProperty<MoneyValue>(                    nameof(value),       typeof(Item), () => 1000,                                               true);
        public SingleProperty<NumberValue>                    maxStack    { get; } = new SingleProperty<NumberValue>(                   nameof(maxStack),    typeof(Item), () => 99,                                                 true);
                                                                                                                            
        public SingleProperty<NumberValue>                    pick        { get; } = new SingleProperty<NumberValue>(                   nameof(pick),        typeof(Item), () => 0,                                                  true);
        public SingleProperty<NumberValue>                    axe         { get; } = new SingleProperty<NumberValue>(                   nameof(axe),         typeof(Item), () => 0,                                                  true);
        public SingleProperty<NumberValue>                    hammer      { get; } = new SingleProperty<NumberValue>(                   nameof(hammer),      typeof(Item), () => 0,                                                  true);
        public SingleProperty<NumberValue>                    damage      { get; } = new SingleProperty<NumberValue>(                   nameof(damage),      typeof(Item), () => 0,                                                  true);
        public SingleProperty<FloatValue>                     knockback   { get; } = new SingleProperty<FloatValue>(                    nameof(knockback),   typeof(Item), () => 0,                                                  true);
        public SingleProperty<NumberValue>                    healLife    { get; } = new SingleProperty<NumberValue>(                   nameof(healLife),    typeof(Item), () => 0,                                                  true);
        public SingleProperty<NumberValue>                    healMana    { get; } = new SingleProperty<NumberValue>(                   nameof(healMana),    typeof(Item), () => 0,                                                  true);
        public SingleProperty<NumberValue>                    defence     { get; } = new SingleProperty<NumberValue>(                   nameof(defence),     typeof(Item), () => 0,                                                  true);
        public SingleProperty<NumberValue>                    bait        { get; } = new SingleProperty<NumberValue>(                   nameof(bait),        typeof(Item), () => 0,                                                  true);
        public SingleProperty<NumberValue>                    useTime     { get; } = new SingleProperty<NumberValue>(                   nameof(useTime),     typeof(Item), () => 1,                                                  true);
                                                                                                                            
        public SingleProperty<BooleanValue>                   questItem   { get; } = new SingleProperty<BooleanValue>(                  nameof(questItem),   typeof(Item), () => false,                                              true);
        public SingleProperty<BooleanValue>                   accessory   { get; } = new SingleProperty<BooleanValue>(                  nameof(accessory),   typeof(Item), () => false,                                              true);
        public SingleProperty<BooleanValue>                   potion      { get; } = new SingleProperty<BooleanValue>(                  nameof(potion),      typeof(Item), () => false,                                              true);
        public SingleProperty<BooleanValue>                   consumable  { get; } = new SingleProperty<BooleanValue>(                  nameof(consumable),  typeof(Item), () => false,                                              true);
        public SingleProperty<BooleanValue>                   melee       { get; } = new SingleProperty<BooleanValue>(                  nameof(melee),       typeof(Item), () => false,                                              true);
        public SingleProperty<BooleanValue>                   magic       { get; } = new SingleProperty<BooleanValue>(                  nameof(magic),       typeof(Item), () => false,                                              true);
        public SingleProperty<BooleanValue>                   ranged      { get; } = new SingleProperty<BooleanValue>(                  nameof(ranged),      typeof(Item), () => false,                                              true);
        public SingleProperty<BooleanValue>                   thrown      { get; } = new SingleProperty<BooleanValue>(                  nameof(thrown),      typeof(Item), () => false,                                              true);
        public SingleProperty<BooleanValue>                   summon      { get; } = new SingleProperty<BooleanValue>(                  nameof(summon),      typeof(Item), () => false,                                              true);
        public SingleProperty<BooleanValue>                   sentry      { get; } = new SingleProperty<BooleanValue>(                  nameof(sentry),      typeof(Item), () => false,                                              true);

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

        /*public class Recipe : IValue
        {
            public Property<BooleanValue>     enabled     { get; } = new Property<BooleanValue>(    nameof(enabled),     typeof(Recipe), () => true,                                                true);
            public PropertyList<GeneralValue> ingridients { get; } = new PropertyList<GeneralValue>(nameof(ingridients), typeof(Recipe), () => new GeneralValue(GeneralValue.ChildOf(item, false)), true);

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
        }*/
    }
}
