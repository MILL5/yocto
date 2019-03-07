#if NET472
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Windows.Markup;
using static yocto.Preconditions;
// ReSharper disable All

namespace yocto
{
    [MarkupExtensionReturnType(typeof(object))]
    public class ResolveResourceExtension : MarkupExtension
    {
        private static readonly ConcurrentDictionary<string, Type> _typeCache = new ConcurrentDictionary<string, Type>();
        string _resolveThis;

        public ResolveResourceExtension()
        {
        }

        public ResolveResourceExtension(string resolveThis)
        {
            CheckIsNotNull(nameof(resolveThis), resolveThis);

            _resolveThis = resolveThis;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            CheckIsNotNull<InvalidOperationException>(nameof(ResolveThis), ResolveThis);

            try
            {
                if (!_typeCache.TryGetValue(ResolveThis, out var type))
                {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                    type = (from a in assemblies
                        from t in a.GetTypes()
                        where t.Name.Equals(ResolveThis)
                        select t).SingleOrDefault();

                    if (type == null)
                    {
                        type = (from a in assemblies
                            from t in a.GetTypes()
                            where t.FullName != null && t.FullName.Equals(ResolveThis)
                            select t).SingleOrDefault();
                    }

                    if (type != null)
                    {
                        _typeCache.AddOrUpdate(ResolveThis, type, (k, v) => type);
                    }
                }

                ((IResolveByType)Application.Current).TryResolve(type, out object resolveThis);

                return resolveThis;
            }
            catch
            {
            }

            return null;
        }

        // Properties
        [ConstructorArgument("resolveThis")]
        public string ResolveThis
        {
            get
            {
                return _resolveThis;
            }
            set
            {
                CheckIsNotNull(nameof(ResolveThis), value);

                _resolveThis = value;
            }
        }
    }
}
#endif
