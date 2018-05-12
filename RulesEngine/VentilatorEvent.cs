using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine
{
    public class VentilatorEvent
    {
        public VentilatorEvent(int id, string name, string cond)
        {
            Id = id;
            StatusName = name;
            Condition = cond;
        }
        
        public int Id { get; set; }

        public string StatusName { get; set; }

        public string Condition { get; set; }
    }
}
