using Rogue.NET.Common.Serialization.Interface;

using System;
using System.IO;

namespace Rogue.NET.Common.Serialization.Formatter
{
    /// <summary>
    /// Base for binary formatter for any type (usually primitives)
    /// </summary>
    public abstract class BaseFormatter<T> : IBaseFormatter
    {
        /// <summary>
        /// Causes the target object to be rendered to the stream - expecting the TargetType as input. Advances
        /// Stream object by size rendered.
        /// </summary>
        /// <param name="stream">Stream to render the object to</param>
        /// <param name="theObject">Object target of TargetType</param>
        public void Write(Stream stream, object theObject)
        {
            // MAKING EXCEPTION FOR ENUM TYPES
            if (theObject.GetType().IsEnum &&
                this is EnumFormatter)
                WriteImpl(stream, (T)theObject);

            else if (theObject.GetType() != typeof(T))
                throw new Exception("Invalid target type for BaseFormatter:  " + theObject.GetType() + " is not a " + typeof(T));

            else
                WriteImpl(stream, (T)theObject);
        }

        /// <summary>
        /// Reads object of expected type from the stream. Advances stream object by that amount.
        /// </summary>
        public T Read(Stream stream)
        {
            return ReadImpl(stream);
        }

        /// <summary>
        /// Renders the object to the stream in the appropriate format. Advances stream the number
        /// of bytes rendered.
        /// </summary>
        protected abstract void WriteImpl(Stream stream, T theObject);

        /// <summary>
        /// Reads object of expected type from the stream. Advances stream object by that amount.
        /// </summary>
        protected abstract T ReadImpl(Stream stream);

        object IBaseFormatter.Read(Stream stream)
        {
            return Read(stream);
        }
    }
}
