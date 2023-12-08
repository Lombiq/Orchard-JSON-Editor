using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata.Models;

namespace Lombiq.JsonEditor.ViewModels;

public record EditContentItemViewModel(
    ContentItem ContentItem,
    ContentTypeDefinition ContentTypeDefinition,
    string Json);
