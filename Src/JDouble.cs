namespace Sdr.JsonMagic
{
    /// <summary>
    /// 
    /// </summary>
    public class JDouble : IJDouble
    {
        public double Object
        {
            get; private set;
        }

        public JObjectType Type
        {
            get 
            {
                return JObjectType.Double;
            }
        }

        object IJsonObjectRoot.Object
        {
            get { throw new System.NotImplementedException(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public JDouble() : this(null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        public JDouble(string json)
        {
            double value;
            if (double.TryParse(json, out value))
            {
                Object = value;
            }
        }

    }
}