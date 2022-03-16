using Lombiq.JsonEditor.Drivers;
using Lombiq.JsonEditor.Fields;
using Lombiq.JsonEditor.Settings;
using Lombiq.JsonEditor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
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
