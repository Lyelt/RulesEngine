using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            RuleEvaluator eval = new RuleEvaluator();
            eval.Start();

            Console.ReadLine();
        }
    }
}
