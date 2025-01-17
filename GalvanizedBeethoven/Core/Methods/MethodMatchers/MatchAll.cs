﻿using System;
using System.Collections.Generic;
using System.Linq;
using GalvanizedSoftware.Beethoven.Extensions;

namespace GalvanizedSoftware.Beethoven.Core.Methods.MethodMatchers
{
  public class MatchAll: IMethodMatcher
  {
    private readonly IEnumerable<IMethodMatcher> matchers;

    public MatchAll(IEnumerable<IMethodMatcher> matchers)
    {
      this.matchers = matchers;
    }

    public bool IsMatch((Type, string)[] parameters, Type[] genericArguments, Type returnType) =>
      matchers
        .Select(matcher => matcher.IsMatchEitherType(parameters, genericArguments, returnType))
        .AllAndNonEmpty();
  }
}