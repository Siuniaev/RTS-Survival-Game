using Assets.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.DI
{
    /// <summary>
    /// Dependency injection container, composition root.
    /// Uses properties with <see cref="InjectionAttribute" /> attribute for injection.
    /// Can inject itself as a dependency IDependencyInjector so that it can be used to make injections into newly created objects in real-time.
    /// Resolves objects with suppression of endless circular injections.
    /// </summary>
    /// <seealso cref="IDependencyInjector" />
    internal sealed class DIContainer : IDependencyInjector
    {
        private readonly Dictionary<Type, Func<object>> registrations;
        private readonly Dictionary<Type, object> startedResolving;
        private readonly Dictionary<Type, object> fullyResolved;

        /// <summary>
        /// Initializes a new instance of the <see cref="DIContainer"/> class.
        /// </summary>
        public DIContainer()
        {
            registrations = new Dictionary<Type, Func<object>>();
            startedResolving = new Dictionary<Type, object>();
            fullyResolved = new Dictionary<Type, object>();

            fullyResolved.Add(typeof(IDependencyInjector), this);
        }

        /// <summary>
        /// Registers the Component type implementing the service interface that should be present in the scene as a singleton.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImpl">The type of the implementation.</typeparam>
        public void RegisterSingleton<TService, TImpl>()
            where TService : class
            where TImpl : Component, TService
        {
            object func() => FindOrCreateSingleton<TImpl>();
            registrations.Add(typeof(TService), func);
        }

        /// <summary>
        /// Registers the Component type that should be present in the scene as a singleton.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        public void RegisterSingleton<T>()
            where T : Component
        {
            object func() => FindOrCreateSingleton<T>();
            registrations.Add(typeof(T), func);
        }

        /// <summary>
        /// Registers the type implementing the service interface.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImpl">The type of the implementation.</typeparam>
        public void RegisterType<TService, TImpl>()
            where TService : class
            where TImpl : TService, new()
        {
            registrations.Add(typeof(TService), () => Activator.CreateInstance<TImpl>());
        }

        /// <summary>
        /// Registers the two types implementing the service interface.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="T1">The first type of the implementation.</typeparam>
        /// <typeparam name="T2">The second type of the implementation.</typeparam>
        public void RegisterTypes<TService, T1, T2>()
            where TService : class
            where T1 : TService, new()
            where T2 : TService, new()
        {
            registrations.Add(typeof(TService), () => new TService[]
            {
                Activator.CreateInstance<T1>(),
                Activator.CreateInstance<T2>()
            } as object);
        }

        /// <summary>
        /// Registers the three types implementing the service interface.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="T1">The first type of the implementation.</typeparam>
        /// <typeparam name="T2">The second type of the implementation.</typeparam>
        /// <typeparam name="T3">The third type of the implementation.</typeparam>
        public void RegisterTypes<TService, T1, T2, T3>()
            where TService : class
            where T1 : TService, new()
            where T2 : TService, new()
            where T3 : TService, new()
        {
            registrations.Add(typeof(TService), () => new TService[]
            {
                Activator.CreateInstance<T1>(),
                Activator.CreateInstance<T2>(),
                Activator.CreateInstance<T3>()
            } as object);
        }

        /// <summary>
        /// Registers the Component type that should be exist in the scene. Multiple instances allowed.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        public void RegisterSceneObjects<T>()
            where T : Component
        {
            object func() => FindSceneObjectsOrCreateOne<T>();
            registrations.Add(typeof(T), func);
        }

        /// <summary>
        /// Returns the resolved instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The resolved instance.</returns>
        public T Resolve<T>() => (T)Resolve(typeof(T));

        /// <summary>
        /// Injects dependencies into the specified object.
        /// </summary>
        /// <param name="injectable">The injectable object.</param>
        /// <exception cref="Exception">Can't resolve property \"{propertyInfo.Name}\" in \"{type.FullName}\"</exception>
        public void MakeInjections(object injectable)
        {
            var objs = injectable is Array array ? array : new[] { injectable };

            foreach (var obj in objs)
            {
                var type = obj.GetType();
                var members = type.FindMembers(MemberTypes.Property, BindingFlags.FlattenHierarchy |
                    BindingFlags.SetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null);

                foreach (var member in members)
                {
                    var attributes = member.GetCustomAttributes(typeof(InjectionAttribute), true);
                    if (!attributes.Any())
                        continue;

                    var propertyInfo = (PropertyInfo)member;
                    object objForInjection;

                    var injectionType = propertyInfo.PropertyType.IsArray ? propertyInfo.PropertyType.GetElementType() : propertyInfo.PropertyType;

                    try
                    {
                        objForInjection = Resolve(injectionType);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Can't resolve property \"{propertyInfo.Name}\" in \"{type.FullName}\"", ex);
                    }

                    propertyInfo.SetValue(obj, objForInjection);
                }

                (obj as IInitiableOnInjecting)?.OnInjected();
            }
        }

        /// <summary>
        /// Finds the scene objects of the specified Component type or create one.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>Finded or created instance.</returns>
        private object FindSceneObjectsOrCreateOne<T>()
            where T : Component
        {
            return Object.FindObjectsOfType<T>() as object ?? new GameObject(typeof(T).Name).AddComponent<T>();
        }

        /// <summary>
        /// Finds the Component type in the scene and return it, destroying the rest found.
        /// If no objects found, creates and return an empty object with this Component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>Finded or created instance as singleton.</returns>
        private object FindOrCreateSingleton<T>()
            where T : Component
        {
            var finded = Object.FindObjectsOfType<T>();
            var singleton = finded?.FirstOrDefault() ?? new GameObject(typeof(T).Name).AddComponent<T>();

            if (finded != null && finded.Length > 1)
                finded.Where(component => component != singleton).ForEach(Object.Destroy);

            singleton.transform.SetParent(null);
            Object.DontDestroyOnLoad(singleton);

            return singleton;
        }

        /// <summary>
        /// Returns the resolved instance of the specified type.
        /// Resolving objects is in three steps to suppress endless circular injection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The resolved instance.</returns>
        /// <exception cref="InvalidOperationException">No registration for {nameof(type)}</exception>
        private object Resolve(Type type)
        {
            if (fullyResolved.TryGetValue(type, out var resolved))
                return resolved;

            if (startedResolving.TryGetValue(type, out var resolving))
                return resolving;

            if (registrations.TryGetValue(type, out var func))
            {
                var obj = func();
                startedResolving.Add(type, obj);
                MakeInjections(obj);
                fullyResolved.Add(type, obj);
                startedResolving.Remove(type);

                return obj;
            }

            throw new InvalidOperationException($"No registration for {nameof(type)}");
        }
    }
}
