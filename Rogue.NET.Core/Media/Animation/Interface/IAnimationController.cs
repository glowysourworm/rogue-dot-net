﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Media.Animation.Interface
{
    public interface IAnimationController
    {
        int AnimationTime { get; }

        void Start();
        void Stop();
        void Pause();
        void Resume();
    }
}
