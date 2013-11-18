using System;
using System.Globalization;
using System.Linq;

using Gendarme.Framework;

using Mono.Cecil;
using Mono.Cecil.Cil;

// TODO : update the severity report with a severity threshold based of the number of occurences of Component lookups
namespace Unity.Rules.Performance
{

    [Problem( "Dependancy from a Method Reference")]
    [Solution( "" )]
    public class MethodRefereneceDependancyRule : Rule, IMethodRule
    {
        public RuleResult CheckMethod( MethodDefinition method )
        {
            ComputeNumberOfMethodReferences(method);
            return Runner.CurrentRuleResult;
        }

        void ComputeNumberOfMethodReferences( MethodDefinition method )
        {
            if (method == null || !method.HasBody) return;

            foreach ( Instruction instruction in method.Body.Instructions )
            {
                Code code = instruction.OpCode.Code;
                if ( code != Code.Callvirt && code != Code.Call ) continue;

                MethodReference methodReference = instruction.Operand as MethodReference;
                if ( methodReference == null ) continue;

                MethodDefinition methodDef = methodReference.Resolve();
                if (methodReference != method)
                {
                    //Write Dependancy Report 
                    UnityEngine.Debug.Log("Found a dependancy Rule");
                    Runner.ReportDependancy(method, methodDef, Severity.Medium, Confidence.Total);
                }
            }

        }
    }
}
