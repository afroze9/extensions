﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options.Validation;
using Microsoft.Extensions.Resilience.Internal;
using Microsoft.Extensions.Resilience.Options;
using Microsoft.Shared.Diagnostics;

namespace Microsoft.Extensions.Resilience;

public static partial class ResiliencePipelineBuilderExtensions
{
    /// <summary>
    /// Adds a retry policy with default options to a pipeline.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the action executed by the policies.</typeparam>
    /// <param name="builder">The policy pipeline builder.</param>
    /// <param name="policyName">The policy name.</param>
    /// <returns>Current instance.</returns>
    public static IResiliencePipelineBuilder<TResult> AddRetryPolicy<TResult>(
        this IResiliencePipelineBuilder<TResult> builder,
        string policyName)
    {
        _ = Throw.IfNull(builder);
        _ = Throw.IfNullOrEmpty(policyName);

        return builder.AddRetryPolicyInternal(policyName, null, null);
    }

    /// <summary>
    /// Adds a retry policy to a pipeline.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the action executed by the policies.</typeparam>
    /// <param name="builder">The policy pipeline builder.</param>
    /// <param name="policyName">The policy name.</param>
    /// <param name="configure">The action that configures the default policy options.</param>
    /// <returns>Current instance.</returns>
    public static IResiliencePipelineBuilder<TResult> AddRetryPolicy<TResult>(
        this IResiliencePipelineBuilder<TResult> builder,
        string policyName,
        Action<RetryPolicyOptions<TResult>> configure)
    {
        _ = Throw.IfNull(builder);
        _ = Throw.IfNullOrEmpty(policyName);
        _ = Throw.IfNull(configure);

        return builder.AddRetryPolicyInternal(policyName, null, configure);
    }

    /// <summary>
    /// Adds a retry policy to a pipeline.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the action executed by the policies.</typeparam>
    /// <param name="builder">The policy pipeline builder.</param>
    /// <param name="policyName">The policy name.</param>
    /// <param name="section">The configuration that the options will bind against.</param>
    /// <returns>Current instance.</returns>
    public static IResiliencePipelineBuilder<TResult> AddRetryPolicy<TResult>(
        this IResiliencePipelineBuilder<TResult> builder,
        string policyName,
        IConfigurationSection section)
    {
        _ = Throw.IfNull(builder);
        _ = Throw.IfNullOrEmpty(policyName);
        _ = Throw.IfNull(section);

        return builder.AddRetryPolicyInternal(policyName, section, null);
    }

    /// <summary>
    /// Adds a retry policy to a pipeline.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the action executed by the policies.</typeparam>
    /// <param name="builder">The policy pipeline builder.</param>
    /// <param name="policyName">The policy name.</param>
    /// <param name="section">The configuration that the options will bind against.</param>
    /// <param name="configure">The action that configures the policy options after <paramref name="section"/> is applied.</param>
    /// <returns>Current instance.</returns>
    /// <remarks>
    /// Keep in mind that the <paramref name="configure"/> delegate will override anything that was configured using <paramref name="section"/>.
    /// </remarks>
    public static IResiliencePipelineBuilder<TResult> AddRetryPolicy<TResult>(
        this IResiliencePipelineBuilder<TResult> builder,
        string policyName,
        IConfigurationSection section,
        Action<RetryPolicyOptions<TResult>> configure)
    {
        _ = Throw.IfNull(builder);
        _ = Throw.IfNullOrEmpty(policyName);
        _ = Throw.IfNull(section);
        _ = Throw.IfNull(configure);

        return builder.AddRetryPolicyInternal(policyName, section, configure);
    }

    private static IResiliencePipelineBuilder<TResult> AddRetryPolicyInternal<TResult>(
        this IResiliencePipelineBuilder<TResult> builder,
        string policyName,
        IConfigurationSection? section,
        Action<RetryPolicyOptions<TResult>>? configure)
    {
        _ = builder.Services.AddValidatedOptions<RetryPolicyOptions<TResult>>(SupportedPolicies.RetryPolicy.GetPolicyOptionsName(builder.PipelineName, policyName));

        return builder.AddPolicy<TResult, RetryPolicyOptions<TResult>, RetryPolicyOptionsValidator<TResult>>(
            SupportedPolicies.RetryPolicy,
            policyName,
            options => options.Configure(section, configure),
            (builder, options) => builder.AddRetryPolicy(policyName, options));
    }
}
