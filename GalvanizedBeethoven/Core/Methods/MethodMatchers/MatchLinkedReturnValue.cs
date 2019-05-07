﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GalvanizedSoftware.Beethoven.Core.Methods.MethodMatchers
{
  public class MatchLinkedReturnValue: IMethodMatcher
  {
    private readonly IMethodMatcher[] methodList;

    internal MatchLinkedReturnValue(IEnumerable<IMethodMatcher> methodList)
    {
      this.methodList = methodList.ToArray();
    }

    public bool IsMatch((Type, string)[] parameters, Type[] genericArguments, Type returnType) =>
      methodList.Any(matcher => matcher.IsMatch(parameters, genericArguments, returnType)) ||
      methodList.Any(matcher => matcher.IsMatchToFlowControlled(parameters, genericArguments, returnType));
  }
}