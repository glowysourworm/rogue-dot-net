using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Formatter
{
    /// <summary>
    /// Base for binary formatter for any type (usually primitives)
    /// </summary>
    public abstract class BaseFormatter
    {
        /// <summary>
        /// The target type for this formatter
        /// </summary>
        public Type TargetType { get; private set; }

        /// <summary>
        /// Causes the target object to be rendered to the stream - expecting the TargetType as input. Advances
        /// Stream object by size rendered.
        /// </summary>
        /// <param name="stream">Stream to render the object to</param>
        /// <param name="theObject">Object target of TargetType</param>
        public void Write(Stream stream, object theObject)
        {
            if (theObject.GetType() != this.TargetType)
                throw new Exception("Invalid target type for formatter:  BaseFormatter.cs");

            WriteImpl(stream, theObject);
        }

        /// <summary>
        /// Renders the object to the stream in the appropriate format. Advances stream the number
        /// of bytes rendered.
        /// </summary>
        protected abstract void WriteImpl(Stream stream, object theObject);

        public BaseFormatter(Type targetType)
        {
            this.TargetType = targetType;
        }
    }
}
