using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Design
{
    [Serializable]
    public class ScenarioDesignTemplate : Template
    {
        private string _objectiveDescription;
        public string ObjectiveDescription
        {
            get { return _objectiveDescription; }
            set
            {
                if (_objectiveDescription != value)
                {
                    _objectiveDescription = value;
                    OnPropertyChanged("ObjectiveDescription");
                }
            }
        }
        public List<LevelDesignTemplate> LevelDesigns { get; set; }
        public ScenarioDesignTemplate()
        {
            this.LevelDesigns = new List<LevelDesignTemplate>();
            this.ObjectiveDescription = "Objective Description (Goes Here)";
        }
    }
}
