﻿using System.Collections.Generic;
using System.Linq;
using GalvanizedSoftware.Beethoven.Core.Properties;
using GalvanizedSoftware.Beethoven.Generic;
using GalvanizedSoftware.Beethoven.Generic.Parameters;
using Tuple = System.Tuple<GalvanizedSoftware.Beethoven.Generic.Parameters.IParameter, object>;

namespace GalvanizedSoftware.Beethoven.Core
{
  public class InstanceMap : IInstanceMap
  {
    private readonly Dictionary<IParameter, object> dictionary;

    public InstanceMap(object[] partDefinitions, IEnumerable<object> parameters)
    {
      IParameter[] array = partDefinitions.OfType<IParameter>()
        .Concat(partDefinitions
          .OfType<PropertyDefinition>()
          .Select(property => property.Parameter)
          .Where(parameter => parameter != null))
        .ToArray();
      dictionary = array
            .OfType<ConstructorParameter>()
            .Zip(parameters, (parameter, instance) => new Tuple(parameter, instance))
          .Concat(array
            .OfType<AutoParameter>()
            .Select(parameter => new Tuple(parameter, null)))
        .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
    }

    public object GetLocal(IParameter parameter)
    {
      KeyValuePair<IParameter, object> existing = dictionary
        .FirstOrDefault(item => item.Key.Equals(parameter));
      return existing.Value ?? Create(parameter as AutoParameter, existing.Key != null);
    }

    private object Create(AutoParameter autoParameter, bool exists)
    {
      if (autoParameter == null)
        return null;
      object instance = autoParameter.Create();
      if (!exists)
        dictionary.Add(autoParameter, instance);
      return instance;
    }
  }
}