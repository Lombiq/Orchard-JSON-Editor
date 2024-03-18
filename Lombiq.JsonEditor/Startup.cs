using Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;
using Lombiq.JsonEditor.Constants;
using Lombiq.JsonEditor.Drivers;
using Lombiq.JsonEditor.Fields;
using Lombiq.JsonEditor.Services;
using Lombiq.JsonEditor.Settings;
using Lombiq.JsonEditor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Contents.Controllers;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.Modules;
using OrchardCore.ResourceManagement;

namespace Lombiq.JsonEditor;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<ResourceManagementOptions>, ResourceManagementOptionsConfiguration>();
        services.AddTagHelpers<JsonEditorTagHelper>();

        services.AddContentField<JsonField>().UseDisplayDriver<JsonFieldDisplayDriver>();
        services.AddScoped<IContentPartFieldDefinitionDisplayDriver, JsonFieldSettingsDriver>();
    }
}

[Feature(FeatureIds.ContentEditor)]
public class ContentEditorStartup : StartupBase
{
    private readonly AdminOptions _adminOptions;

    public ContentEditorStartup(IOptions<AdminOptions> adminOptions) => _adminOptions = adminOptions.Value;

    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IContentDisplayDriver, EditJsonActionsMenuContentDisplayDriver>();
        services.AddOrchardServices();
        services.AddScoped<ApiController>();
        services.AddContentSecurityPolicyProvider<JsonEditorContentSecurityPolicyProvider>();
    }
}
