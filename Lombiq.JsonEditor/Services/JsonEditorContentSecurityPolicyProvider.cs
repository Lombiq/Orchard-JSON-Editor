﻿using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Lombiq.JsonEditor.Constants;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;

namespace Lombiq.JsonEditor.Services;

/// <summary>
/// Permits <c>blob: data:</c> access to the <see cref="WorkerSrc"/> on pages that include the <see
/// cref="ResourceNames.Library"/> script.
/// </summary>
public class JsonEditorContentSecurityPolicyProvider : ResourceManagerContentSecurityPolicyProvider
{
    protected override string ResourceType => "script";
    protected override string ResourceName => ResourceNames.Library;
    protected override IReadOnlyCollection<string> DirectiveNameChain { get; } = new[] { WorkerSrc, ScriptSrc };
    protected override string DirectiveValue => $"{Blob} {Data}";

    protected override ValueTask ThenUpdateAsync(
        IDictionary<string, string> securityPolicies,
        HttpContext context,
        bool resourceExists)
    {
        // Fixes "[Severe] blob:https://localhost:9391/6a0aeee0-8ec7-449c-86b4-7668d046d24c 0 Refused to load the
        // script 'data:application/javascript;base64,...' because it violates the following Content Security Policy
        // directive" error.
        if (resourceExists)
        {
            securityPolicies[ScriptSrc] = IContentSecurityPolicyProvider
                .GetDirective(securityPolicies, ScriptSrc)
                .MergeWordSets(DirectiveValue);
        }

        return ValueTask.CompletedTask;
    }
}
