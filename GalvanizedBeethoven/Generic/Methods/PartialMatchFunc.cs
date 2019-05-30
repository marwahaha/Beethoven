﻿using GalvanizedSoftware.Beethoven.Core.Methods;
using GalvanizedSoftware.Beethoven.Extensions;
using System;
using System.Linq;
using System.Reflection;
using GalvanizedSoftware.Beethoven.Core.Methods.MethodMatchers;
using GalvanizedSoftware.Beethoven.Generic.Parameters;

namespace GalvanizedSoftware.Beethoven.Generic.Methods
{
  public class PartialMatchFunc : Method
  {
    private readonly MethodInfo methodInfo;
    private readonly object target;
    private readonly bool hasReturnType;
    private readonly (Type, string)[] localParameters;

    public PartialMatchFunc(string mainName, IParameter parameter, Delegate lambdaDelegate) :
      this(mainName, lambdaDelegate.Target, lambdaDelegate.Method, parameter)
    {
    }

    private PartialMatchFunc(string mainName, object target, MethodInfo lambdaMethodInfo, IParameter parameter) :
      base(mainName, new MatchLambdaPartiallyNoReturn(lambdaMethodInfo), parameter)
    {
      methodInfo = lambdaMethodInfo;
      localParameters = lambdaMethodInfo.GetParameterTypeAndNames();
      hasReturnType = lambdaMethodInfo.HasReturnType();
      this.target = target;
    }

    public override void Invoke(object localInstance, Action<object> returnAction, object[] parameters, Type[] genericArguments,
      MethodInfo masterMethodInfo)
    {
      (Type, string)[] masterParameters = masterMethodInfo
        .GetParameterTypeAndNames()
        .AppendReturnValue(masterMethodInfo.ReturnType)
        .ToArray();
      int[] indexes = localParameters
        .Select(item => Array.IndexOf(masterParameters, item))
        .ToArray();
      object[] localParameterValues = indexes
        .Select(index => parameters[index])
        .ToArray();
      object returnValue = methodInfo.Invoke(target, localParameterValues, genericArguments);
      if (hasReturnType)
        returnAction(returnValue);
    }
  }
}