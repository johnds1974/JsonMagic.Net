namespace Sdr.JsonMagic
{
    public class JBoolean : IJBoolean
    {
        public JObjectType Type { get { return JObjectType.Bool; } }

        public bool Value { get; private set; }

        object IJsonObjectRoot.Value
        {
            get { return Value; }
        }

        public JBoolean() : this(null)
        {
        }

        public JBoolean(string json)
        {
            
        }
    }
}