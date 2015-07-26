namespace Sdr.JsonMagic
{
    /// <summary>
    /// 
    /// </summary>
    public class JInteger : IJInteger
    {
        public int Object
        {
            get; private set;
        }

        public JObjectType Type
        {
            get 
            {
                return JObjectType.Integer;
            }
        }

        object IJsonObjectRoot.Object
        {
            get { 
                return this.Object; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public JInteger() : this(null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        public JInteger(string json)
        {
            int value;
            if (int.TryParse(json, out value))
            {
                Object = value;
            }
        }

    }
}