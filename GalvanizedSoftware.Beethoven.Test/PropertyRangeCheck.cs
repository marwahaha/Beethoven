﻿using GalvanizedSoftware.Beethoven.Core.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GalvanizedSoftware.Beethoven.Extentions;
using System;

namespace GalvanizedSoftware.Beethoven.Test
{
  [TestClass]
  public class PropertyRangeCheck
  {
    [TestMethod]
    public void TestMethodPropertyRangeCheck1()
    {
      BeethovenFactory factory = new BeethovenFactory();
      ITest test = factory.Generate<ITest>(
        new Property<int>(nameof(ITest.Property1))
        .RangeCheck(0, 42)
        .SetterGetter());
      Assert.AreEqual(0, test.Property1);
      test.Property1 = 42;
      Assert.AreEqual(42, test.Property1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestMethodPropertyRangeCheck2()
    {
      BeethovenFactory factory = new BeethovenFactory();
      ITest test = factory.Generate<ITest>(
        new Property<int>(nameof(ITest.Property1))
          .RangeCheck(0, 42));
      test.Property1 = 43;
      Assert.Fail();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestMethodPropertyRangeCheck3()
    {
      BeethovenFactory factory = new BeethovenFactory();
      ITest test = factory.Generate<ITest>(
        new Property<int>(nameof(ITest.Property1))
          .RangeCheck(0, 42));
      test.Property1 = -1;
      Assert.Fail();
    }
  }
}