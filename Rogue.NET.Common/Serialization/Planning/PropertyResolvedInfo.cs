using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Target;

using System.Reflection;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class PropertyResolvedInfo
    {
        public string PropertyName { get; set; }
        public ObjectInfo ResolvedInfo { get; set; }
        public bool IsUserDefined { get; set; }

        readonly PropertyInfo _reflectedInfo;

        internal PropertyResolvedInfo(PropertyInfo reflectedInfo)
        {
            _reflectedInfo = reflectedInfo;
        }

        internal PropertyInfo GetReflectedInfo()
        {
            if (this.IsUserDefined)
                throw new System.Exception("Trying to get reflected property info from user defined property PropertyResolvedInfo.cs");

            if (_reflectedInfo == null)
                throw new System.Exception("Internal error - Reflected property info is null: PropertyResolvedInfo.cs");

            return _reflectedInfo;
        }

        internal bool AreEqualProperties(PropertyResolvedInfo info)
        {
            return this.PropertyName.Equals(info.PropertyName) &&
                   this.ResolvedInfo.Type.Equals(info.ResolvedInfo.Type);
        }

        public override string ToString()
        {
            return this.FormatToString();
        }
    }
}
