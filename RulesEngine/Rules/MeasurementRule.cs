using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.Rules
{
    public class MeasurementRule : Rule
    {
        private string _measureId;
        private Expression _target;
        private ExpressionType _operator;

        public MeasurementRule(int ruleId, string measureId, ExpressionType op, object targetVal) : base(ruleId)
        {
            _measureId = measureId;
            _operator = op;

            if (double.TryParse(targetVal.ToString(), out double dVal))
                _target = Expression.Constant(dVal);
            else
                _target = Expression.Constant(targetVal);

            Console.WriteLine($"Creating new rule with ID [{Id}]: when CPC message contains measurement [{_measureId}] [{_operator}] [{_target}]");
        }

        public override bool Evaluate(Message message)
        {
            if (message.measurements.ContainsKey(_measureId))
            {
                object val = message.measurements[_measureId].Value;

                try
                {
                    // If the target value is a double type, we want to convert to match
                    val = Convert.ChangeType(val, _target.Type);
                    Expression left = Expression.Constant(val);

                    // Turn operator and operands into a binary expression
                    BinaryExpression binary = Expression.MakeBinary(_operator, left, _target);
                    Func<bool> evaluate = Expression.Lambda<Func<bool>>(binary).Compile();
                    return evaluate();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }

            return false;
        }
    }
}
