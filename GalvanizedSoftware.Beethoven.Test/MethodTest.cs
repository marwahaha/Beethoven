﻿using System;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GalvanizedSoftware.Beethoven.Test
{
  [TestClass]
  public class MethodTest
  {
    [TestMethod]
    public void MethodSimple()
    {
      ITestMethods fake = A.Fake<ITestMethods>();
      BeethovenFactory factory = new BeethovenFactory();
      ITestMethods test = factory.Generate<ITestMethods>(fake);
      test.Simple();
      A.CallTo(() => fake.Simple()).MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public void MethodReturnValue()
    {
      ITestMethods fake = A.Fake<ITestMethods>();
      BeethovenFactory factory = new BeethovenFactory();
      ITestMethods test = factory.Generate<ITestMethods>(fake);
      A.CallTo(() => fake.ReturnValue()).Returns(42);
      Assert.AreEqual(42, test.ReturnValue());
      A.CallTo(() => fake.ReturnValue()).MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public void MethodWithParameters1()
    {
      ITestMethods fake = A.Fake<ITestMethods>();
      BeethovenFactory factory = new BeethovenFactory();
      ITestMethods test = factory.Generate<ITestMethods>(fake);
      A.CallTo(() => fake.WithParameters("123", "abc")).Returns(42);
      A.CallTo(() => fake.WithParameters("", "")).Returns(41);
      A.CallTo(() => fake.WithParameters("123", "abc", 0)).Returns(0);
      Assert.AreEqual(42, test.WithParameters("123", "abc"));
      A.CallTo(() => fake.WithParameters("123", "abc")).MustHaveHappenedOnceExactly();
      A.CallTo(() => fake.WithParameters("123", "abc", 0)).MustNotHaveHappened();
      A.CallTo(() => fake.WithParameters("", "")).MustNotHaveHappened();
    }

    [TestMethod]
    public void MethodWithParameters2()
    {
      ITestMethods fake = A.Fake<ITestMethods>();
      BeethovenFactory factory = new BeethovenFactory();
      ITestMethods test = factory.Generate<ITestMethods>(fake);
      A.CallTo(() => fake.WithParameters("123", "abc", 5)).Returns(42);
      Assert.AreEqual(42, test.WithParameters("123", "abc", 5));
    }

    [TestMethod]
    public void MethodOutAndRef()
    {
      OutAndRefImplementation obj = new OutAndRefImplementation();
      BeethovenFactory factory = new BeethovenFactory();
      ITestMethods test = factory.Generate<ITestMethods>(obj);
      string text1 = "abc";
      string text2;
      Assert.AreEqual(19, test.OutAndRef(out text2, ref text1, 5));
      Assert.AreEqual("cba", text1);
      Assert.AreEqual("abc abc abc abc abc", text2);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void MethodInvalidSignature1()
    {
      BeethovenFactory factory = new BeethovenFactory();
      ITestMethods test = factory.Generate<ITestMethods>(new InvalidSignature());
      test.Simple();
    }
  }
}
