using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OrchardCore.Users.Models;
using Shouldly;
using System.Threading.Tasks;

namespace Lombiq.JsonEditor.Tests.UI.Extensions;

public static class TestCaseUITestContextExtensions
{
    public static async Task TestJsonEditorSampleBehaviorAsync(this UITestContext context)
    {
        var sampleContentItemId = "4xapn6ykttkk6wbbwgg1aaxqda";

        await context.EnableJsonEditorFeatureAsync();

        await context.ExecuteJsonEditorSampleRecipeDirectlyAsync();

        // Checking if the sample item is displayed correctly.
        await context.GoToContentItemByIdAsync(sampleContentItemId);

        context.Exists(By.XPath($"//div[contains(text(),'These are coming from the JSON field:')]"));
        context.Exists(By.XPath($"//li[contains(text(),'hello')]"));
        context.Exists(By.XPath($"//li[contains(text(),'world')]"));

        context.GoToContentItemEditorByIdAsync(sampleContentItemId);
    }
}
