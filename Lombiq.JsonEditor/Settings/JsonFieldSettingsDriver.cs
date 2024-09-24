using Lombiq.JsonEditor.Fields;
using Lombiq.JsonEditor.Models;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Metadata.Builders;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Lombiq.JsonEditor.Settings;

public sealed class JsonFieldSettingsDriver : ContentPartFieldDefinitionDisplayDriver<JsonField>
{
    private readonly IStringLocalizer T;

    public JsonFieldSettingsDriver(IStringLocalizer<JsonFieldSettingsDriver> stringLocalizer) => T = stringLocalizer;

    public override IDisplayResult Edit(ContentPartFieldDefinition model, BuildEditorContext context) =>
        Initialize<JsonFieldSettings>($"{nameof(JsonFieldSettings)}_Edit", model.CopySettingsTo)
            .PlaceInContent();

    public override async Task<IDisplayResult> UpdateAsync(
        ContentPartFieldDefinition model,
        UpdatePartFieldEditorContext context)
    {
        var settings = await context.CreateModelAsync<JsonFieldSettings>(Prefix);

        try
        {
            JsonNode.Parse(settings.JsonEditorOptions).ToObject<JsonEditorOptions>();
            context.Builder.WithSettings(settings);
        }
        catch (JsonException)
        {
            context.Updater.ModelState.AddModelError(
                Prefix,
                T["The input isn't a valid {0} object.", nameof(JsonEditorOptions)]);
        }

        return await EditAsync(model, context);
    }
}
