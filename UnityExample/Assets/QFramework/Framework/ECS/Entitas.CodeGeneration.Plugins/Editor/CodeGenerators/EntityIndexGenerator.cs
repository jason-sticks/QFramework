﻿using System.Collections.Generic;
using System.Linq;



namespace QFramework
{

    public class EntityIndexGenerator : ICodeGenerator, IConfigurable
    {

        public string Name
        {
            get { return "Entity Index"; }
        }

        public int Priority
        {
            get { return 0; }
        }

        public bool IsEnabledByDefault
        {
            get { return true; }
        }

        public bool RunInDryMode
        {
            get { return true; }
        }

        public Dictionary<string, string> DefaultProperties
        {
            get { return _ignoreNamespacesConfig.DefaultProperties; }
        }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string CLASS_TEMPLATE =
            @"public partial class Contexts {

${indexConstants}

    [QFramework.CodeGeneration.Attributes.PostConstructor]
    public void InitializeEntityIndices() {
${addIndices}
    }
}

public static class ContextsExtensions {

${getIndices}
}";

        const string INDEX_CONSTANTS_TEMPLATE = @"    public const string ${IndexName} = ""${IndexName}"";";

        const string ADD_INDEX_TEMPLATE =
            @"        ${contextName}.AddEntityIndex(new ${IndexType}<${ContextName}Entity, ${KeyType}>(
            ${IndexName},
            ${contextName}.GetGroup(${ContextName}Matcher.${IndexName}),
            (e, c) => ((${ComponentType})c).${MemberName}));";

        const string ADD_CUSTOM_INDEX_TEMPLATE =
            @"        ${contextName}.AddEntityIndex(new ${IndexType}(${contextName}));";

        const string GET_INDEX_TEMPLATE =
            @"    public static System.Collections.Generic.HashSet<${ContextName}Entity> GetEntitiesWith${IndexName}(this ${ContextName}Context context, ${KeyType} ${MemberName}) {
        return ((${IndexType}<${ContextName}Entity, ${KeyType}>)context.GetEntityIndex(Contexts.${IndexName})).GetEntities(${MemberName});
    }";

        const string GET_PRIMARY_INDEX_TEMPLATE =
            @"    public static ${ContextName}Entity GetEntityWith${IndexName}(this ${ContextName}Context context, ${KeyType} ${MemberName}) {
        return ((${IndexType}<${ContextName}Entity, ${KeyType}>)context.GetEntityIndex(Contexts.${IndexName})).GetEntity(${MemberName});
    }";

        const string CUSTOM_METHOD_TEMPLATE =
            @"    public static ${ReturnType} ${MethodName}(this ${ContextName}Context context, ${methodArgs}) {
        return ((${IndexType})(context.GetEntityIndex(Contexts.${IndexName}))).${MethodName}(${args});
    }
";

        public void Configure(Properties properties)
        {
            _ignoreNamespacesConfig.Configure(properties);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            var entityIndexData = data
                .OfType<EntityIndexData>()
                .OrderBy(d => d.GetEntityIndexName())
                .ToArray();

            return entityIndexData.Length == 0
                ? new CodeGenFile[0]
                : generateEntityIndices(entityIndexData);
        }

        CodeGenFile[] generateEntityIndices(EntityIndexData[] data)
        {

            var indexConstants = string.Join("\n", data
                .Select(d => INDEX_CONSTANTS_TEMPLATE.Replace("${IndexName}", d.GetEntityIndexName()))
                .ToArray());

            var addIndices = string.Join("\n\n", data
                .Select(d => generateAddMethods(d))
                .ToArray());

            var getIndices = string.Join("\n\n", data
                .Select(d => generateGetMethods(d))
                .ToArray());

            var fileContent = CLASS_TEMPLATE
                .Replace("${indexConstants}", indexConstants)
                .Replace("${addIndices}", addIndices)
                .Replace("${getIndices}", getIndices);

            return new[]
            {
                new CodeGenFile(
                    "Contexts.cs",
                    fileContent,
                    GetType().FullName
                )
            };
        }

        string generateAddMethods(EntityIndexData data)
        {
            return string.Join("\n", data.GetContextNames()
                .Aggregate(new List<string>(), (addMethods, contextName) =>
                {
                    addMethods.Add(generateAddMethod(data, contextName));
                    return addMethods;
                }).ToArray());
        }

        string generateAddMethod(EntityIndexData data, string contextName)
        {
            return data.IsCustom()
                ? generateCustomMethods(data)
                : generateMethods(data, contextName);
        }

        string generateCustomMethods(EntityIndexData data)
        {
            return ADD_CUSTOM_INDEX_TEMPLATE
                .Replace("${contextName}", data.GetContextNames()[0].LowercaseFirst())
                .Replace("${IndexType}", data.GetEntityIndexType());
        }

        string generateMethods(EntityIndexData data, string contextName)
        {
            return ADD_INDEX_TEMPLATE
                .Replace("${contextName}", contextName.LowercaseFirst())
                .Replace("${ContextName}", contextName)
                .Replace("${IndexName}", data.GetEntityIndexName())
                .Replace("${IndexType}", data.GetEntityIndexType())
                .Replace("${KeyType}", data.GetKeyType())
                .Replace("${ComponentType}", data.GetComponentType())
                .Replace("${MemberName}", data.GetMemberName())
                .Replace("${componentName}",
                    data.GetComponentType().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces).LowercaseFirst());
        }

        string generateGetMethods(EntityIndexData data)
        {
            return string.Join("\n\n", data.GetContextNames()
                .Aggregate(new List<string>(), (getMethods, contextName) =>
                {
                    getMethods.Add(generateGetMethod(data, contextName));
                    return getMethods;
                }).ToArray());
        }

        string generateGetMethod(EntityIndexData data, string contextName)
        {
            var template = "";
            if (data.GetEntityIndexType() == "QFramework.EntityIndex")
            {
                template = GET_INDEX_TEMPLATE;
            }
            else if (data.GetEntityIndexType() == "QFramework.PrimaryEntityIndex")
            {
                template = GET_PRIMARY_INDEX_TEMPLATE;
            }
            else
            {
                return getCustomMethods(data);
            }

            return template
                .Replace("${ContextName}", contextName)
                .Replace("${IndexName}", data.GetEntityIndexName())
                .Replace("${IndexType}", data.GetEntityIndexType())
                .Replace("${KeyType}", data.GetKeyType())
                .Replace("${MemberName}", data.GetMemberName());
        }

        string getCustomMethods(EntityIndexData data)
        {
            return string.Join("\n", data.GetCustomMethods()
                .Select(m => CUSTOM_METHOD_TEMPLATE
                    .Replace("${ReturnType}", m.returnType)
                    .Replace("${MethodName}", m.methodName)
                    .Replace("${ContextName}", data.GetContextNames()[0])
                    .Replace("${methodArgs}",
                        string.Join(", ", m.parameters.Select(p => p.type + " " + p.name).ToArray()))
                    .Replace("${IndexType}", data.GetEntityIndexType())
                    .Replace("${IndexName}", data.GetEntityIndexName())
                    .Replace("${args}", string.Join(", ", m.parameters.Select(p => p.name).ToArray()))).ToArray());
        }
    }
}