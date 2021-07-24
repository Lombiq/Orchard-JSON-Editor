using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using static Lombiq.JsonEditor.Constants.ResourceNames;

namespace Lombiq.JsonEditor
{
    public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
    {
        private const string Vendors = "~/Lombiq.JsonEditor/vendors/";

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
        }

        public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
    }
}
