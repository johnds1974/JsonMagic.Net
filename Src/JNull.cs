namespace Sdr.JsonMagic
{
    public class JNull : IJNull
    {
        public JObjectType Type { get { return JObjectType.Null;} }

        public object Object {
            get { return null; }
        }
    }
}