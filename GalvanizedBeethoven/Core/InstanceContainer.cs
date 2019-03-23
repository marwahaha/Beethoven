﻿using Castle.DynamicProxy;
using GalvanizedSoftware.Beethoven.Core.Binding;
using GalvanizedSoftware.Beethoven.Core.Events;
using GalvanizedSoftware.Beethoven.Core.Methods;
using GalvanizedSoftware.Beethoven.Core.Properties;
using GalvanizedSoftware.Beethoven.Generic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GalvanizedSoftware.Beethoven.Extensions;
using GalvanizedSoftware.Beethoven.Generic;
using GalvanizedSoftware.Beethoven.Generic.Methods;
using static GalvanizedSoftware.Beethoven.Core.Constants;

namespace GalvanizedSoftware.Beethoven.Core
{
  internal class InstanceContainer<T> : IObjectProvider where T : class
  {
    private readonly Dictionary<Type, object> masters = new Dictionary<Type, object>();
    private readonly IObjectProvider objectProviderHandler;
    private readonly PropertiesSignatureChecker<T> propertiesSignatureChecker = new PropertiesSignatureChecker<T>();

    public InstanceContainer(object[] partDefinitions)
    {
      AddMaster(new TargetBindingParent(this));
      EventInvokers = AddMaster(new EventInvokers(this));
      List<object> wrappers = GetWrappers(partDefinitions).ToList();
      wrappers.AddRange(GetDefaultImplementationWrappers(partDefinitions, wrappers));
      propertiesSignatureChecker.CheckSignatures(wrappers);
      MasterInterceptor masterInterceptor = new MasterInterceptor(
        new WrapperFactories(wrappers),
        new EventsFactory<T>(EventInvokers));
      AddMaster<IInterceptor>(masterInterceptor);
      objectProviderHandler = new ObjectProviderHandler(
        masters.Values
          .Concat(partDefinitions.OfType<IBindingParent>()));
    }

    public EventInvokers EventInvokers { get; }

    public IEnumerable<TChild> Get<TChild>()
    {
      return objectProviderHandler.Get<TChild>();
    }

    private static IEnumerable<object> GetWrappers(object[] partDefinitions)
    {
      foreach (object definition in partDefinitions)
      {
        switch (definition)
        {
          case null:
            break;
          case Property property:
            yield return property;
            break;
          case Method method:
            yield return method;
            break;
          case IEnumerable<Property> properties:
            foreach (Property property in properties)
              yield return property;
            break;
          case IEnumerable<Method> methods:
            foreach (Method method in methods)
              yield return method;
            break;
          case DefaultProperty _:
          case DefaultMethod _:
            // Dependent on what other wrappers are in there, so it has to be evaluated last
            break;
          case LinkedObjects linkedObjects:
            foreach (Property subProperty in linkedObjects.GetProperties())
              yield return subProperty;
            foreach (Method mappedMethod in linkedObjects.GetMethods<T>())
              yield return mappedMethod;
            break;
          default:
            foreach (Property subProperty in new PropertiesMapper(definition))
              yield return subProperty;
            foreach (Method mappedMethod in new MethodsMapper<T>(definition))
              yield return mappedMethod;
            break;
        }
      }
    }

    private static IEnumerable<object> GetDefaultImplementationWrappers(object[] partDefinitions, IList<object> wrappers)
    {
      return GetDefaultProperties(partDefinitions, wrappers.OfType<Property>())
        .Concat(GetDefaultMethods(partDefinitions));
    }

    private static IEnumerable<object> GetDefaultProperties(object[] partDefinitions, IEnumerable<Property> propertyWrappers)
    {
      DefaultProperty[] defaultProperties = partDefinitions.OfType<DefaultProperty>().ToArray();
      if (!defaultProperties.Any())
        yield break;
      DefaultProperty defaultProperty = defaultProperties.Single();
      IDictionary<string, Type> propertyInfos = typeof(T)
        .GetProperties(ResolveFlags)
        .ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.PropertyType);
      string[] typeProperties = propertyInfos.Keys.ToArray();
      HashSet<string> alreadyImplemented = new HashSet<string>(propertyWrappers.Select(p => p.Name));
      foreach (string propertyName in typeProperties.Except(alreadyImplemented))
        yield return defaultProperty.Create(propertyInfos[propertyName], propertyName);
    }

    private static IEnumerable<object> GetDefaultMethods(object[] partDefinitions)
    {
      DefaultMethod defaultMethod = partDefinitions.OfType<DefaultMethod>().SingleOrDefault();
      if (defaultMethod == null)
        yield break;
      MethodInfo[] methodInfos = typeof(T)
        .GetMethodsAndInherited()
        .ToArray();
      foreach (MethodInfo methodInfo in methodInfos)
        yield return defaultMethod.CreateMapped(methodInfo);
    }

    internal void Bind(T target)
    {
      foreach (IBindingParent bindingParent in Get<IBindingParent>())
        bindingParent.Bind(target);
    }

    internal TMaster GetMaster<TMaster>()
    {
      return masters.TryGetValue(typeof(TMaster), out object value) ? (TMaster)value : default;
    }

    private TMaster AddMaster<TMaster>(TMaster instance)
    {
      masters.Add(typeof(TMaster), instance);
      return instance;
    }
  }
}