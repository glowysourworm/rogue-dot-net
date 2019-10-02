using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Abstract
{
    [Serializable]
    public class DungeonObjectTemplate : Template
    {
        private SymbolDetailsTemplate _symbolDetails;
        private double _generationRate;
        private string _shortDescription;
        private string _longDescription;
        private bool _isCursed;
        private bool _isUnique;
        private bool _isObjectiveItem;
        private bool _hasBeenGenerated;

        public SymbolDetailsTemplate SymbolDetails
        {
            get { return _symbolDetails; }
            set
            {
                if (_symbolDetails != value)
                {
                    _symbolDetails = value;
                    OnPropertyChanged("SymbolDetails");
                }
            }
        }
        public double GenerationRate
        {
            get { return _generationRate; }
            set
            {
                if (_generationRate != value)
                {
                    _generationRate = value;
                    OnPropertyChanged("GenerationRate");
                }
            }
        }
        public string ShortDescription
        {
            get { return _shortDescription; }
            set
            {
                if (_shortDescription != value)
                {
                    _shortDescription = value;
                    OnPropertyChanged("ShortDescription");
                }
            }
        }
        public string LongDescription
        {
            get { return _longDescription; }
            set
            {
                if (_longDescription != value)
                {
                    _longDescription = value;
                    OnPropertyChanged("LongDescription");
                }
            }
        }
        public bool IsCursed
        {
            get { return _isCursed; }
            set
            {
                if (_isCursed != value)
                {
                    _isCursed = value;
                    OnPropertyChanged("IsCursed");
                }
            }
        }
        public bool IsUnique
        {
            get { return _isUnique; }
            set
            {
                if (_isUnique != value)
                {
                    _isUnique = value;
                    OnPropertyChanged("IsUnique");
                }
            }
        }
        public bool IsObjectiveItem
        {
            get { return _isObjectiveItem; }
            set
            {
                if (_isObjectiveItem != value)
                {
                    _isObjectiveItem = value;
                    OnPropertyChanged("IsObjectiveItem");
                }
            }
        }
        public bool HasBeenGenerated
        {
            get { return _hasBeenGenerated; }
            set
            {
                if (_hasBeenGenerated != value)
                {
                    _hasBeenGenerated = value;
                    OnPropertyChanged("HasBeenGenerated");
                }
            }
        }

        public DungeonObjectTemplate()
        {
            this.SymbolDetails = new SymbolDetailsTemplate();

            this.ShortDescription = "";
            this.LongDescription = "";
        }
        public DungeonObjectTemplate(DungeonObjectTemplate tmp)
        {
            this.SymbolDetails = tmp.SymbolDetails;
            this.GenerationRate = tmp.GenerationRate;
            this.ShortDescription = tmp.ShortDescription;
            this.LongDescription = tmp.LongDescription;
            this.IsCursed = tmp.IsCursed;
            this.IsUnique = tmp.IsUnique;
            this.IsObjectiveItem = tmp.IsObjectiveItem;
        }
    }
}
