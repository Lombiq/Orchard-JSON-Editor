using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.ViewModels;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;

namespace Lombiq.JsonEditor.Drivers;

public class EditJsonActionsMenuContentDisplayDriver : ContentDisplayDriver
{
    public override IDisplayResult Display(ContentItem model, IUpdateModel updater) =>
        Initialize<ContentItemViewModel>("Content_EditJsonActions", a => a.ContentItem = model.ContentItem)
            .Location("ActionsMenu:after");
}
