using Lombiq.JsonEditor.Fields;
using Lombiq.JsonEditor.Models;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;

namespace Lombiq.JsonEditor.Settings
{
    public class JsonFieldSettingsDriver : ContentPartFieldDefinitionDisplayDriver<JsonField>
    {
        private readonly IStringLocalizer T;

        public JsonFieldSettingsDriver(IStringLocalizer<JsonFieldSettingsDriver> stringLocalizer) => T = stringLocalizer;

        public override IDisplayResult Edit(ContentPartFieldDefinition model) =>
            Initialize<JsonFieldSettings>($"{nameof(JsonFieldSettings)}_Edit", model.PopulateSettings)
                .Location("Content");

        public override async Task<IDisplayResult> UpdateAsync(
            ContentPartFieldDefinition model,
            UpdatePartFieldEditorContext context)
        {
            var settings = new JsonFieldSettings();
            await context.Updater.TryUpdateModelAsync(settings, Prefix);

            try
            {
                JsonConvert.DeserializeObject<JsonEditorOptions>(settings.JsonEditorOptions);
                context.Builder.WithSettings(settings);
            }
            catch (JsonException)
            {
                context.Updater.ModelState.AddModelError(
                    Prefix,
                    T["The input isn't a valid {0} object.", nameof(JsonEditorOptions)]);
            }

            return Edit(model);
        }
    }
}
