using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    public class SymbolDeltaTemplateViewModel : TemplateViewModel
    {
        private SymbolTypes _type;
        private SmileyMoods _smileyMood;
        private string _smileyBodyColor;
        private string _smileyLineColor;
        private string _smileyAuraColor;
        private string _characterSymbol;
        private string _characterColor;
        private ImageResources _icon;
        private DisplayImageResources _displayIcon;
        private bool _isFullSymbolDelta;
        private bool _isImageDelta;
        private bool _isMoodDelta;
        private bool _isBodyDelta;
        private bool _isLineDelta;
        private bool _isAuraDelta;
        private bool _isCharacterDelta;
        private bool _isColorDelta;

        public SymbolTypes Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public SmileyMoods SmileyMood
        {
            get { return _smileyMood; }
            set { this.RaiseAndSetIfChanged(ref _smileyMood, value); }
        }
        public string SmileyBodyColor
        {
            get { return _smileyBodyColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyBodyColor, value); }
        }
        public string SmileyLineColor
        {
            get { return _smileyLineColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyLineColor, value); }
        }
        public string SmileyAuraColor
        {
            get { return _smileyAuraColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyAuraColor, value); }
        }
        public string CharacterSymbol
        {
            get { return _characterSymbol; }
            set { this.RaiseAndSetIfChanged(ref _characterSymbol, value); }
        }
        public string CharacterColor
        {
            get { return _characterColor; }
            set { this.RaiseAndSetIfChanged(ref _characterColor, value); }
        }
        public ImageResources Icon
        {
            get { return _icon; }
            set { this.RaiseAndSetIfChanged(ref _icon, value); }
        }
        public DisplayImageResources DisplayIcon
        {
            get { return _displayIcon; }
            set { this.RaiseAndSetIfChanged(ref _displayIcon, value); }
        }
        public bool IsFullSymbolDelta
        {
            get { return _isFullSymbolDelta; }
            set { this.RaiseAndSetIfChanged(ref _isFullSymbolDelta, value); }
        }
        public bool IsImageDelta
        {
            get { return _isImageDelta; }
            set { this.RaiseAndSetIfChanged(ref _isImageDelta, value); }
        }
        public bool IsMoodDelta
        {
            get { return _isMoodDelta; }
            set { this.RaiseAndSetIfChanged(ref _isMoodDelta, value); }
        }
        public bool IsBodyDelta
        {
            get { return _isBodyDelta; }
            set { this.RaiseAndSetIfChanged(ref _isBodyDelta, value); }
        }
        public bool IsLineDelta
        {
            get { return _isLineDelta; }
            set { this.RaiseAndSetIfChanged(ref _isLineDelta, value); }
        }
        public bool IsAuraDelta
        {
            get { return _isAuraDelta; }
            set { this.RaiseAndSetIfChanged(ref _isAuraDelta, value); }
        }
        public bool IsCharacterDelta
        {
            get { return _isCharacterDelta; }
            set { this.RaiseAndSetIfChanged(ref _isCharacterDelta, value); }
        }
        public bool IsColorDelta
        {
            get { return _isColorDelta; }
            set { this.RaiseAndSetIfChanged(ref _isColorDelta, value); }
        }

        public SymbolDeltaTemplateViewModel()
        {
            this.Type = SymbolTypes.Image;
            this.Icon = ImageResources.AmuletOrange;
            this.DisplayIcon = DisplayImageResources.ChristianBlack;

            this.SmileyMood = SmileyMoods.Happy;
            this.SmileyBodyColor = Colors.Yellow.ToString();
            this.SmileyLineColor = Colors.Black.ToString();
            this.SmileyAuraColor = Colors.Yellow.ToString();

            this.CharacterColor = Colors.White.ToString();
            this.CharacterSymbol = "T";
        }
    }
}
