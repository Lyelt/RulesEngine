using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.Rules
{
    public class AlarmDurationRule : AlarmRule
    {
        private TimeSpan _duration;

        public AlarmDurationRule(int ruleId, string alarmId, TimeSpan duration) : base(ruleId, alarmId)
        {
            _duration = duration;
        }

        public override bool Evaluate(Message message)
        {
            return base.Evaluate(message); // && alarm duration (from ventcheck) >= _duration
        }
    }
}
