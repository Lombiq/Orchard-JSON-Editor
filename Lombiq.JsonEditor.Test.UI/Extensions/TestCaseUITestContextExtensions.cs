using Atata;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using System.Threading.Tasks;

namespace Lombiq.JsonEditor.Tests.UI.Extensions;

public static class TestCaseUITestContextExtensions
{
    private const string SampleContentItemId = "4xapn6ykttkk6wbbwgg1aaxqda";
    private const string HelloValue = "hello";
    private const string WorldValue = "world";

    private static readonly By ObjectByXPath = By.XPath($"//div[@class='jsoneditor-readonly' and contains(text(),'object')]");
    private static readonly By ObjectCountByXPath = By.XPath($"//div[@class='jsoneditor-value jsoneditor-object' and contains(text(),'{{1}}')]");
    private static readonly By ArrayByXPath = By.XPath($"//div[@class='jsoneditor-field' and contains(text(),'printThese')]");
    private static readonly By ArrayCountByXPath = By.XPath($"//div[@class='jsoneditor-value jsoneditor-array' and contains(text(),'[2]')]");

    public static async Task TestJsonEditorSampleBehaviorAsync(this UITestContext context)
    {
        await context.EnableJsonEditorFeatureAsync();

        await context.ExecuteJsonEditorSampleRecipeDirectlyAsync();

        // Checking if the sample item is displayed correctly.
        await context.GoToContentItemByIdAsync(SampleContentItemId);

        context.Exists(By.XPath($"//div[contains(text(),'These are coming from the JSON field:')]"));
        context.Exists(By.XPath($"//li[contains(text(),'{HelloValue}')]"));
        context.Exists(By.XPath($"//li[contains(text(),'{WorldValue}')]"));

        await context.SignInDirectlyAsync();
        await context.GoToContentItemEditorByIdAsync(SampleContentItemId);

        // Checking if the sample item is displayed correctly in the editor in tree editor mode.
        await context.TestTreeTypeModeAsync();

        // Checking if the sample item is displayed correctly in the editor in tree view mode.
        await context.SwitchToModeAsync("Switch to tree view");
        await context.TestTreeTypeModeAsync();

        // Checking if the sample item is displayed correctly in the editor in form editor mode.
        await context.SwitchToModeAsync("Switch to form editor");
        await context.TestTreeTypeModeAsync();
    }

    private static void CheckArrayValueInTreeMode(this UITestContext context, string arrayValue, bool missing = false)
    {
        var arrayValueByXPath =
            By.XPath($"//div[@class='jsoneditor-value jsoneditor-string' and contains(text(),'{arrayValue}')]");

        if (!missing)
        {
            context.Exists(arrayValueByXPath);
        }
        else
        {
            context.Missing(arrayValueByXPath);
        }
    }

    private static Task ClickOnExpandAllAsync(this UITestContext context) =>
        context.ClickReliablyOnAsync(By.XPath($"//button[@class='jsoneditor-expand-all']"));

    private static Task ClickOnCollapseAllAsync(this UITestContext context) =>
        context.ClickReliablyOnAsync(By.XPath($"//button[@class='jsoneditor-collapse-all']"));

    private static async Task SwitchToModeAsync(this UITestContext context, string editorTitle)
    {
        await context.ClickReliablyOnAsync(By.XPath($"//button[@class='jsoneditor-modes jsoneditor-separator']"));
        await context.ClickReliablyOnAsync(By.XPath($"//button[@class='jsoneditor-type-modes' and @title ='{editorTitle}']"));
    }

    private static async Task TestTreeTypeModeAsync(this UITestContext context)
    {
        await context.ClickOnExpandAllAsync();

        // Checking object {1}.
        context.Exists(ObjectByXPath);
        context.Exists(ObjectCountByXPath);

        // Checking printThese [2].
        context.Exists(ArrayByXPath);
        context.Exists(ArrayCountByXPath);

        // Checking "hello" and "word".
        context.CheckArrayValueInTreeMode(HelloValue);
        context.CheckArrayValueInTreeMode(WorldValue);

        // Collapse button should hide things.
        await context.ClickOnCollapseAllAsync();

        context.Exists(ObjectByXPath);
        context.Exists(ObjectCountByXPath);

        context.Missing(ArrayByXPath);
        context.Missing(ArrayCountByXPath);

        context.CheckArrayValueInTreeMode(HelloValue, missing: true);
        context.CheckArrayValueInTreeMode(WorldValue, missing: true);
    }

}
