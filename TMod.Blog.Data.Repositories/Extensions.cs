using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Abstractions.Repositories;

namespace TMod.Blog.Data.Repositories
{
    public static class Extensions
    {
        public static IServiceCollection AddBlogRepositories(this IServiceCollection services)
        {
            Type[] repositoryTypes = Assembly.GetAssembly(typeof(Extensions))?.GetTypes().Where(p=>!p.IsAbstract && !p.IsInterface && p.IsRepositoryType()).ToArray()??[];
            foreach ( Type repositoryType in repositoryTypes )
            {
                RegistryBaseType(repositoryType, repositoryType.BaseType, services);
                foreach ( Type interfaceType in repositoryType.GetInterfaces() )
                {
                    RegistryInterfaceType(repositoryType,interfaceType, services);
                }
            }
            return services;
        }


        private static bool IsRepositoryType(this Type type)
        {
            return type.GetInterfaces()
                .Any(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IRepository<,>));
        }

        private static void RegistryBaseType(Type? type,Type? baseType,IServiceCollection services)
        {
            if(type is null )
            {
                return;
            }
            if(baseType is null )
            {
                return;
            }
            services.TryAddEnumerable(ServiceDescriptor.Scoped(baseType, type));
            RegistryBaseType(type,baseType.BaseType, services);
        }

        private static void RegistryInterfaceType(Type? type,Type? interfaceType,IServiceCollection services)
        {
            if(type is null )
            {
                return;
            }
            if(interfaceType is null )
            {
                return;
            }
            services.TryAddEnumerable(ServiceDescriptor.Scoped(interfaceType, type));
            foreach ( Type item in interfaceType.GetInterfaces() )
            {
                RegistryInterfaceType(type, item, services);
            }
        }
    }
}
