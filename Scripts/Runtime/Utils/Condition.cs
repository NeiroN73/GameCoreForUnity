using System;

namespace Game.Scripts.Utils
{
    [Serializable]
    public enum Condition
    {
        LessThan,        // <
        LessOrEqual,     // <=
        Equal,           // ==
        GreaterOrEqual,  // >=
        GreaterThan      // >
    }
}