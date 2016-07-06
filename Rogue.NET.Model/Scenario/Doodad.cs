using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using Rogue.NET.Common;
using Rogue.NET.Scenario.Model;
using System.Runtime.Serialization;
using System.Reflection;

namespace Rogue.NET.Model.Scenario
{
    [Serializable]
    public abstract class Doodad : ScenarioObject
    {
        public bool IsOneUse { get; set; }
        public bool HasBeenUsed { get; set; }
        public DoodadType Type { get;set; }

        public Doodad()
        {
            this.SymbolInfo.Type = SymbolTypes.Image;
        }
        public Doodad(string name) : base(name) 
        {
            this.SymbolInfo.Type = SymbolTypes.Image;
        }
        public Doodad(string name, ImageResources icon, double scale)
            : base(name, icon, scale)
        {
        }
        public Doodad(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var properties = typeof(Doodad).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(Doodad).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));
        }
    }
    [Serializable]
    public class DoodadNormal : Doodad
    {
        public string PairId { get; set; }
        public DoodadNormalType NormalType { get; set; }
        public DoodadNormal() : base()
        {
        }
        public DoodadNormal(DoodadNormalType type, string name, string pairId, bool visible) : base(name)
        {
            this.Type = DoodadType.Normal;
            this.NormalType = type;
            this.IsPhysicallyVisible = visible;
            this.PairId = pairId;

            switch (type)
            {
                case DoodadNormalType.SavePoint:
                    this.SymbolInfo = new SymbolDetails(1, 10, ImageResources.SavePoint); break;
                case DoodadNormalType.Shop:
                    this.SymbolInfo = new SymbolDetails(1, 10, ImageResources.Shop); break;
                case DoodadNormalType.StairsDown:
                    this.SymbolInfo = new SymbolDetails(1, 10, ImageResources.StairsDown); break;
                case DoodadNormalType.StairsUp:
                    this.SymbolInfo = new SymbolDetails(1, 10, ImageResources.StairsUp); break;
                case DoodadNormalType.Teleport1:
                    this.SymbolInfo = new SymbolDetails(1, 10, ImageResources.teleport1); break;
                case DoodadNormalType.Teleport2:
                    this.SymbolInfo = new SymbolDetails(1, 10, ImageResources.teleport2); break;
                case DoodadNormalType.TeleportRandom:
                    this.SymbolInfo = new SymbolDetails(1, 10, ImageResources.TeleportRandom); break;

            }
        }
        public DoodadNormal(DoodadNormalType type, string name, string pairId, bool visible,double scale)
            : base(name, ImageResources.teleport2, scale)
        {
            this.Type = DoodadType.Normal;
            this.NormalType = type;
            this.IsPhysicallyVisible = visible;
            this.PairId = pairId;

            switch (type)
            {
                case DoodadNormalType.SavePoint:
                    this.SymbolInfo = new SymbolDetails(scale, 10, ImageResources.SavePoint); break;
                case DoodadNormalType.Shop:
                    this.SymbolInfo = new SymbolDetails(scale, 10, ImageResources.Shop); break;
                case DoodadNormalType.StairsDown:
                    this.SymbolInfo = new SymbolDetails(scale, 10, ImageResources.StairsDown); break;
                case DoodadNormalType.StairsUp:
                    this.SymbolInfo = new SymbolDetails(scale, 10, ImageResources.StairsUp); break;
                case DoodadNormalType.Teleport1:
                    this.SymbolInfo = new SymbolDetails(scale, 10, ImageResources.teleport1); break;
                case DoodadNormalType.Teleport2:
                    this.SymbolInfo = new SymbolDetails(scale, 10, ImageResources.teleport2); break;
                case DoodadNormalType.TeleportRandom:
                    this.SymbolInfo = new SymbolDetails(scale, 10, ImageResources.TeleportRandom); break;

            }
        }
        public DoodadNormal(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var properties = typeof(DoodadNormal).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(DoodadNormal).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));
        }
    }
    [Serializable]
    public class DoodadMagic : Doodad
    {
        public Spell AutomaticSpell { get; set; }
        public Spell InvokedSpell { get; set; }
        public bool IsAutomatic { get; set; }
        public bool IsInvoked { get; set; }

        public DoodadMagic() : base()
        { 
            this.Type = DoodadType.Magic;
            this.AutomaticSpell = new Spell();
            this.InvokedSpell = new Spell();
        }
        public DoodadMagic(string name, Spell autoSpell, Spell invokeSpell, bool isauto, bool isinvoke, ImageResources icon)
            : base()
        {
            this.Type = DoodadType.Magic;
            this.RogueName = name;
            this.AutomaticSpell = autoSpell;
            this.InvokedSpell = invokeSpell;
            this.IsAutomatic = isauto;
            this.IsInvoked = isinvoke;
            this.SymbolInfo.Icon = icon;
        }
        public DoodadMagic(string name, Spell autoSpell, Spell invokeSpell, bool isauto, bool isinvoke, ImageResources icon, double scale)
            : base(name, icon, scale)
        {
            this.Type = DoodadType.Magic;
            this.RogueName = name;
            this.AutomaticSpell = autoSpell;
            this.InvokedSpell = invokeSpell;
            this.IsAutomatic = isauto;
            this.IsInvoked = isinvoke;
            this.SymbolInfo.Icon = icon;
        }
        public DoodadMagic(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var properties = typeof(DoodadMagic).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(DoodadMagic).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));
        }
    }
}
