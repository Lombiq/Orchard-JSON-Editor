using Lombiq.JsonEditor.Constants;
using Lombiq.JsonEditor.Controllers;
using Lombiq.JsonEditor.Drivers;
using Lombiq.JsonEditor.Fields;
using Lombiq.JsonEditor.Settings;
using Lombiq.JsonEditor.TagHelpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.Modules;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.ResourceManagement;
using System;

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

public class ContentEditorStartup : StartupBase
{
    private readonly AdminOptions _adminOptions;

    public ContentEditorStartup(IOptions<AdminOptions> adminOptions) => _adminOptions = adminOptions.Value;

    public override void ConfigureServices(IServiceCollection services) =>
        services.AddScoped<IContentDisplayDriver, EditJsonActionsMenuContentDisplayDriver>();

    public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) =>
        routes.MapAreaControllerRoute(
            name: "EditContentItem",
            areaName: FeatureIds.Area,
            pattern: _adminOptions.AdminUrlPrefix + "/Contents/ContentItems/{contentItemId}/Edit/Json",
            defaults: new { controller = typeof(AdminController).ControllerName(), action = nameof(AdminController.Edit) });
}
