﻿using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Item
{
    [Serializable]
    public class ItemBase : ScenarioObject
    {
        /// <summary>
        /// Identified individual item flag (reveals class / cursed / unique)
        /// </summary>
        public bool IsIdentified { get; set; }
        public double Weight { get; set; }

        public ItemBase()
        {
        }
        public ItemBase(string name, ImageResources icon) : base(name, icon)
        {
        }
    }
}