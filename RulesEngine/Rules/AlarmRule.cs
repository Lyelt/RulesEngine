using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.Rules
{
    public class AlarmRule : Rule
    {
        private string _alarmId;

        public AlarmRule(int ruleId, string alarmId) : base(ruleId)
        {
            _alarmId = alarmId;

            Console.WriteLine($"Creating new rule with ID [{Id}]: when message contains alarm [{_alarmId}]");
        }

        public override bool Evaluate(Message message)
        {
            return message.alarms.ContainsKey(_alarmId);
        }
    }
}
