using AutoMapper;
using System.Reflection;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            List<Type> types = [.. assembly.GetExportedTypes()
                                           .Where(t => t.GetInterfaces()
                                           .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))];

            foreach (Type type in types)
            {
                object instance = Activator.CreateInstance(type);
                MethodInfo methodInfo = type.GetMethod("Mapping");
                methodInfo?.Invoke(instance, [this]);
            }
        }
    }
}