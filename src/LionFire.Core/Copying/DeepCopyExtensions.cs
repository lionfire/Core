
// See also: Clone via IL DynamicMethod http://whizzodev.blogspot.ca/2008/03/object-cloning-using-il-in-c.html
// deep clone via IL: http://whizzodev.blogspot.ca/2008/06/object-deep-cloning-using-il-in-c.html

using LionFire;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using OX.Copyable;
using Microsoft.Extensions.Logging;
using LionFire.ExtensionMethods.Collections;
using LionFire.ExtensionMethods;

namespace LionFire.Copying
{
    public static class DeepCopyConfiguration
    {
        public static Predicate<Assembly> ShouldInspectAssembly { get; set; } = a => false;
    }
    /// <summary>
    /// This class defines all the extension methods provided by the Copyable framework 
    /// on the <see cref="System.Object"/> type.
    /// </summary>
    public static class DeepCopyObjectExtensions
    {       

        #region (Public) Extension Methods

        public static T DeepCopy<T>(this T instance, CopyFlags copyFlags = CopyFlagsSettings.Default)
            where T : class
        {
            if (instance == null)
                return null;
            return (T)DeepCopy(instance, DeduceInstance(instance), copyFlags);
        }

        public static T DeepCopy<T>(this T instance, T copy, CopyFlags copyFlags = CopyFlagsSettings.Default)
            where T : class
        {
            if (instance == null)
                return null;
            if (copy == null)
                throw new ArgumentNullException("The copy instance cannot be null");
            return (T)Clone(instance, new VisitedGraph(), copy, copyFlags);
        }

        // FUTURE: Another option: DeepCopy except use ICloneable if present?

        /// <summary>
        /// Creates a copy of the object.
        /// </summary>
        /// <param name="instance">The object to be copied.</param>
        /// <returns>A deep copy of the object.</returns>
        public static object DeepCopy(this object instance)
        {
            if (instance == null)
                return null;
            return DeepCopy(instance, DeduceInstance(instance));
        }

        /// <summary>
        /// Creates a deep copy of the object using the supplied object as a target for the copy operation.
        /// </summary>
        /// <param name="instance">The object to be copied.</param>
        /// <param name="copy">The object to copy values to. All fields of this object will be overwritten.</param>
        /// <returns>A deep copy of the object.</returns>
        public static object DeepCopy(this object instance, object copy)
        {
            if (instance == null)
                return null;
            if (copy == null)
                throw new ArgumentNullException("The copy instance cannot be null");
            return Clone(instance, new VisitedGraph(), copy);
        }

        #endregion

        static DeepCopyObjectExtensions()
        {
            InitializeInstanceProviders();
        }

        #region (Private)

        /// <summary>
        /// A list of instance providers that are available.
        /// </summary>
        private static List<IInstanceProvider> Providers => providers ??= InitializeInstanceProviders();
        private static List<IInstanceProvider> providers;

        private static Dictionary<Type, IInstanceProvider> ProvidersByType;
        private static Dictionary<Type, ICloneProvider> CloneProvidersByType;


        /// <summary>
        /// Updates the list of instance providers with any found in the newly loaded assembly.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="args">The event arguments.</param>
        private static void AssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            UpdateInstanceProviders(args.LoadedAssembly, Providers);
        }

        /// <summary>
        /// Initializes the list of instance providers.
        /// </summary>
        /// <returns>A list of instance providers that are used by the Copyable framework.</returns>
        private static List<IInstanceProvider> InitializeInstanceProviders()
        {
            ProvidersByType = new Dictionary<Type, IInstanceProvider>();
            CloneProvidersByType = new Dictionary<Type, ICloneProvider>();

            List<IInstanceProvider> providers = new List<IInstanceProvider>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!DeepCopyConfiguration.ShouldInspectAssembly(assembly)) continue;
                UpdateInstanceProviders(assembly, providers);
                UpdateCloneProviders(assembly);
            }
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(AssemblyLoaded);

            return providers;
        }

        /// <summary>
        /// Updates the list of instance providers with the ones found in the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly with which the list of instance providers will be updated.</param>
        private static void UpdateInstanceProviders(Assembly assembly, List<IInstanceProvider> providerList)
        {
            if (!DeepCopyConfiguration.ShouldInspectAssembly(assembly)) return;
            try
            {
                var dict = ProvidersByType;
                var providers = GetInstanceProviders(assembly);
                providerList.AddRange(providers);
                foreach (var provider in providers)
                {
                    foreach (var type in provider.TypesSupported)
                    {
                        if (!dict.TryAdd(type, provider))
                        {
                            l.Warn("DeepCopy instance provider alreay provided for type '" + type.FullName + "'.  Using: " + dict[type].GetType().FullName + " and ignoring " + provider.GetType().FullName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                l.Error($"DeepCopy - UpdateInstanceProviders failed for assembly '{assembly.FullName}'", ex);
            }
        }
        /// <summary>
        /// Updates the list of clone providers with the ones found in the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly with which the list of clone providers will be updated.</param>
        private static void UpdateCloneProviders(Assembly assembly)
        {
            try
            {
                var dict = CloneProvidersByType;
                var providers = GetCloneProviders(assembly);
                //providerList.AddRange(providers);
                foreach (var provider in providers)
                {
                    foreach (var type in provider.TypesSupported)
                    {
                        if (!dict.TryAdd(type, provider))
                        {
                            l.Warn("DeepCopy clone provider alreay provided for type '" + type.FullName + "'.  Using: " + dict[type].GetType().FullName + " and ignoring " + provider.GetType().FullName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                l.Error($"DeepCopy - UpdateInstanceProviders failed for assembly '{assembly.FullName}'", ex);
            }
        }

        /// <summary>
        /// Yields all instance providers defined in the assembly, if and only if they are instantiable
        /// without any arguments. <b>NOTE: Instance providers that cannot be instantiated in this 
        /// way are not used by the Copyable framework!</b>
        /// </summary>
        /// <param name="assembly">The assembly from which instance providers will be retrieved.</param>
        /// <returns>An <see cref="IEnumerable" /> of the instance providers of the assembly.</returns>
        private static IEnumerable<IInstanceProvider> GetInstanceProviders(Assembly assembly)
        {
            foreach (Type t in assembly.GetTypes())
            {
                if (t.IsInterface || t.ContainsGenericParameters) continue;
                if (typeof(IInstanceProvider).IsAssignableFrom(t))
                {
                    IInstanceProvider provider = null;
                    try
                    {
                        provider = (IInstanceProvider)Activator.CreateInstance(t);
                    }
                    catch { } // Ignore provider if it cannot be created
                    if (provider != null)
                        yield return provider;
                }
            }
        }

        /// <summary>
        /// Yields all clone providers defined in the assembly, if and only if they are instantiable
        /// without any arguments. <b>NOTE: Clone providers that cannot be instantiated in this 
        /// way are not used by the Copyable framework!</b>
        /// </summary>
        /// <param name="assembly">The assembly from which clone providers will be retrieved.</param>
        /// <returns>An <see cref="IEnumerable" /> of the clone providers of the assembly.</returns>
        private static IEnumerable<ICloneProvider> GetCloneProviders(Assembly assembly)
        {
            foreach (Type t in assembly.GetTypes())
            {
                if (t.IsInterface || t.ContainsGenericParameters) continue;
                if (typeof(ICloneProvider).IsAssignableFrom(t))
                {
                    ICloneProvider provider = null;
                    try
                    {
                        provider = (ICloneProvider)Activator.CreateInstance(t);
                    }
                    catch { } // Ignore provider if it cannot be created
                    if (provider != null)
                        yield return provider;
                }
            }
        }

        public static bool IsImmutableType(Type instanceType)
        {
            if (typeof(Type).IsAssignableFrom(instanceType))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a deep copy of an object using the supplied dictionary of visited objects as 
        /// a source of objects already encountered in the copy traversal. The dictionary of visited 
        /// objects is used for holding objects that have already been copied, to avoid erroneous 
        /// duplication of parts of the object graph.
        /// </summary>
        /// <param name="instance">The object to be copied.</param>
        /// <param name="visited">The graph of objects visited so far.</param>
        /// <returns></returns>
        private static object Clone(this object instance, VisitedGraph visited, CopyFlags copyFlags = CopyFlagsSettings.Default)
        {
            try
            {
                if (instance == null)
                    return null;

                Type instanceType = instance.GetType();

                if (IsImmutableType(instanceType)) // Don't bother copying immutable types
                {
                    return instance;
                }

                if (instanceType.IsPointer || instanceType == typeof(Pointer) || instanceType.IsPrimitive || instanceType == typeof(string)
                    || instanceType.IsEnum || instanceType.IsValueType)
                    return instance; // Pointers, primitive types and strings are considered immutable

                if (instanceType.IsArray)
                {
                    int length = ((Array)instance).Length;
                    Array copied = (Array)Activator.CreateInstance(instanceType, length);
                    visited.Add(instance, copied);
                    for (int i = 0; i < length; ++i)
                        copied.SetValue(((Array)instance).GetValue(i).Clone(visited), i);
                    return copied;
                }

                return Clone(instance, visited, DeduceInstance(instance), copyFlags);
            }
            catch (Exception)
            {
                if (CopyableSettings.AllowFallback)
                {
                    l.LogCritical("Error copying " + instance.GetType().Name + ". Falling back to non-copy.");
                    return instance;
                }
                else
                {
                    throw;
                }
            }
        }

        private static void CopyCollectionItems<T>(object srcO, object destO, AssignmentMode mode, object assignmentOptions)
        {
            ICollection<T> src = (ICollection<T>)srcO;
            ICollection<T> dest = (ICollection<T>)destO;
            foreach (T item in src)
            {
                var val = (T)PrepareForAssignment(item, mode, assignmentOptions);
                dest.Add(val);
            }
        }
        public static object PrepareForAssignment(object obj, AssignmentMode mode, object options)
        {
            var type = obj.GetType();
            if (type == typeof(string) || type.IsValueType) { mode = AssignmentMode.Assign; }

            switch (mode)
            {
                default:
                case AssignmentMode.Unspecified:
                    throw new ArgumentException("No mode specified");
                case AssignmentMode.Ignore:
                    throw new ArgumentException("Ignore mode should be ignored before passing it to this method");
                case AssignmentMode.Assign:
                    return obj;
                case AssignmentMode.CloneIfCloneable:
                    ICloneable c = obj as ICloneable;
                    if (c != null) return c.Clone();
                    return obj;
                case AssignmentMode.DeepCopy:
                    return obj.DeepCopy((CopyFlags)options);
            }
        }
        private static MethodInfo CopyCollectionItemsMI = typeof(DeepCopyObjectExtensions).GetMethod("CopyCollectionItems", BindingFlags.NonPublic | BindingFlags.Static);

        public static bool IsCollectionType(Type type, bool includeBaseClasses)
        {
            do
            {
                if (type.FullName.StartsWith("System.Collections.")) return true;
                if (type.FullName.StartsWith("LionFire.Collections.")) return true;
                // EXTENSIONPOINT
            } while (includeBaseClasses && ((type = type.BaseType) != null));

            return false;
        }

        private static CopyFlags GetExtraFlags(object instance)
        {
            var instanceType = instance.GetType();

            if (instanceType.IsGenericType)
            {
                var genType = instance.GetType().GetGenericTypeDefinition();
                if (genType != null)
                {
                    if (genType == typeof(KeyValuePair<,>))
                    {
                        return CopyFlags.NonPublicFields;
                    }
                }
            }
            return CopyFlags.None;
        }
        private static object Clone(this object instance, VisitedGraph visited, object copy, CopyFlags copyFlags = CopyFlagsSettings.Default)
        {
            if (instance == null) return null;

            #region Overrides

            bool finishedClone = false;
            IDeepCopyOverride dcOverride = copy as IDeepCopyOverride;
            if (dcOverride != null)
            {
                copy = dcOverride.Clone(instance, visited, copy, copyFlags);
                finishedClone = true;
            }

            // FUTURE - this relies on convention and may not be reliable.  Consider using an attribute to mark classes derived from collection classes that should be treated as collections.
            if (!finishedClone && IsCollectionType(instance.GetType(), true))
            {
                foreach (var iface in instance.GetType().GetInterfaces())
                {
#if !NET35
                    var args = iface.GenericTypeArguments;
#else
                    var args = iface.GetGenericArguments();
#endif
                    if (iface.Name == "ICollection`1" && args.Length == 1)
                    {
                        var mi = CopyCollectionItemsMI.MakeGenericMethod(args);
                        mi.Invoke(null, new object[] { instance, copy, AssignmentMode.DeepCopy, copyFlags });
                        finishedClone = IsCollectionType(instance.GetType(), false); // MICROOPTIMIZE This could be cached from the earlier invokation of IsCollectionType
                    }
                }
            }

            bool useICloneProviders = copyFlags.HasFlag(CopyFlags.ICloneProvider);
            if (!finishedClone && useICloneProviders)
            {
                var cloneProvider = CloneProvidersByType.TryGetValue(instance.GetType());
                if (cloneProvider != null)
                {
                    copy = cloneProvider.Clone(instance);
                    finishedClone = true;
                }
            }

            #endregion

            #region Visited graph

            if (visited.ContainsKey(instance))
                return visited[instance];
            else
                visited.Add(instance, copy);

            #endregion

            #region Early exit for overrides

            if (finishedClone)
            {
                return copy;
            }

            #endregion

            Type type = instance.GetType();

            while (type != null)
            {
                var effectiveFlags = copyFlags | GetExtraFlags(instance);

                bool useICloneable = effectiveFlags.HasFlag(CopyFlags.ICloneable);

                bool shallow = effectiveFlags.HasFlag(CopyFlags.Shallow);

                if (effectiveFlags.HasFlag(CopyFlags.PublicFields) || effectiveFlags.HasFlag(CopyFlags.NonPublicFields))
                {
                    BindingFlags bindingFlags = BindingFlags.Instance;
                    if (effectiveFlags.HasFlag(CopyFlags.PublicFields)) bindingFlags |= BindingFlags.Public;
                    //if (effectiveFlags.HasFlag(CopyFlags.NonPublicFields)) - always include nonpublic here, filter out later (REVIEW: Use same approach for public members to be consistent??)
                    bindingFlags |= BindingFlags.NonPublic;

                    foreach (FieldInfo field in type.GetFields(bindingFlags))
                    {
                        if (!CopyableSettings.CloneDelegates && typeof(Delegate).IsAssignableFrom(field.FieldType)) continue;
                        if (field.IsDefined(typeof(IgnoreAttribute)
#if NET35
, false
#endif
)) continue;
                        if (!field.IsPublic && !effectiveFlags.HasFlag(CopyFlags.NonPublicFields))
                        {
                            var attr = field.GetCustomAttribute<DeepCopyAttribute>();
                            if (attr == null) continue;
                        }

                        bool memberShallow = false;
                        {
                            var attr = field.GetCustomAttribute<AssignmentAttribute>();
                            if (attr != null)
                            {
                                if (attr.AssignmentMode == AssignmentMode.Ignore) continue;
                                if (attr.AssignmentMode == AssignmentMode.Assign) memberShallow = true;
                            }
                        }

                        object value = field.GetValue(instance);

                        if (useICloneable)
                        {
                            ICloneable cloneable = value as ICloneable;
                            if (cloneable != null)
                            {
                                value = cloneable.Clone();
                            }
                        }

                        if (visited.ContainsKey(value))
                            field.SetValue(copy, visited[value]);
                        else
                        {
                            if (!shallow)
                            {
                                if (!memberShallow)
                                {
                                    var attr = field.FieldType.GetCustomAttribute<AssignmentAttribute>();
                                    if (attr != null)
                                    {
                                        if (attr.AssignmentMode == AssignmentMode.Ignore) continue;
                                        if (attr.AssignmentMode == AssignmentMode.Assign) memberShallow = true;
                                    }
                                }
                            }
                            field.SetValue(copy, (shallow || memberShallow) ? value : value.Clone(visited, copyFlags));
                        }
                    }
                }

                if (effectiveFlags.HasFlag(CopyFlags.PublicProperties))
                {
                    BindingFlags bindingFlags = BindingFlags.Instance;
                    if (effectiveFlags.HasFlag(CopyFlags.PublicProperties)) bindingFlags |= BindingFlags.Public;
                    //if (effectiveFlags.HasFlag(CopyFlags.NonPublicProperties)) - always include nonpublic here, filter out later
                    bindingFlags |= BindingFlags.NonPublic;

                    foreach (PropertyInfo property in type.GetProperties(bindingFlags))
                    {
                        if (!property.CanWrite || !property.CanRead) continue;
                        if (property.GetIndexParameters().Length > 0) continue;

#if NET35
                        var getter = property.GetGetMethod();
                        var setter = property.GetSetMethod();
#else
                        var getter = property.GetMethod;
                        var setter = property.GetMethod;
#endif
                        if (getter == null || setter == null)
                        {
                            continue;
                        }

                        if (!effectiveFlags.HasFlag(CopyFlags.NonPublicProperties) && !(getter.IsPublic && setter.IsPublic))
                        {
                            var attr = property.GetCustomAttribute<DeepCopyAttribute>();
                            if (attr == null) continue;
                        }

                        if (!CopyableSettings.CloneDelegates && typeof(Delegate).IsAssignableFrom(property.PropertyType)) continue;
                        if (property.IsDefined(typeof(IgnoreAttribute)
#if NET35
, true
#endif
)) continue;

                        bool memberShallow = false;
                        {
                            var attr = property.GetCustomAttribute<AssignmentAttribute>();
                            if (attr != null)
                            {
                                if (attr.AssignmentMode == AssignmentMode.Ignore) continue;
                                if (attr.AssignmentMode == AssignmentMode.Assign) memberShallow = true;
                            }
                        }

                        object value = property.GetValue(instance
#if NET35
, null
#endif
);

                        if (useICloneable)
                        {
                            ICloneable cloneable = value as ICloneable;
                            if (cloneable != null)
                            {
                                value = cloneable.Clone();
                            }
                        }

                        if (visited.ContainsKey(value))
                            property.SetValue(copy, visited[value]
#if NET35
, null
#endif
);
                        else
                        {
                            if (!shallow)
                            {
                                if (!memberShallow)
                                {
                                    var attr = property.PropertyType.GetCustomAttribute<AssignmentAttribute>();
                                    if (attr != null)
                                    {
                                        if (attr.AssignmentMode == AssignmentMode.Ignore) continue;
                                        if (attr.AssignmentMode == AssignmentMode.Assign) memberShallow = true;
                                    }
                                }
                            }
                            property.SetValue(copy, (shallow || memberShallow) ? value : value.Clone(visited, copyFlags)
                                //#if NET35
                                //, null
                                //#endif
                                );
                        }
                    }
                }
                type = type.BaseType;
            }
            return copy;
        }


        private static object DeduceInstance(object instance)
        {
            Type instanceType = instance.GetType();

            if (typeof(Copyable).IsAssignableFrom(instanceType))
                return ((Copyable)instance).CreateInstanceForCopy();

            var provider = ProvidersByType.TryGetValue(instanceType);
            if (provider != null)
            {
                return provider.CreateInstance(instance);
            }
            //else // OLD, redundant
            //{
            //    foreach (IInstanceProvider providerItem in Providers)
            //    {
            //        if (provider.Provided == instanceType)
            //            return provider.CreateCopy(instance);
            //    }
            //}

            try
            {
                try
                {
                    return Activator.CreateInstance(instanceType);
                }
                catch
                {
                    return FormatterServices.GetUninitializedObject(instanceType);
                }
            }
            catch (Exception)
            {
                if (CopyableSettings.AllowFallback)
                {
                    l.Warn("Failed to copy " + instance.GetType().Name + ".  Falling back to just using the object reference.  TODO: Use MemberwiseClone?");
                    return instance; // return the instance!
                }
                else
                {
                    throw new ArgumentException(string.Format("Object of type {0} cannot be cloned because an uninitialized object could not be created.", instanceType.FullName));
                }
            }
        }

        #endregion

        #region Misc

        private static readonly ILogger l = Log.GetNonNull(typeof(DeepCopyObjectExtensions).FullName);

        #endregion
    }

}
