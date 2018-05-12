using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine
{
    public class Message
    {
        public Dictionary<string, Measurement> measurements { get; set; }

        public Dictionary<string, Alarm> alarms { get; set; }
    }

    public class Measurement
    {
        public string MeasureId { get; set; }

        public string Value { get; set; }
    }

    public class Alarm
    {
        public string AlarmId { get; set; }

        public string Value { get; set; }
        
    }
}
