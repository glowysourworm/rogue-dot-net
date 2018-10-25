﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.Enums
{
    public enum LevelMessageType
    {
        Level,
        PlayerAdvancement
    }
    public enum LevelTemporaryEventType
    {
        RoamingLightSource
    }
    public enum PlayerStartLocation
    {
        AtCurrent,
        StairsUp,
        StairsDown,
        Random,
    }
}
