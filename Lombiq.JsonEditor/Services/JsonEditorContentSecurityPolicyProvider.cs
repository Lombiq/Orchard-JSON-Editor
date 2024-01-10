using Lombiq.JsonEditor.Constants;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
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
}
