using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rogue.NET.Common;
using System.ComponentModel;
using System.Windows.Controls;
using Rogue.NET.Common.Views;
using System.Windows.Media;
using System.Windows;
using System.Runtime.Serialization;

namespace Rogue.NET.Model.Scenario
{
    [Serializable]
    public class SymbolDetails : FrameworkElement, ICloneable, INotifyPropertyChanged, ISerializable
    {
        //Had to add a setter for creating a combined symbol during alterations
        string _id = System.Guid.NewGuid().ToString();
        public string Id { get { return _id; } set { _id = value; } }

        Image _image = null;
        TextBlock _text = null;
        Smiley _smiley = null;
        double _scale = 1;

        /// <summary>
        /// If the child visual of this ObjectView is an Image, this method returns
        /// the Image.ImageSource property - otherwise it returns null. 
        /// </summary>
        public ImageSource SymbolImageSource
        {
            get
            {
                if (_image != null)
                    return _image.Source;

                else if (_smiley != null)
                    return ResourceManager.GetSmileyImage(_smiley.SmileyColor, _smiley.SmileyLineColor, _smiley.SmileyMood, _scale);

                else
                    return ResourceManager.GetScenarioObjectImage(((SolidColorBrush)_text.Foreground).Color, _text.Text, _scale);
            }
        }

        //To store the icon used to create the image
        ImageResources _icon = ImageResources.AmuletBlack;

        public SymbolTypes Type { get; set; }

        //Smiley Details
        public SmileyMoods SmileyMood { get; set; }
        public string SmileyBodyColor { get; set; }
        public string SmileyLineColor { get; set; }
        public string SmileyAuraColor { get; set; }

        //Character Details
        public string CharacterSymbol { get; set; }
        public string CharacterColor { get; set; }

        //Image Details
        public ImageResources Icon { get; set; }

        public double Scale { get { return _scale; } }

        public SymbolDetails()
        {
            this.Type = SymbolTypes.Image;
            this.Icon = ImageResources.Agility;
            Initialize(10, 1);
        }

        public SymbolDetails(double scaleFactor, int zindex, SmileyMoods mood, string body, string line, string aura)
        {
            this.Type = SymbolTypes.Smiley;
            this.SmileyMood = mood;
            this.SmileyBodyColor = body;
            this.SmileyLineColor = line;
            this.SmileyAuraColor = aura;

            Initialize(zindex, scaleFactor);
        }
        public SymbolDetails(double scaleFactor, int zindex, string symbol, string color)
        {
            this.Type = SymbolTypes.Character;
            this.CharacterSymbol = symbol;
            this.CharacterColor = color;

            Initialize(zindex, scaleFactor);
        }
        public SymbolDetails(double scaleFactor, int zindex, ImageResources icon)
        {
            this.Type = SymbolTypes.Image;
            this.Icon = icon;

            Initialize(zindex, scaleFactor);
        }

        public SymbolDetails(SerializationInfo info, StreamingContext context)
        {
            _id = info.GetString("Id");
            this.Type = (SymbolTypes)info.GetValue("Type", typeof(SymbolTypes));
            this.SmileyMood = (SmileyMoods)info.GetValue("SmileyMood", typeof(SmileyMoods));
            this.SmileyBodyColor = info.GetString("SmileyBodyColor");
            this.SmileyLineColor = info.GetString("SmileyLineColor");
            this.SmileyAuraColor = info.GetString("SmileyAuraColor");
            this.CharacterSymbol = info.GetString("CharacterSymbol");
            this.CharacterColor = info.GetString("CharacterColor");
            this.Icon = (ImageResources)info.GetValue("Icon", typeof(ImageResources));
            _scale = (double)info.GetValue("Scale", typeof(double));

            Initialize(10, _scale);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", _id);
            info.AddValue("Type", this.Type);
            info.AddValue("SmileyMood", this.SmileyMood);
            info.AddValue("SmileyBodyColor", this.SmileyBodyColor);
            info.AddValue("SmileyLineColor", this.SmileyLineColor);
            info.AddValue("SmileyAuraColor", this.SmileyAuraColor);
            info.AddValue("CharacterSymbol", this.CharacterSymbol);
            info.AddValue("CharacterColor", this.CharacterColor);
            info.AddValue("Icon", this.Icon);
            info.AddValue("Scale", this.Scale);
        }

        private void Initialize(int zindex, double scale)
        {
            _scale = scale;

            switch (this.Type)
            {
                case SymbolTypes.Character:
                    {
                        _image = null;
                        _smiley = null;
                        _text = new TextBlock();
                        _text.Background = Brushes.Transparent;
                        _text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.CharacterColor));
                        _text.Height = ScenarioConfiguration.CELLHEIGHT * scale;
                        _text.Width = ScenarioConfiguration.CELLWIDTH * scale;
                        _text.FontSize = 12 * (scale);
                        _text.FontFamily = ResourceManager.GetApplicationFontFamily();
                        _text.TextAlignment = TextAlignment.Center;
                        _text.Text = this.CharacterSymbol;
                        AddVisualChild(_text);
                    }
                    break;
                case SymbolTypes.Image:
                    {
                        _smiley = null;
                        _text = null;
                        _image = new Image();
                        _image.Height = ScenarioConfiguration.CELLHEIGHT * scale;
                        _image.Width = ScenarioConfiguration.CELLWIDTH * scale;
                        _image.Source = ResourceManager.GetScenarioObjectImage(this.Icon);
                        _icon = this.Icon;
                        AddVisualChild(_image);
                    }
                    break;
                case SymbolTypes.Smiley:
                    {
                        _image = null;
                        _text = null;
                        _smiley = new Smiley();
                        _smiley.SmileyColor = (Color)ColorConverter.ConvertFromString(this.SmileyBodyColor);
                        _smiley.SmileyLineColor = (Color)ColorConverter.ConvertFromString(this.SmileyLineColor);
                        _smiley.SmileyMood = this.SmileyMood;
                        _smiley.SmileyRadius = 2 * scale;
                        _smiley.Background = Brushes.Transparent;
                        _smiley.Foreground = Brushes.Transparent;
                        _smiley.BorderBrush = Brushes.Transparent;
                        _smiley.Height = ScenarioConfiguration.CELLHEIGHT * scale;
                        _smiley.Width = ScenarioConfiguration.CELLWIDTH * scale;
                        AddVisualChild(_smiley);
                    }
                    break;
            }
            InvalidateVisual();
        }
        public override string ToString()
        {
            switch (this.Type)
            {
                case SymbolTypes.Character:
                    return this.CharacterSymbol;
                case SymbolTypes.Image:
                    return this.Icon.ToString();
                case SymbolTypes.Smiley:
                default:
                    return this.SmileyBodyColor + " Smiley";
            }
        }

        //Use to produce a copy for generating symbol deltas
        public object Clone()
        {
            return MemberwiseClone();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #region Layout
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_text != null)
                _text.Measure(availableSize);

            else if (_image != null)
                _image.Measure(availableSize);

            else if (_smiley != null)
                _smiley.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_text != null)
                _text.Arrange(new Rect(finalSize));

            else if (_image != null)
                _image.Arrange(new Rect(finalSize));

            else if (_smiley != null)
                _smiley.Arrange(new Rect(finalSize));

            return base.ArrangeOverride(finalSize);
        }
        protected override Visual GetVisualChild(int index)
        {
            if (_text != null)
                return _text;

            else if (_image != null)
                return _image;

            else
                return _smiley;
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }
        #endregion
    }
}
