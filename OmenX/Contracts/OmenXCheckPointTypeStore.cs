using System;
using System.Collections.Generic;

namespace OmenX.Contracts
{
    public class OmenXCheckPointTypeStore
    {
        public HashSet<Type> Types { get; set; } = new HashSet<Type>();
    }
}