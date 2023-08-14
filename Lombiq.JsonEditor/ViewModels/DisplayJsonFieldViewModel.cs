using Lombiq.JsonEditor.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata.Models;

namespace Lombiq.JsonEditor.ViewModels;

public class DisplayJsonFieldViewModel
{
    public string Value => Field.Value;
    public JsonField Field { get; set; }
    public ContentPart Part { get; set; }
    public ContentPartFieldDefinition PartFieldDefinition { get; set; }
}
