﻿using GalvanizedSoftware.Beethoven.Core.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GalvanizedSoftware.Beethoven.Extentions;

namespace GalvanizedSoftware.Beethoven.Generic.Methods
{
  public class LinkedMethods : Method
  {
    private readonly Method[] methodList;

    public LinkedMethods(string name) : base(name)
    {
      methodList = new Method[0];
    }

    private LinkedMethods(LinkedMethods previous, Method newMethod) :
      base(previous.Name)
    {
      methodList = previous.methodList.Append(newMethod).ToArray();
    }

    public LinkedMethods Lambda<T>(T actionOrFunc) =>
      new LinkedMethods(this, new LambdaMethod<T>(Name, actionOrFunc));

    public LinkedMethods MappedMethod(object instance, string targetName) =>
      new LinkedMethods(this, new MappedMethod(Name, instance, targetName));

    public LinkedMethods AutoMappedMethod(object instance) =>
      new LinkedMethods(this, new MappedMethod(Name, instance, Name));

    public override bool IsMatch(IEnumerable<Type> parameters, Type[] genericArguments, Type returnType)
    {
      return methodList.Any(method => method.IsMatch(parameters, genericArguments, returnType)) ||
             methodList.Any(method => method.IsMatch(parameters, genericArguments, typeof(bool)));
    }

    internal override void Invoke(Action<object> returnAction, object[] parameters, Type[] genericArguments, MethodInfo methodInfo)
    {
      Type[] parameterTypes = methodInfo.GetParameterTypes().ToArray();
      foreach (Method method in methodList)
      {
        if (!InvokeFirstMatch(method, parameters, parameterTypes, genericArguments, methodInfo))
          break;
      }
    }

    private bool InvokeFirstMatch(Method method, object[] parameters, Type[] parameterTypes,
      Type[] genericArguments, MethodInfo methodInfo)
    {
      Type returnType = methodInfo.ReturnType;
      if (method.IsMatch(parameterTypes, genericArguments, returnType))
      {
        method.Invoke(null, parameters, genericArguments, methodInfo);
        return true;
      }
      if (!method.IsMatch(parameterTypes, genericArguments, typeof(bool)))
        throw new MissingMethodException();
      bool result = true;
      Action<object> localReturn = newValue => result = (bool)newValue;
      method.Invoke(localReturn, parameters, genericArguments, methodInfo);
      return result;
    }
  }
}