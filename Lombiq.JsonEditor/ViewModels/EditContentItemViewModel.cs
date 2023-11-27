using OrchardCore.ContentManagement;

namespace Lombiq.JsonEditor.ViewModels;

public record EditContentItemViewModel(
    string ContentItemId,
    string DisplayText,
    string Json)
{
    public EditContentItemViewModel(ContentItem contentItem, string json)
        : this(contentItem.ContentItemId, contentItem.DisplayText, json)
    {
    }
}
