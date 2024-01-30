using Lombiq.JsonEditor.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using OrchardCore.DisplayManagement;
using System.Threading.Tasks;

namespace Lombiq.JsonEditor.TagHelpers;

[HtmlTargetElement("json-editor")]
public class JsonEditorTagHelper : TagHelper
{
    private readonly IDisplayHelper _displayHelper;
    private readonly IShapeFactory _shapeFactory;

    [HtmlAttributeName("content")]
    public object Content { get; set; }

    [HtmlAttributeName("json")]
    public string SerializedJson { get; set; }

    [HtmlAttributeName("options")]
    public JsonEditorOptions Options { get; set; } = new();

    [HtmlAttributeName("name")]
    public string InputName { get; set; }

    public JsonEditorTagHelper(IDisplayHelper displayHelper, IShapeFactory factory)
    {
        _displayHelper = displayHelper;
        _shapeFactory = factory;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var shape = await _shapeFactory.New.JsonEditor(
            Content: Content,
            SerializedJson: SerializedJson,
            Options: Options,
            InputName: InputName);
        var content = (IHtmlContent)await _displayHelper.ShapeExecuteAsync(shape);

        output.TagName = null;
        output.TagMode = TagMode.StartTagAndEndTag;
        output.PostContent.SetHtmlContent(content);
    }
}
