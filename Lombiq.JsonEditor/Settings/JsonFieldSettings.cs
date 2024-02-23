using Lombiq.HelpfulLibraries.Common.Utilities;

namespace Lombiq.JsonEditor.Settings;

public class JsonFieldSettings : ICopier<JsonFieldSettings>
{
    public string JsonEditorOptions { get; set; }

    public void CopyTo(JsonFieldSettings target) => target.JsonEditorOptions = JsonEditorOptions;
}
