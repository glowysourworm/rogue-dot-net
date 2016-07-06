using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Events.Splash
{
    public class SplashUpdateEvent : CompositePresentationEvent<SplashUpdateEvent>
    {
        /// <summary>
        /// sets loading message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Sets progress bar if there is one
        /// </summary>
        public double Progress { get; set; }
    }
}
