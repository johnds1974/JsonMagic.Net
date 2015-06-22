using System.Collections.Generic;

namespace Sdr.JsonMagic
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJArray : IJsonObjectRoot<object[]>
    {
        IList<IJsonObjectRoot> Items { get; }
    }
}