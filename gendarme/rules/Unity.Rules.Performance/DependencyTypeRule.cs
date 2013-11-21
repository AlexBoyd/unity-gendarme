using System;
using System.Globalization;
using System.Linq;

using Gendarme.Framework;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Unity.Rules.Performance
{

    [Problem( "Dependancy from a Reference to another class within a  type defition")]
    [Solution( "Do some stuff" )]
    public class DependencyTypeRule : Rule, ITypeRule
    {
        public RuleResult CheckType( TypeDefinition type )
        {
            ComputeTypeDependencies(type);
            return Runner.CurrentRuleResult;
        }

        void ComputeTypeDependencies(TypeDefinition type) 
        {
            Runner.ReportDependancy(type, type, Severity.Audit, Confidence.Total);

            if (type.BaseType != null)
            {
                Runner.ReportDependancy(type, type.BaseType, Severity.Critical, Confidence.Total);
            }

            foreach (TypeReference typeRef in type.Interfaces)
            {
                Runner.ReportDependancy(type, type.BaseType, Severity.High, Confidence.Total);
            }

        }
    }
}
