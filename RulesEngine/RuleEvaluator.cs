using RulesEngine.Rules;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq.Expressions;

namespace RulesEngine
{
    internal class RuleEvaluator
    {
        private List<Rule> _rules = new List<Rule>();
        private List<VentilatorEvent> _events = new List<VentilatorEvent>();

        internal void Start()
        {
            InitializeRules();
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _events.Add(new VentilatorEvent(1, "Severe Hyperventilation", "1 & (2 | 3)"));
            _events.Add(new VentilatorEvent(2, "Severe Hypoxemia", "4 & 5 & (2 | 3)"));
            _events.Add(new VentilatorEvent(3, "Hypoxemia", "1 | 22"));
            _events.Add(new VentilatorEvent(4, "WAY TOO LOW SPO2", "99"));
            _events.Add(new VentilatorEvent(5, "something else", "8 | 3"));       
        }

        private void InitializeRules()
        {
            _rules.Add(new AlarmRule(1, "CO2lo"));
            _rules.Add(new AlarmRule(8, "SPO2hi"));
            _rules.Add(new MeasurementRule(4, "SFRatio", ExpressionType.LessThan, "153"));
            _rules.Add(new MeasurementDurationRule(5, "OSI", ExpressionType.GreaterThan, "7.8", TimeSpan.FromMinutes(60)));
            _rules.Add(new AlarmRule(2, "HRhi"));
            _rules.Add(new AlarmRule(3, "HRlo"));
            _rules.Add(new AlarmRule(6, "CO2hi"));
            _rules.Add(new MeasurementRule(22, "HR", ExpressionType.LessThan, "80"));
            _rules.Add(new MeasurementRule(99, "SPO2", ExpressionType.LessThan, "85"));
            
        }

        private ExpressionType GetExpressionType(string expr)
        {
            ExpressionType result;
            if (Enum.TryParse(expr, out result))
                return result;

            switch (expr.ToLower())
            {
                case ">":
                    result = ExpressionType.GreaterThan;
                    break;
                case ">=":
                    result = ExpressionType.GreaterThanOrEqual;
                    break;
                case "<":
                    result = ExpressionType.LessThan;
                    break;
                case "<=":
                    result = ExpressionType.LessThanOrEqual;
                    break;
                case "=":
                case "==":
                    result = ExpressionType.Equal;
                    break;
                default:
                    throw new ArgumentException($"Expression {expr} is not a valid expression type.");
            }

            return result;
        }

        private void EvaluateRules(Message message)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var results = new SortedDictionary<int, bool>(Comparer<int>.Create((x, y) => y.CompareTo(x)));

            foreach (Rule rule in _rules)
            {
                bool result = rule.Evaluate(message);
                results[rule.Id] = result;
            }

            ResolveVentilatorEvents(results);
            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;
        }

        private void ResolveVentilatorEvents(SortedDictionary<int, bool> results)
        {
            try
            {
                List<string> statuses = new List<string>();
                foreach (var possibleEvent in _events)
                {
                    // Replace all occurrences of a Rule ID with a boolean representing whether the rule was satsified
                    string infix = possibleEvent.Condition.Replace(results);

                    // If the result of the expression is true, the patient satisfies all rules in the condition
                    bool result = BinaryExpressionParser.ConvertAndEvaluate(infix);

                    if (result)
                        statuses.Add(possibleEvent.StatusName);
                }

                if (statuses.Count == 0)
                    statuses.Add("Acceptable");

                foreach (string status in statuses)
                {
                    Console.WriteLine($"Patient has status {status}");
                }
            }
            catch (FormatException fex)
            {
                Console.Error.WriteLine($"Invalid format in input string.");
                Console.Error.WriteLine(fex);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}