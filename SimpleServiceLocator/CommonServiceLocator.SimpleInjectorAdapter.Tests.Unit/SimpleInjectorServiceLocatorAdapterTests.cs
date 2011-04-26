﻿using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

namespace CommonServiceLocator.SimpleInjectorAdapter.Tests.Unit
{
    [TestClass]
    public class SimpleInjectorServiceLocatorAdapterTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullArgument_ThrowsExpectedException()
        {
            // Act
            new SimpleInjectorServiceLocatorAdapter(null);
        }

        [TestMethod]
        public void Ctor_ValidArgument_Succeeds()
        {
            // Act
            new SimpleInjectorServiceLocatorAdapter(new Container());
        }
    }
}