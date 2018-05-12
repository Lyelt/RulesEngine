using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.Rules
{
    public class MeasurementDurationRule : MeasurementRule
    {
        private TimeSpan _duration;

        public MeasurementDurationRule(int ruleId, string measureId, ExpressionType op, object targetVal, TimeSpan duration)
            : base(ruleId, measureId, op, targetVal)
        {
            _duration = duration;
        }

        public override bool Evaluate(Message message)
        {
            // If the current message doesn't satisfy the condition, early return
            if (!base.Evaluate(message))
                return false;

            // Test every message in the patient's archive 
            bool hasValueForDuration = true;

            return hasValueForDuration;
        }
    }
}
