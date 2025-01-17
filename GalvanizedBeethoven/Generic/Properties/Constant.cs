﻿using System;
using GalvanizedSoftware.Beethoven.Core;
using GalvanizedSoftware.Beethoven.Core.Properties;

namespace GalvanizedSoftware.Beethoven.Generic.Properties
{
  public class Constant<T> : IPropertyDefinition<T>
  {
    private readonly Action<T> errorHandler;
    private readonly T value;

    public Constant(T value) :
      this(value, null)
    {
    }

    public Constant(T value, Action<T> errorHandler)
    {
      this.value = value;
      this.errorHandler = errorHandler ??
                          (invalidValue =>
                            throw new ArgumentOutOfRangeException($"Value cannot be changed to {invalidValue}"));
    }

    // ReSharper disable once RedundantAssignment
    public bool InvokeGetter(InstanceMap instanceMap, ref T returnValue)
    {
      returnValue = value;
      return true;
    }

    public bool InvokeSetter(InstanceMap instanceMap, T newValue)
    {
      if (value.Equals(newValue))
        return true;
      errorHandler(newValue);
      return false;
    }
  }
}