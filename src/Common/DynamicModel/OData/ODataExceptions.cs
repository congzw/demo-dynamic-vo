using System;

namespace Common.DynamicModel.OData
{
    [Serializable]
    public sealed class ODataParseException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ODataParseException()
        {
        }

        /// <summary>
        /// Message constructor
        /// </summary>
        public ODataParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Message and inner exception constructor
        /// </summary>
        public ODataParseException(string message, Exception inner)
            : base(message, inner)
        {
        }

        internal ODataParseException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}