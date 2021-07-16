using OrchardCore.Modules.Manifest;
using static Lombiq.JsonEditor.Constants.FeatureIds;

[assembly: Module(
    Name = "Lombiq JSON Editor",
    Author = "Lombiq Technologies",
    Version = "1.0",
    Description = "Module for displaying a JSON Editor like on jsoneditoronline.org.",
    Website = "https://github.com/Lombiq/"
)]

[assembly: Feature(
    Id = Default,
    Name = "Lombiq JSON Editor",
    Category = "Content",
    Description = "Module for displaying a JSON Editor like on jsoneditoronline.org.",
    Dependencies = new[]
    {
        "OrchardCore.Contents",
        "OrchardCore.ResourceManagement",
    }
)]
