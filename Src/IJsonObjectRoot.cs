namespace Sdr.JsonMagic
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJsonObjectRoot
    {
        JObjectType Type { get; }
        object Value { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IJsonObjectRoot<out T> : IJsonObjectRoot
    {
        new T Value { get; }
    }
}