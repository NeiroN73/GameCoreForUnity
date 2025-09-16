using System;
using System.Collections.Generic;

namespace GameCore.SensorActionMachine
{
    [Serializable]
    public class EnemyBlackboardDropdown : BlackboardDropdown
    {
        protected override List<string> GetFields()
        {
            return BlackboardFields.GetFields(typeof(EnemyBlackboardDropdown));
        }
    }
}