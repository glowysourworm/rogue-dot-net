
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.Attribute
{
    /// <summary>
    /// Attribute for describing a type (typically ViewModel) to the user
    /// </summary>
    public class UITypeAttribute : System.Attribute
    {
        /// <summary>
        /// Display name for the type
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Description for the type
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The type of view that is the editor for this type
        /// </summary>
        public Type ViewType { get; set; }
    }
}
