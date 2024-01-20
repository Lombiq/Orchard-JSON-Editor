using Lombiq.JsonEditor.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using OrchardCore.DisplayManagement;
using System.Threading.Tasks;

namespace Lombiq.JsonEditor.TagHelpers;

[HtmlTargetElement("json-editor")]
public class JsonEditorTagHelper(IDisplayHelper displayHelper, IShapeFactory factory) : TagHelper
{
    [HtmlAttributeName("content")]
    public object Content { get; set; }

    [HtmlAttributeName("json")]
    public string SerializedJson { get; set; }

    [HtmlAttributeName("options")]
    public JsonEditorOptions Options { get; set; } = new();

    [HtmlAttributeName("name")]
    public string InputName { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var shape = await factory.New.JsonEditor(
            Content: Content,
            SerializedJson: SerializedJson,
            Options: Options,
            InputName: InputName);
        var content = (IHtmlContent)await displayHelper.ShapeExecuteAsync(shape);

        output.TagName = null;
        output.TagMode = TagMode.StartTagAndEndTag;
        output.PostContent.SetHtmlContent(content);
    }
}
