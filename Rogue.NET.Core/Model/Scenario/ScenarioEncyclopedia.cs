using Rogue.NET.Core.Model.Scenario.Abstract;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario
{
    [Serializable]
    public class ScenarioEncyclopedia : ISerializable
    {
        private Dictionary<string, ScenarioMetaData> _encyclopedia;
        private IEnumerable<ScenarioImage> _characterClasses;
        private IEnumerable<AlterationCategory> _alterationCategories;

        public ScenarioMetaData this[string rogueName]
        {
            get { return _encyclopedia[rogueName]; }
        }

        public IEnumerable<ScenarioImage> CharacterClasses { get { return _characterClasses; } }

        public IEnumerable<AlterationCategory> AlterationCategories { get { return _alterationCategories; } }

        public ScenarioEncyclopedia()
        {
            _encyclopedia = new Dictionary<string, ScenarioMetaData>();
        }
        public ScenarioEncyclopedia(IDictionary<string, ScenarioMetaData> encyclopedia, 
                                    IEnumerable<ScenarioImage> characterClasses, 
                                    IEnumerable<AlterationCategory> alterationCategories)
        {
            _encyclopedia = encyclopedia.ToDictionary(x => x.Key, x => x.Value);
            _characterClasses = characterClasses.ToList();
            _alterationCategories = alterationCategories.ToList();
        }
        public ScenarioEncyclopedia(SerializationInfo info, StreamingContext context)
        {
            _encyclopedia = (Dictionary<string, ScenarioMetaData>)info.GetValue("Encyclopedia", typeof(Dictionary<string, ScenarioMetaData>));
            _characterClasses = (IEnumerable<ScenarioImage>)info.GetValue("CharacterClasses", typeof(IEnumerable<ScenarioImage>));
            _alterationCategories = (IEnumerable<AlterationCategory>)info.GetValue("AlterationCategories", typeof(IEnumerable<AlterationCategory>));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Encyclopedia", _encyclopedia);
            info.AddValue("CharacterClasses", _characterClasses);
            info.AddValue("AlterationCategories", _alterationCategories);
        }

        #region Linq Support

        /// <summary>
        /// Returns the first or default entry in the encyclopedia that matches the search predicate
        /// </summary>
        public ScenarioMetaData Search(Func<ScenarioMetaData, bool> predicate)
        {
            return _encyclopedia.Values.FirstOrDefault(x => predicate(x));
        }

        /// <summary>
        /// Returns true if the specified search predicate returns true for any entry in the encyclopedia
        /// </summary>
        public bool Contains(Func<ScenarioMetaData, bool> predicate)
        {
            return _encyclopedia.Values.Any(x => predicate(x));
        }

        /// <summary>
        /// Returns true if the encyclopedia contains an entry for the specified RogueBase.RogueName
        /// </summary>
        public bool Contains(string rogueName)
        {
            return _encyclopedia.ContainsKey(rogueName);
        }

        /// <summary>
        /// Modifies each encyclopedia entry with the specified modifier
        /// </summary>
        public void ModifyEach(Action<ScenarioMetaData> modifier)
        {
            foreach (var metaData in _encyclopedia.Values)
                modifier(metaData);
        }

        /// <summary>
        /// Returns a subset of the encyclopedia entries specified by the search predicate
        /// </summary>
        public IEnumerable<ScenarioMetaData> SubSet(Func<ScenarioMetaData, bool> predicate)
        {
            return _encyclopedia.Values.Where(x => predicate(x));
        }
        #endregion
    }
}
