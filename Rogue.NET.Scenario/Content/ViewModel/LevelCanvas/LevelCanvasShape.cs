using System.Windows.Media;
using System.Windows.Shapes;

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas
{
    public class LevelCanvasShape : Shape
    {
        Geometry _geometry;

        /// <summary>
        /// Id used for reference to the rendered scenario object property (Aura or Light Radius)
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Id to match the scenario object
        /// </summary>
        public string ScenarioObjectId { get; private set; }

        protected override Geometry DefiningGeometry { get { return _geometry; } }

        public override Geometry RenderedGeometry { get { return _geometry; } }

        public LevelCanvasShape(string id, string scenarioObjectId, Geometry geometry)
        {
            this.Id = id;
            this.ScenarioObjectId = scenarioObjectId;

            _geometry = geometry;
        }
    }
}
