using System;
using System.Globalization;
using System.Linq;

using Gendarme.Framework;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Unity.Rules.Performance
{

    [Problem( "Dependancy from a Method Reference")]
    [Solution( "Do some stuff" )]
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
                if ( code != Code.Callvirt && code != Code.Call) continue;

                MethodReference methodReference = instruction.Operand as MethodReference;
                if ( methodReference == null ) continue;

                MethodReference methodDef = methodReference.Resolve();
                if (methodDef.DeclaringType != method.DeclaringType)
                {
                    Runner.ReportDependancy(method.DeclaringType, methodReference.DeclaringType, instruction, Severity.Medium, Confidence.Total);
                }
            }

        }
    }
}
