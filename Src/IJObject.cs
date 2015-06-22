using System.Collections.Generic;

namespace Sdr.JsonMagic
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJObject : IJsonObjectRoot<object>
    {
        IDictionary<string, IJsonObjectRoot> Properties { get; }
    }
}