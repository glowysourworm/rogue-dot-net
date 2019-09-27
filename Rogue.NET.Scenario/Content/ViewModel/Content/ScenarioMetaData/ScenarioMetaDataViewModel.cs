using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Processing.Service.Interface;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;

using ScenarioMetaDataClass = Rogue.NET.Core.Model.Scenario.ScenarioMetaData;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData
{
    public class ScenarioMetaDataViewModel : Image, INotifyPropertyChanged
    {
        string _id;
        string _rogueName;
        string _displayName;
        string _type;
        string _description;
        string _longDescription;
        bool _isIdentified;
        bool _isObjective;
        bool _isCursed;
        bool _isUnique;
        bool _isCurseIdentified;
        DungeonMetaDataObjectTypes _objectType;
        

        public string Id
        {
            get { return _id; }
            private set { this.RaiseAndSetIfChanged(ref _id, value); }
        }
        public string RogueName
        {
            get { return _rogueName; }
            set { this.RaiseAndSetIfChanged(ref _rogueName, value); }
        }
        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }
        public string Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public string Description
        {
            get { return _description; }
            set { this.RaiseAndSetIfChanged(ref _description, value); }
        }
        public string LongDescription
        {
            get { return _longDescription; }
            set { this.RaiseAndSetIfChanged(ref _longDescription, value); }
        }
        public bool IsIdentified
        {
            get { return _isIdentified; }
            set { this.RaiseAndSetIfChanged(ref _isIdentified, value); }
        }
        public bool IsObjective
        {
            get { return _isObjective; }
            set { this.RaiseAndSetIfChanged(ref _isObjective, value); }
        }
        public bool IsCursed
        {
            get { return _isCursed; }
            set { this.RaiseAndSetIfChanged(ref _isCursed, value); }
        }
        public bool IsUnique
        {
            get { return _isUnique; }
            set { this.RaiseAndSetIfChanged(ref _isUnique, value); }
        }
        public bool IsCurseIdentified
        {
            get { return _isCurseIdentified; }
            set { this.RaiseAndSetIfChanged(ref _isCurseIdentified, value); }
        }
        public DungeonMetaDataObjectTypes ObjectType
        {
            get { return _objectType; }
            set { this.RaiseAndSetIfChanged(ref _objectType, value); }
        }

        public ObservableCollection<ScenarioMetaDataAttackAttributeViewModel> AttackAttributes { get; set; }

        public ScenarioMetaDataViewModel(ScenarioMetaDataClass metaData, IScenarioResourceService scenarioResourceService)
        {
            this.Height = ModelConstants.CellHeight * 2;
            this.Width = ModelConstants.CellWidth * 2;

            this.AttackAttributes = new ObservableCollection<ScenarioMetaDataAttackAttributeViewModel>(
                                            metaData.AttackAttributes
                                                    .Where(x => x.Attack.IsSet() || x.Resistance.IsSet() || x.Weakness.IsSet())
                                                    .Select(x => new ScenarioMetaDataAttackAttributeViewModel(x)));

            Update(metaData, scenarioResourceService);
        }

        public void Update(ScenarioMetaDataClass metaData, IScenarioResourceService scenarioResourceService)
        {
            this.Id = metaData.Id;
            this.RogueName = metaData.RogueName;
            this.DisplayName = metaData.IsIdentified ? metaData.RogueName : "???";
            this.Type = metaData.Type;
            this.Description = metaData.IsIdentified ? metaData.Description : "???";
            this.LongDescription = metaData.IsIdentified ? metaData.LongDescription : "???";
            this.IsIdentified = metaData.IsIdentified;
            this.IsObjective = metaData.IsObjective;
            this.IsCursed = metaData.IsCursed;
            this.IsUnique = metaData.IsUnique;
            this.IsCurseIdentified = metaData.IsCurseIdentified;
            this.IsObjective = metaData.IsObjective;
            this.ObjectType = metaData.ObjectType;

            if (metaData.IsIdentified)
                this.Source = scenarioResourceService.GetImageSource(metaData, 1.0);

            // TODO:SYMBOL
            //else
            //    this.Source = scenarioResourceService.GetImageSource(new ScenarioImage("", "?", Colors.White.ToString()), 1.0);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string memberName = "")
        {
            var changed = false;
            if (field == null)
                changed = value != null;
            else
                changed = !field.Equals(value);

            if (changed)
            {
                field = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }
        }
    }
}
