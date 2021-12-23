using Lombiq.JsonEditor.Fields;
using Lombiq.JsonEditor.ViewModels;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;

namespace Lombiq.JsonEditor.Drivers
{
    public class JsonFieldDisplayDriver : ContentFieldDisplayDriver<JsonField>
    {
        private readonly IStringLocalizer T;

        public JsonFieldDisplayDriver(IStringLocalizer<JsonFieldDisplayDriver> stringLocalizer) => T = stringLocalizer;

        public override IDisplayResult Display(JsonField field, BuildFieldDisplayContext fieldDisplayContext) =>
            Initialize<DisplayJsonFieldViewModel>(GetDisplayShapeType(fieldDisplayContext), model =>
                {
                    model.Field = field;
                    model.Part = fieldDisplayContext.ContentPart;
                    model.PartFieldDefinition = fieldDisplayContext.PartFieldDefinition;
                })
                .Location("Detail", "Content")
                .Location("Summary", "Content");

        public override IDisplayResult Edit(JsonField field, BuildFieldEditorContext context) =>
            Initialize<EditJsonFieldViewModel>(GetEditorShapeType(context), model =>
            {
                model.Value = field.Value;
                model.Field = field;
                model.Part = context.ContentPart;
                model.PartFieldDefinition = context.PartFieldDefinition;
            });

        public override async Task<IDisplayResult> UpdateAsync(JsonField field, IUpdateModel updater, UpdateFieldEditorContext context)
        {
            var model = new EditJsonFieldViewModel();

            if (!await updater.TryUpdateModelAsync(model, Prefix)) return await EditAsync(field, context);

            if (!TryParse(model.Value))
            {
                updater.ModelState.AddModelError(Prefix, T["The input isn't a valid JSON entity."]);
            }
            else
            {
                field.Value = model.Value;
            }

            return await EditAsync(field, context);
        }

        private static bool TryParse(string value)
        {
            try
            {
                JObject.Parse(value);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
