using OrchardCore.ResourceManagement;
using static Lombiq.JsonEditor.Constants.ResourceNames;

namespace Lombiq.JsonEditor
{
    public class ResourceManifest : IResourceManifestProvider
    {
        private const string Vendors = "~/Lombiq.JsonEditor/vendors/";

        public void BuildManifests(IResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest
                .DefineScript(Library)
                .SetUrl(Vendors + "jsoneditor/jsoneditor.min.js", Vendors + "jsoneditor/jsoneditor.js")
                .SetVersion("9.5.0");

            manifest
                .DefineStyle(Library)
                .SetUrl(Vendors + "jsoneditor/jsoneditor.min.css", Vendors + "jsoneditor/jsoneditor.css")
                .SetVersion("9.5.0");
        }
    }
}
