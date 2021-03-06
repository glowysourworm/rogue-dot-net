﻿using Rogue.NET.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common
{
    public class AlterationAttributeViewModel : NotifyViewModel
    {
        string _attributeName;
        string _attributeValue;

        public string AttributeName
        {
            get { return _attributeName; }
            set { this.RaiseAndSetIfChanged(ref _attributeName, value); }
        }
        public string AttributeValue
        {
            get { return _attributeValue; }
            set { this.RaiseAndSetIfChanged(ref _attributeValue, value); }
        }

        public AlterationAttributeViewModel()
        {

        }
        public AlterationAttributeViewModel(string name, string value)
        {
            this.AttributeName = name;
            this.AttributeValue = value;
        }
    }
}
