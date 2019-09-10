using System.Windows;
using System.Windows.Input;

namespace Rogue.NET.Model
{
    public class CommandPreferencesViewModel : DependencyObject
    {
        #region Dependency Property
        public static readonly DependencyProperty NorthProperty = DependencyProperty.Register("North", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty SouthProperty = DependencyProperty.Register("South", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty EastProperty = DependencyProperty.Register("East", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty WestProperty = DependencyProperty.Register("West", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty NorthEastProperty = DependencyProperty.Register("NorthEast", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty NorthWestProperty = DependencyProperty.Register("NorthWest", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty SouthEastProperty = DependencyProperty.Register("SouthEast", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty SouthWestProperty = DependencyProperty.Register("SouthWest", typeof(Key), typeof(CommandPreferencesViewModel));

        public static readonly DependencyProperty AttackProperty = DependencyProperty.Register("Attack", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty OpenProperty = DependencyProperty.Register("Open", typeof(Key), typeof(CommandPreferencesViewModel));

        public static readonly DependencyProperty SearchProperty = DependencyProperty.Register("Search", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty DoodadProperty = DependencyProperty.Register("Doodad", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty SkillProperty = DependencyProperty.Register("Skill", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty SelectTargetProperty = DependencyProperty.Register("SelectTarget", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty EndTargetingProperty = DependencyProperty.Register("EndTargeting", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty FireProperty = DependencyProperty.Register("Fire", typeof(Key), typeof(CommandPreferencesViewModel));

        public static readonly DependencyProperty ShowPlayerSubpanelEquipmentProperty = DependencyProperty.Register("ShowPlayerSubpanelEquipment", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty ShowPlayerSubpanelConsumablesProperty = DependencyProperty.Register("ShowPlayerSubpanelConsumables", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty ShowPlayerSubpanelSkillsProperty = DependencyProperty.Register("ShowPlayerSubpanelSkills", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty ShowPlayerSubpanelStatsProperty = DependencyProperty.Register("ShowPlayerSubpanelStats", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty ShowPlayerSubpanelAlterationsProperty = DependencyProperty.Register("ShowPlayerSubpanelAlterations", typeof(Key), typeof(CommandPreferencesViewModel));
        #endregion

        //Compass Keys
        public Key North 
        {
            get { return (Key)GetValue(NorthProperty); }
            set { SetValue(NorthProperty, value); }
        }
        public Key South
        {
            get { return (Key)GetValue(SouthProperty); }
            set { SetValue(SouthProperty, value); }
        }
        public Key East
        {
            get { return (Key)GetValue(EastProperty); }
            set { SetValue(EastProperty, value); }
        }
        public Key West
        {
            get { return (Key)GetValue(WestProperty); }
            set { SetValue(WestProperty, value); }
        }
        public Key NorthEast
        {
            get { return (Key)GetValue(NorthEastProperty); }
            set { SetValue(NorthEastProperty, value); }
        }
        public Key NorthWest
        {
            get { return (Key)GetValue(NorthWestProperty); }
            set { SetValue(NorthWestProperty, value); }
        }
        public Key SouthEast
        {
            get { return (Key)GetValue(SouthEastProperty); }
            set { SetValue(SouthEastProperty, value); }
        }
        public Key SouthWest
        {
            get { return (Key)GetValue(SouthWestProperty); }
            set { SetValue(SouthWestProperty, value); }
        }

        //Compass Modifiers
        public Key AttackModifier
        {
            get { return (Key)GetValue(AttackProperty); }
            set { SetValue(AttackProperty, value); }
        }
        public Key OpenModifier
        {
            get { return (Key)GetValue(OpenProperty); }
            set { SetValue(OpenProperty, value); }
        }

        //Level actions
        public Key Search
        {
            get { return (Key)GetValue(SearchProperty); }
            set { SetValue(SearchProperty, value); }
        }
        public Key Target
        {
            get { return (Key)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
        public Key SelectTarget
        {
            get { return (Key)GetValue(SelectTargetProperty); }
            set { SetValue(SelectTargetProperty, value); }
        }
        public Key EndTargeting
        {
            get { return (Key)GetValue(EndTargetingProperty); }
            set { SetValue(EndTargetingProperty, value); }
        }
        public Key Skill
        {
            get { return (Key)GetValue(SkillProperty); }
            set { SetValue(SkillProperty, value); }
        }
        public Key Doodad
        {
            get { return (Key)GetValue(DoodadProperty); }
            set { SetValue(DoodadProperty, value); }
        }
        public Key Fire
        {
            get { return (Key)GetValue(FireProperty); }
            set { SetValue(FireProperty, value); }
        }

        public Key ShowPlayerSubpanelEquipment
        {
            get { return (Key)GetValue(ShowPlayerSubpanelEquipmentProperty); }
            set { SetValue(ShowPlayerSubpanelEquipmentProperty, value); }
        }
        public Key ShowPlayerSubpanelConsumables
        {
            get { return (Key)GetValue(ShowPlayerSubpanelConsumablesProperty); }
            set { SetValue(ShowPlayerSubpanelConsumablesProperty, value); }
        }
        public Key ShowPlayerSubpanelSkills
        {
            get { return (Key)GetValue(ShowPlayerSubpanelSkillsProperty); }
            set { SetValue(ShowPlayerSubpanelSkillsProperty, value); }
        }
        public Key ShowPlayerSubpanelStats
        {
            get { return (Key)GetValue(ShowPlayerSubpanelStatsProperty); }
            set { SetValue(ShowPlayerSubpanelStatsProperty, value); }
        }
        public Key ShowPlayerSubpanelAlterations
        {
            get { return (Key)GetValue(ShowPlayerSubpanelAlterationsProperty); }
            set { SetValue(ShowPlayerSubpanelAlterationsProperty, value); }
        }

        public CommandPreferencesViewModel()
        {
            North = Key.O;
            South = Key.OemPeriod;
            East = Key.OemSemicolon;
            West = Key.K;
            NorthEast = Key.P;
            NorthWest = Key.I;
            SouthEast = Key.OemQuestion;
            SouthWest = Key.OemComma;

            AttackModifier = Key.LeftCtrl | Key.RightCtrl;
            OpenModifier = Key.LeftShift | Key.RightShift;;
            Search = Key.S;
            Target = Key.T;
            SelectTarget = Key.Enter;
            EndTargeting = Key.Escape;
            Skill = Key.X;
            Doodad = Key.D;
            Fire = Key.F;

            ShowPlayerSubpanelEquipment = Key.D1;
            ShowPlayerSubpanelConsumables = Key.D2;
            ShowPlayerSubpanelSkills = Key.D3;
            ShowPlayerSubpanelStats = Key.D4;
            ShowPlayerSubpanelAlterations = Key.D5;
        }
        public static CommandPreferencesViewModel GetDefaults()
        {
            return new CommandPreferencesViewModel() {

                North = Key.O,
                South = Key.OemPeriod,
                East = Key.OemSemicolon,
                West = Key.K,
                NorthEast = Key.P,
                NorthWest = Key.I,
                SouthEast = Key.OemQuestion,
                SouthWest = Key.OemComma,

                AttackModifier = Key.LeftCtrl | Key.RightCtrl,
                OpenModifier = Key.LeftShift | Key.RightShift,

                Search = Key.S,
                Target = Key.T,
                SelectTarget = Key.Enter,
                EndTargeting = Key.Escape,
                Skill = Key.X,
                Doodad = Key.D,
                Fire = Key.F,

                ShowPlayerSubpanelEquipment = Key.D1,
                ShowPlayerSubpanelConsumables = Key.D2,
                ShowPlayerSubpanelSkills = Key.D3,
                ShowPlayerSubpanelStats = Key.D4,
                ShowPlayerSubpanelAlterations = Key.D5
            };
        }
    }
}
