using Lombiq.Privacy.Tests.UI.Constants;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using OrchardCore.Users.Models;
using Shouldly;
using System.Threading.Tasks;

namespace Lombiq.Privacy.Tests.UI.Extensions;

public static class TestCaseUITestContextExtensions
{
    public static async Task TestPrivacySampleBehaviorAsync(this UITestContext context)
    {
        await context.ExecutePrivacySampleRecipeDirectlyAsync();

        await context.TestFormConsentCheckboxContentAsync();

        await context.GoToRelativeUrlAsync(AbsolutePaths.CompetitorRegistration);

        // Here we fill up the competitor form.
        await context.FillInWithRetriesAsync(By.Id("full-name"), TestCompetitor.FullName);
        await context.FillInWithRetriesAsync(By.Id("age"), TestCompetitor.Age);
        await context.FillInWithRetriesAsync(By.Id("e-mail"), TestCompetitor.Email);
        await context.ClickReliablyOnSubmitAsync();

        // Consent checkbox left unchecked, so after clicking on submit navigation should not happens.
        context.GetCurrentUri().AbsolutePath.ShouldBe(AbsolutePaths.CompetitorRegistration);

        // Now we set consent checkbox to checked.
        await context.SetCheckboxValueAsync(By.Id("PrivacyConsentCheckboxPart_ConsentCheckbox"), isChecked: true);
        await context.ClickReliablyOnSubmitAsync();

        // After submit, it navigates to the new content items view.
        context.GetCurrentUri().AbsolutePath
            .ShouldNotBeOneOf(AbsolutePaths.ErrorPage, AbsolutePaths.CompetitorRegistration);
    }

    public static async Task TestFormConsentCheckboxContentAsync(this UITestContext context)
    {
        await context.GoToRelativeUrlAsync(AbsolutePaths.CompetitorRegistration);

        context.VerifyElementTexts(
            By.CssSelector(ElementSelectors.PrivacyConsentCheckboxLabelCss),
            ExpectedContents.FormConsentCheckboxContent);
    }

    public static async Task TestConsentBannerWithThemeAsync(this UITestContext context, string theme)
    {
        await context.SelectThemeAsync(theme);
        await context.SignInDirectlyAsync();
        await context.TestConsentBannerAsync();
    }

    public static async Task TestConsentBannerAsync(this UITestContext context)
    {
        await context.EnablePrivacyConsentBannerFeatureAsync();
        await context.GoToHomePageAsync(onlyIfNotAlreadyThere: false);
        await context.TestConsentBannerContentAsync();
        await context.TestConsentBannerAcceptButtonAsync();
    }

    public static async Task TestConsentBannerContentAsync(this UITestContext context)
    {
        await context.GoToHomePageAsync();

        context.VerifyElementTexts(
            By.CssSelector("#privacy-consent-banner > .modal-content > .modal-body"),
            ExpectedContents.ConsentBannerContent);
    }

    public static async Task TestConsentBannerAcceptButtonAsync(this UITestContext context)
    {
        await context.GoToHomePageAsync();

        var privacyConsentAcceptButtonBy = By.Id(ElementSelectors.PrivacyConsentAcceptButtonId);

        void AssertPrivacyConsentAcceptButtonMissing()
        {
            try
            {
                context.Missing(privacyConsentAcceptButtonBy);
            }
            catch (StaleElementReferenceException)
            {
                // If the banner disappears when we're in the middle of Missing() then unlucky timing can cause it to
                // get properties of the now vanished element, causing a StaleElementReferenceException. This is not a
                // problem, so we may continue.
            }
        }

        await context.ClickReliablyOnAsync(privacyConsentAcceptButtonBy);
        AssertPrivacyConsentAcceptButtonMissing();

        // Verify persistence.
        context.Refresh();
        AssertPrivacyConsentAcceptButtonMissing();
    }

    public static async Task TestRegistrationConsentCheckboxAsync(this UITestContext context)
    {
        await context.EnablePrivacyConsentBannerFeatureAsync();
        await context.EnablePrivacyRegistrationConsentFeatureAsync();
        await context.SetUserRegistrationTypeAsync(UserRegistrationType.AllowRegistration);

        await context.TestRegistrationConsentCheckboxContentAsync();

        // Go to registration and create a new user.
        await context.GoToRegistrationPageAsync();
        await context.FillInWithRetriesAsync(By.Id("UserName"), TestUser.Name);
        await context.FillInWithRetriesAsync(By.Id("Email"), TestUser.Email);
        await context.FillInWithRetriesAsync(By.Id("Password"), TestUser.Password);
        await context.FillInWithRetriesAsync(By.Id("ConfirmPassword"), TestUser.Password);
        await context.SetCheckboxValueAsync(By.Id("RegistrationCheckbox"), isChecked: true);
        await context.ClickReliablyOnSubmitAsync();

        // Login with the created user.
        await context.SignInDirectlyAsync(TestUser.Name);
        (await context.GetCurrentUserNameAsync()).ShouldBe(TestUser.Name);

        // Check that, the consent banner doesn't come up after login.
        await context.GoToHomePageAsync();
        context.Missing(By.Id(ElementSelectors.PrivacyConsentAcceptButtonId));
    }

    public static async Task TestRegistrationConsentCheckboxContentAsync(this UITestContext context)
    {
        await context.GoToRegistrationPageAsync();

        context.VerifyElementTexts(
            By.CssSelector(ElementSelectors.PrivacyConsentCheckboxLabelCss),
            ExpectedContents.RegistrationConsentCheckboxContent);
    }
}
