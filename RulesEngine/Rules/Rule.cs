using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine
{
    public abstract class Rule
    {
        public int Id { get; private set; }

        public Rule(int ruleId)
        {
            Id = ruleId;
        }

        public abstract bool Evaluate(Message message);

        public override bool Equals(object obj)
        {
            if (obj is Rule other && other?.Id != null)
                return Id.Equals(other.Id);

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
