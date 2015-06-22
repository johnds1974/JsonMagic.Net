namespace Sdr.JsonMagic
{
    public class JNull : IJNull
    {
        public JObjectType Type { get { return JObjectType.Null;} }

        public object Value {
            get { return null; }
        }
    }
}