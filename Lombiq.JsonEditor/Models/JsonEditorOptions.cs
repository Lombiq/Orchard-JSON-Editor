using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Lombiq.JsonEditor.Models;

public class JsonEditorOptions
{
    // Documentation is from: https://github.com/josdejong/jsoneditor/blob/master/docs/api.md#configuration-options

    /// <summary>
    /// Gets or sets a value indicating whether Unicode characters are escaped and displayed as their hexadecimal code
    /// (like \u260E) instead of of the character itself (like ☎).
    /// </summary>
    public bool EscapeUnicode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether object keys in 'tree', 'view' or 'form' mode list be listed
    /// alphabetically instead by their insertion order. Sorting is performed using a natural sort algorithm, which
    /// makes it easier to see objects that have string numbers as keys.
    /// </summary>
    public bool SortObjectKeys { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether edit history is enabled. Adds a button Undo and Redo to the menu of the
    /// JSONEditor. Only applicable when mode is 'tree', 'form', or 'preview'.
    /// </summary>
    public bool History { get; set; } = true;

    [JsonPropertyName("mode")]
    [JsonInclude]
    private string ModeString { get; set; } = "tree";

    /// <summary>
    /// Gets or sets the editor mode. In 'view' mode, the data and data structure is read-only. In 'form' mode, only the
    /// value can be changed, the data structure is read-only. Mode 'code' requires the Ace editor to be loaded on the
    /// page. Mode 'text' shows the data as plain text. The 'preview' mode can handle large JSON documents up to 500
    /// MiB. It shows a preview of the data, and allows to transform, sort, filter, format, or compact the data.
    /// </summary>
    [JsonIgnore]
    public JsonEditorMode Mode
    {
        get => Enum.Parse<JsonEditorMode>(ModeString, ignoreCase: true);
        set => ModeString = GetModeString(value);
    }

    public IEnumerable<string> Modes { get; set; } = ["tree", "view", "form", "code", "text", "preview"];

    /// <summary>
    /// Gets or sets the JSON Schema to validate against. A JSON schema describes the structure that a JSON object must
    /// have, like required properties or the type that a value must have. See http://json-schema.org/ for more
    /// information.
    /// </summary>
    public object Schema { get; set; }

    /// <summary>
    /// Gets or sets schemas that are referenced using the $ref property from the JSON schema that are set in the schema
    /// option, the object structure in the form of {reference_key: schemaObject}.
    /// </summary>
    public object SchemaRefs { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a search box in the upper right corner of the JSONEditor is enabled.
    /// Only applicable when mode is 'tree', 'view', or 'form'.
    /// </summary>
    public bool Search { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of spaces to indent.
    /// </summary>
    public int Indentation { get; set; } = 2;

    /// <summary>
    /// Gets or sets the array of templates that will appear in the context menu, Each template is a json object
    /// pre-created that can be added as a object value to any node in your document.
    /// </summary>
    public IEnumerable<JsonEditorTemplate> Templates { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether sorting of arrays and object properties is enabled. Only applicable for
    /// mode 'tree'.
    /// </summary>
    public bool EnableSort { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether filtering, sorting, and transforming JSON using a JMESPath query is
    /// enabled. Only applicable for mode 'tree'.
    /// </summary>
    public bool EnableTransform { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of children allowed for a given node before the "show more / show all" message appears
    /// (in 'tree', 'view', or 'form' modes).
    /// </summary>
    public int MaxVisibleChilds { get; set; } = 100;

    /// <summary>
    /// Gets or sets a value indicating whether main menu bar should be shown. Contains format, sort, transform, search
    /// etc. functionality.
    /// </summary>
    public bool MainMenuBar { get; set; } = true;

    public JsonEditorOptions SetModes(params JsonEditorMode[] modes)
    {
        Modes = modes.Select(GetModeString);
        return this;
    }

    private static string GetModeString(JsonEditorMode mode) =>
        // The mode names are all lower case strings.
#pragma warning disable CA1308 // Normalize strings to uppercase.
        mode.ToString().ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase.

    public static JsonEditorOptions GetSample(IHtmlLocalizer localizer) =>
        new()
        {
            Templates =
            [
                new JsonEditorTemplate
                {
                    Field = "aTechnicalNameThatHasToBeUnique",
                    Text = localizer["The template's name in the dropdown."].Value,
                    Title =
                        localizer["The tooltip text. These templates are in the context menu's Append and Insert sections while in Tree mode."]
                        .Value,
                    Value = new { YourObject = "goes here" },
                },
            ],
        };
}
