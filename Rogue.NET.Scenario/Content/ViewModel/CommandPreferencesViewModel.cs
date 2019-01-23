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
        public static readonly DependencyProperty FireProperty = DependencyProperty.Register("Fire", typeof(Key), typeof(CommandPreferencesViewModel));
        public static readonly DependencyProperty RenounceReligionProperty = DependencyProperty.Register("RenounceReligion", typeof(Key), typeof(CommandPreferencesViewModel));
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
        public Key RenounceReligion
        {
            get { return (Key)GetValue(RenounceReligionProperty); }
            set { SetValue(RenounceReligionProperty, value); }
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
            Skill = Key.X;
            Doodad = Key.D;
            Fire = Key.F;
            RenounceReligion = Key.R;
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
                Skill = Key.X,
                Doodad = Key.D,
                Fire = Key.F,
                RenounceReligion = Key.R
            };
        }
    }
}
