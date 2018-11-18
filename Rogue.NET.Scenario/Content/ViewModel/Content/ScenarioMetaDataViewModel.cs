using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Service.Interface;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class ScenarioMetaDataViewModel : Image, INotifyPropertyChanged
    {
        string _id;
        string _rogueName;
        string _type;
        string _description;
        string _longDescription;
        bool _isIdentified;
        bool _isObjective;
        bool _isCursed;
        bool _isUnique;
        bool _isCurseIdentified;
        DungeonMetaDataObjectTypes _objectType;
        ObservableCollection<AttackAttributeViewModel> AttackAttributes { get; set; }

        public string Id
        {
            get { return _id; }
            private set { this.RaiseAndSetIfChanged(ref _id, value); }
        }
        public string RogueName
        {
            get { return _rogueName; }
            private set { this.RaiseAndSetIfChanged(ref _rogueName, value); }
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

        public ScenarioMetaDataViewModel(ScenarioMetaData metaData, IScenarioResourceService scenarioResourceService)
        {
            this.Height = ModelConstants.CELLHEIGHT * 2;
            this.Width = ModelConstants.CELLWIDTH * 2;

            Update(metaData, scenarioResourceService);
        }

        public void Update(ScenarioMetaData metaData, IScenarioResourceService scenarioResourceService)
        {
            this.Id = metaData.Id;
            this.RogueName = metaData.RogueName;
            this.Type = metaData.Type;
            this.Description = metaData.Description;
            this.LongDescription = metaData.LongDescription;
            this.IsIdentified = metaData.IsIdentified;
            this.IsObjective = metaData.IsObjective;
            this.IsCursed = metaData.IsCursed;
            this.IsUnique = metaData.IsUnique;
            this.IsCurseIdentified = metaData.IsCurseIdentified;
            this.IsObjective = metaData.IsObjective;
            this.ObjectType = metaData.ObjectType;

            if (metaData.IsIdentified)
                this.Source = scenarioResourceService.GetImageSource(metaData);

            else
                this.Source = scenarioResourceService.GetImage("?", Colors.White.ToString());
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
