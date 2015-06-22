namespace Sdr.JsonMagic
{
    public class JString : IJString
    {
        public JObjectType Type {
            get { return JObjectType.String; }
        }
        public string Value { get; private set; }

        object IJsonObjectRoot.Value
        {
            get { return Value; }
        }

        public JString()
        {
        }

        public JString(string json)
        {
            
        }

    }
}