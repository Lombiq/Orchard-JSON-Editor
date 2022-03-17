using Lombiq.JsonEditor.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata.Models;

namespace Lombiq.JsonEditor.ViewModels;

public class EditJsonFieldViewModel
{
    public string Value { get; set; }
    public JsonField Field { get; set; }
    public ContentPart Part { get; set; }
    public ContentPartFieldDefinition PartFieldDefinition { get; set; }
}
