namespace Sdr.JsonMagic
{
    public class JBoolean : IJBoolean
    {
        public JObjectType Type { get { return JObjectType.Bool; } }

        public bool Object { get; private set; }

        object IJsonObjectRoot.Object
        {
            get { return Object; }
        }

        public JBoolean() : this(null)
        {
        }

        public JBoolean(string json)
        {
            
        }
    }
}