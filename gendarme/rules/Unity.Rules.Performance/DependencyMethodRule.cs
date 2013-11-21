using System;
using System.Globalization;
using System.Linq;

using Gendarme.Framework;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Unity.Rules.Performance
{

    [Problem( "Dependancy from a Reference to another class within a method")]
    [Solution( "Do some stuff" )]
    public class DependencyMethodRule : Rule, IMethodRule
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
                MethodReference methodReference = instruction.Operand as MethodReference;
                
                if(methodReference != null && methodReference.DeclaringType != null)
                { 
                    MethodReference methodDef = methodReference.Resolve();
                    if (methodDef.DeclaringType != method.DeclaringType)
                    {
                        Runner.ReportDependancy(method.DeclaringType, methodReference.DeclaringType, instruction, Severity.Medium, Confidence.Total);
                    }
                }

                PropertyReference propRef = instruction.Operand as PropertyReference;

                if (propRef != null && propRef.DeclaringType != null)
                {
                    PropertyDefinition propDef = propRef.Resolve();
                    if (propDef.DeclaringType != method.DeclaringType)
                    {
                        Runner.ReportDependancy(method.DeclaringType, propDef.DeclaringType, instruction, Severity.Low, Confidence.Total);
                    }
                }

                MemberReference memberRef = instruction.Operand as MemberReference;

                if (memberRef != null && memberRef.DeclaringType != null)
                {
                    if (memberRef.DeclaringType != method.DeclaringType)
                    {
                        Runner.ReportDependancy(method, memberRef.DeclaringType, instruction, Severity.High, Confidence.Total);
                    }
                }
            }

        }
    }
}
