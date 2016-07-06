using Rogue.NET.Common;
using Rogue.NET.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Rogue.NET.Model.Scenario
{
    /// <summary>
    /// Storage for Data about in-game items: Equipment, Consumables, Enemies, Doodads, etc...
    /// </summary>
    [Serializable]
    public class ScenarioMetaData : ScenarioObject
    {
        SymbolDetails _symbolInfo;
        bool _isIdentified;

        public string Type { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public bool IsIdentified
        {
            get { return _isIdentified; }
            set
            {
                _isIdentified = value;
                SetIdentifiedSymbol();
            }
        }
        public bool IsObjective { get; set; }
        public bool IsCursed { get; set; }
        public bool IsUnique { get; set; }
        public bool IsCurseIdentified { get; set; }
        public DungeonMetaDataObjectTypes ObjectType { get; set; }
        public SerializableObservableCollection<AttackAttributeTemplate> AttackAttributes { get; set; }
        public ScenarioMetaData()
        {
            this.RogueName = "";
            this.Type = "";
            this.Description = "";
            this.LongDescription = "";
            this.ObjectType = DungeonMetaDataObjectTypes.Item;
            this.AttackAttributes = new SerializableObservableCollection<AttackAttributeTemplate>();

            this.Height = ScenarioConfiguration.CELLHEIGHT * 3;
            this.Width = ScenarioConfiguration.CELLWIDTH * 3;
            this.IsPhysicallyVisible = true;

            this.SymbolInfo = new SymbolDetails(3, 10,
                                        "?",
                                        Colors.White.ToString());

            InitializeSymbol(this.SymbolInfo);
        }
        public ScenarioMetaData(SymbolDetails symDetails, IEnumerable<AttackAttributeTemplate> attackAttributes, DungeonMetaDataObjectTypes objType, bool cursed, bool unique, bool isObjective, string name, string type, string descr, string longdescr)
            : base(name)
        {
            this.RogueName = name;
            this.IsCursed = cursed;
            this.IsUnique = unique;
            this.Type = type;
            this.Description = descr;
            this.IsIdentified = false;          
            this.IsObjective = isObjective;
            this.LongDescription = longdescr;
            this.IsCurseIdentified = false;
            this.ObjectType = objType;
            this.AttackAttributes = new SerializableObservableCollection<AttackAttributeTemplate>(attackAttributes);

            this.Height = ScenarioConfiguration.CELLHEIGHT * 3;
            this.Width = ScenarioConfiguration.CELLWIDTH * 3;
            this.IsPhysicallyVisible = true;

            // unidentified symbol
            this.SymbolInfo = new SymbolDetails(3, 10,
                                        "?",
                                        Colors.White.ToString());

            InitializeSymbol(symDetails);
            InvalidateVisual();
        }

        public ScenarioMetaData(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var properties = typeof(ScenarioMetaData).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
            _symbolInfo = (SymbolDetails)info.GetValue("_symbolInfo", typeof(SymbolDetails));
            SetIdentifiedSymbol();
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(ScenarioMetaData).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));

            info.AddValue("_symbolInfo", _symbolInfo);
        }

        private void InitializeSymbol(SymbolDetails symDetails)
        {
            switch (symDetails.Type)
            {
                case SymbolTypes.Character:
                    _symbolInfo = new SymbolDetails(3, 10, symDetails.CharacterSymbol, symDetails.CharacterColor);
                    break;
                case SymbolTypes.Image:
                    _symbolInfo = new SymbolDetails(3, 10, symDetails.Icon);
                    break;
                case SymbolTypes.Smiley:
                    _symbolInfo = new SymbolDetails(3, 10, symDetails.SmileyMood, symDetails.SmileyBodyColor, symDetails.SmileyLineColor, symDetails.SmileyAuraColor);
                    break;
            }
            SetIdentifiedSymbol();
        }
        private void SetIdentifiedSymbol()
        {
            if (_isIdentified && _symbolInfo != null)
                this.SymbolInfo = _symbolInfo;

            else
                this.SymbolInfo = new SymbolDetails(3, 10,
                                        "?",
                                        Colors.White.ToString());
        }
    }
}
