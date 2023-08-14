using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using static Lombiq.JsonEditor.Constants.ResourceNames;

namespace Lombiq.JsonEditor;

public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
{
    private const string Module = "~/Lombiq.JsonEditor/";
    private const string Vendors = Module + "vendors/";

    private static readonly ResourceManifest _manifest = new();

    static ResourceManagementOptionsConfiguration()
    {
        _manifest
            .DefineScript(Library)
            .SetUrl(Vendors + "jsoneditor/jsoneditor.min.js", Vendors + "jsoneditor/jsoneditor.js")
            .SetVersion("9.5.0");

        _manifest
            .DefineStyle(Library)
            .SetUrl(Vendors + "jsoneditor/jsoneditor.min.css", Vendors + "jsoneditor/jsoneditor.css")
            .SetVersion("9.5.0");

        _manifest
            .DefineStyle(Style)
            .SetUrl(Module + "css/json-editor.min.css", Module + "css/json-editor.css")
            .SetVersion("1.0.0");
    }

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}
