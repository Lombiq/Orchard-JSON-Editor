namespace Lombiq.JsonEditor.Models;

public class JsonEditorTemplate
{
    /// <summary>
    /// Gets or sets the template name.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the template tooltip message.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the template technical name.
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Gets or sets the content to be inserted.
    /// </summary>
    public object Value { get; set; }
}
