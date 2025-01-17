﻿using System;
using System.Reflection;
using GalvanizedSoftware.Beethoven.Core.Methods;

namespace GalvanizedSoftware.Beethoven.Core.Interceptors
{
  internal sealed class MethodInterceptor : IGeneralInterceptor
  {
    private readonly Method method;

    internal MethodInterceptor(Method method)
    {
      MainDefinition = this.method = method;
    }

    public void Invoke(InstanceMap instanceMap, ref object returnValue, object[] parameters, Type[] genericArguments,
      MethodInfo methodInfo) =>
      method.InvokeFindInstance(instanceMap, ref returnValue, parameters, genericArguments, methodInfo);

    public object MainDefinition { get; }
  }
}
