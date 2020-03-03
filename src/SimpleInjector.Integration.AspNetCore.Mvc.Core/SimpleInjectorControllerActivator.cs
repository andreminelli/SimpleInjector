﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Integration.AspNetCore.Mvc
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;

    /// <summary>Controller activator for Simple Injector.</summary>
    public sealed class SimpleInjectorControllerActivator : IControllerActivator
    {
        private readonly ConcurrentDictionary<Type, InstanceProducer?> controllerProducers =
            new ConcurrentDictionary<Type, InstanceProducer?>();

        private readonly Container container;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleInjectorControllerActivator"/> class.
        /// </summary>
        /// <param name="container">The container instance.</param>
        public SimpleInjectorControllerActivator(Container container)
        {
            Requires.IsNotNull(container, nameof(container));

            this.container = container;
        }

        /// <summary>Creates a controller.</summary>
        /// <param name="context">The Microsoft.AspNet.Mvc.ActionContext for the executing action.</param>
        /// <returns>A new controller instance.</returns>
        public object Create(ControllerContext context)
        {
            Type controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();

            InstanceProducer? producer =
                this.controllerProducers.GetOrAdd(controllerType, this.GetControllerProducer);

            if (producer is null)
            {
                const string AddControllerActivationMethod =
                    nameof(SimpleInjectorAspNetCoreBuilderMvcCoreExtensions.AddControllerActivation);

                throw new InvalidOperationException(
                    $"For the {nameof(SimpleInjectorControllerActivator)} to function properly, it " +
                    $"requires all controllers to be registered explicitly, but a registration for " +
                    $"{controllerType.ToFriendlyName()} is missing. To ensure all controllers are regis" +
                    $"tered properly, call the {nameof(SimpleInjectorAspNetCoreBuilderMvcCoreExtensions)}" +
                    $".{AddControllerActivationMethod} extension method from within your Startup" +
                    $".ConfigureServices method—e.g. 'services.AddSimpleInjector(container, " +
                        $"options => options.AddAspNetCore().{AddControllerActivationMethod}());'." +
                    $"{Environment.NewLine}Full controller name: {controllerType.FullName}.");
            }

            return producer.GetInstance();
        }

        /// <summary>Releases the controller.</summary>
        /// <param name="context">The Microsoft.AspNet.Mvc.ActionContext for the executing action.</param>
        /// <param name="controller">The controller instance.</param>
        public void Release(ControllerContext context, object controller)
        {
        }

        // By searching through the current registrations, we ensure that the controller is not auto-registered, because
        // that might cause it to be resolved from ASP.NET Core, in case auto cross-wiring is enabled.
        private InstanceProducer? GetControllerProducer(Type controllerType) =>
            this.container.GetCurrentRegistrations().SingleOrDefault(r => r.ServiceType == controllerType);
    }
}