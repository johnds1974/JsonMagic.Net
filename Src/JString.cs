namespace Sdr.JsonMagic
{
    public class JString : IJString
    {
        public JObjectType Type {
            get { return JObjectType.String; }
        }

        public string Object { get; private set; }

        object IJsonObjectRoot.Object
        {
            get { return Object; }
        }

        public JString() : this(null)
        {
        }

        public JString(string json)
        {
            Object = json;
        }

    }
}