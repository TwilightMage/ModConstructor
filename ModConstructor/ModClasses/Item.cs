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

        public Property<EnumerableValue>                preset      { get; } = new Property<EnumerableValue>(               nameof(preset),      typeof(Item), () => new EnumerableValue(nameof(preset), 1),             true);
        public Property<GeneralValue>                   ammo        { get; } = new Property<GeneralValue>(                  nameof(ammo),        typeof(Item), () => new GeneralValue(GeneralValue.ChildOf(item, true)), true);
        public Property<GeneralValue>                   block       { get; } = new Property<GeneralValue>(                  nameof(block),       typeof(Item), () => new GeneralValue(GeneralValue.ChildOf(item, true)), true);
                                                                                                                            
        public Property<SpriteValue>                    texture     { get; } = new Property<SpriteValue>(                   nameof(texture),     typeof(Item), () => new SpriteValue(),                                  true);
        public Property<StringValueLocalizable.Bastard> displayName { get; } = new Property<StringValueLocalizable.Bastard>(nameof(displayName), typeof(Item), () => new StringValueLocalizable.Bastard("ItemName"),     true);
        public Property<StringValueLocalizable.Bastard> description { get; } = new Property<StringValueLocalizable.Bastard>(nameof(description), typeof(Item), () => new StringValueLocalizable.Bastard("ItemTooltip"),  true);
        public Property<EnumerableValue>                rare        { get; } = new Property<EnumerableValue>(               nameof(rare),        typeof(Item), () => new EnumerableValue(nameof(rare), 1),               true);
        public Property<MoneyValue>                     value       { get; } = new Property<MoneyValue>(                    nameof(value),       typeof(Item), () => 1000,                                               true);
        public Property<NumberValue>                    maxStack    { get; } = new Property<NumberValue>(                   nameof(maxStack),    typeof(Item), () => 99,                                                 true);
                                                                                                                            
        public Property<NumberValue>                    pick        { get; } = new Property<NumberValue>(                   nameof(pick),        typeof(Item), () => 0,                                                  true);
        public Property<NumberValue>                    axe         { get; } = new Property<NumberValue>(                   nameof(axe),         typeof(Item), () => 0,                                                  true);
        public Property<NumberValue>                    hammer      { get; } = new Property<NumberValue>(                   nameof(hammer),      typeof(Item), () => 0,                                                  true);
        public Property<NumberValue>                    damage      { get; } = new Property<NumberValue>(                   nameof(damage),      typeof(Item), () => 0,                                                  true);
        public Property<FloatValue>                     knockback   { get; } = new Property<FloatValue>(                    nameof(knockback),   typeof(Item), () => 0,                                                  true);
        public Property<NumberValue>                    healLife    { get; } = new Property<NumberValue>(                   nameof(healLife),    typeof(Item), () => 0,                                                  true);
        public Property<NumberValue>                    healMana    { get; } = new Property<NumberValue>(                   nameof(healMana),    typeof(Item), () => 0,                                                  true);
        public Property<NumberValue>                    defence     { get; } = new Property<NumberValue>(                   nameof(defence),     typeof(Item), () => 0,                                                  true);
        public Property<NumberValue>                    bait        { get; } = new Property<NumberValue>(                   nameof(bait),        typeof(Item), () => 0,                                                  true);
        public Property<NumberValue>                    useTime     { get; } = new Property<NumberValue>(                   nameof(useTime),     typeof(Item), () => 1,                                                  true);
                                                                                                                            
        public Property<BooleanValue>                   questItem   { get; } = new Property<BooleanValue>(                  nameof(questItem),   typeof(Item), () => false,                                              true);
        public Property<BooleanValue>                   accessory   { get; } = new Property<BooleanValue>(                  nameof(accessory),   typeof(Item), () => false,                                              true);
        public Property<BooleanValue>                   potion      { get; } = new Property<BooleanValue>(                  nameof(potion),      typeof(Item), () => false,                                              true);
        public Property<BooleanValue>                   consumable  { get; } = new Property<BooleanValue>(                  nameof(consumable),  typeof(Item), () => false,                                              true);
        public Property<BooleanValue>                   melee       { get; } = new Property<BooleanValue>(                  nameof(melee),       typeof(Item), () => false,                                              true);
        public Property<BooleanValue>                   magic       { get; } = new Property<BooleanValue>(                  nameof(magic),       typeof(Item), () => false,                                              true);
        public Property<BooleanValue>                   ranged      { get; } = new Property<BooleanValue>(                  nameof(ranged),      typeof(Item), () => false,                                              true);
        public Property<BooleanValue>                   thrown      { get; } = new Property<BooleanValue>(                  nameof(thrown),      typeof(Item), () => false,                                              true);
        public Property<BooleanValue>                   summon      { get; } = new Property<BooleanValue>(                  nameof(summon),      typeof(Item), () => false,                                              true);
        public Property<BooleanValue>                   sentry      { get; } = new Property<BooleanValue>(                  nameof(sentry),      typeof(Item), () => false,                                              true);

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
