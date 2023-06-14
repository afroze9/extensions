﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Http.Resilience.Internal.Routing;

internal sealed class DefaultRoutingStrategyFactory<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TRoutingStrategy> : IRequestRoutingStrategyFactory
    where TRoutingStrategy : IRequestRoutingStrategy
{
    private readonly string _clientId;
    private readonly IServiceProvider _serviceProvider;
    private readonly ObjectFactory _factory;

    public DefaultRoutingStrategyFactory(string clientId, IServiceProvider serviceProvider)
    {
        _clientId = clientId;
        _serviceProvider = serviceProvider;
        _factory = ActivatorUtilities.CreateFactory(typeof(TRoutingStrategy), new[] { typeof(string) });
    }

    public IRequestRoutingStrategy CreateRoutingStrategy() => (IRequestRoutingStrategy)_factory(_serviceProvider, new object[] { _clientId });
}
