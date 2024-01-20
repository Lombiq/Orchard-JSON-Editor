using AngleSharp.Common;
using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;
using Lombiq.JsonEditor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.Contents;
using OrchardCore.Contents.Controllers;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.DisplayManagement.Title;
using OrchardCore.Title.ViewModels;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lombiq.JsonEditor.Controllers;

public class AdminController(
    IContentDefinitionManager contentDefinitionManager,
    ILayoutAccessor layoutAccessor,
    INotifier notifier,
    IPageTitleBuilder pageTitleBuilder,
    IShapeFactory shapeFactory,
    IOrchardServices<AdminController> services,
    Lazy<ApiController> contentApiControllerLazy) : Controller
{
    private readonly IAuthorizationService _authorizationService = services.AuthorizationService.Value;
    private readonly IContentManager _contentManager = services.ContentManager.Value;
    private readonly IStringLocalizer<AdminController> T = services.StringLocalizer.Value;
    private readonly IHtmlLocalizer<AdminController> H = services.HtmlLocalizer.Value;

    public async Task<IActionResult> Edit(string contentItemId)
    {
        if (string.IsNullOrWhiteSpace(contentItemId) ||
            await _contentManager.GetAsync(contentItemId, VersionOptions.Latest) is not { } contentItem ||
            !await CanEditAsync(contentItem))
        {
            return NotFound();
        }

        var title = T["Edit {0} as JSON", GetName(contentItem)].Value;
        pageTitleBuilder.AddSegment(new StringHtmlContent(title));
        var titleShape = await shapeFactory.CreateAsync<TitlePartViewModel>("TitlePart", model =>
        {
            model.Title = title;
            model.ContentItem = contentItem;
        });
        await layoutAccessor.AddShapeToZoneAsync("Title", titleShape);

        var definition = await contentDefinitionManager.GetTypeDefinitionAsync(contentItem.ContentType);
        return View(new EditContentItemViewModel(contentItem, definition, JsonConvert.SerializeObject(contentItem)));
    }

    [ValidateAntiForgeryToken]
    [HttpPost, ActionName(nameof(Edit))]
    public async Task<IActionResult> EditPost(
        string contentItemId,
        string json,
        string returnUrl,
        [Bind(Prefix = "submit.Publish")] string submitPublish,
        [Bind(Prefix = "submit.Save")] string submitSave)
    {
        if (string.IsNullOrWhiteSpace(contentItemId) ||
            string.IsNullOrWhiteSpace(json) ||
            JsonConvert.DeserializeObject<ContentItem>(json) is not { } contentItem)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(contentItem.ContentItemId)) contentItem.ContentItemId = contentItemId;
        contentItem = await _contentManager.LoadAsync(contentItem);

        if (!await CanEditAsync(contentItem))
        {
            return NotFound();
        }

        switch (await UpdateContentAsync(contentItem, submitSave != null))
        {
            case BadRequestObjectResult { Value: ValidationProblemDetails details }
                when !string.IsNullOrWhiteSpace(details.Detail):
                await notifier.ErrorAsync(new LocalizedHtmlString(details.Detail, details.Detail));
                return await Edit(contentItem.ContentItemId);
            case OkObjectResult:
                await notifier.SuccessAsync(H["Content item {0} has been successfully saved.", GetName(contentItem)]);
                break;
            default:
                await notifier.ErrorAsync(H["The submission has failed, please try again."]);
                return await Edit(contentItem.ContentItemId);
        }

        if (!string.IsNullOrEmpty(returnUrl) &&
            !(IsContinue(submitSave) || IsContinue(submitPublish)) &&
            Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Edit), new { contentItemId, returnUrl });
    }

    private Task<bool> CanEditAsync(ContentItem contentItem) =>
        _authorizationService.AuthorizeAsync(User, CommonPermissions.EditContent, contentItem);

    private async Task<IActionResult> UpdateContentAsync(ContentItem contentItem, bool isDraft)
    {
        // The Content API Controller requires the AccessContentApi permission. As this isn't an external API request it
        // doesn't make sense to require this permission. So we create a temporary claims principal and explicitly grant
        // the permission.
        var currentUser = User;
        HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(User.Claims.Concat(Permissions.AccessContentApi)));

        try
        {
            // Here the API controller is called directly. The behavior is the same as if we sent a POST request using an
            // HTTP client (except the permission bypass above), but it's faster and more resource-efficient.
            var contentApiController = contentApiControllerLazy.Value;
            contentApiController.ControllerContext.HttpContext = HttpContext;
            return await contentApiController.Post(contentItem, isDraft);
        }
        finally
        {
            // Ensure that the original claims principal is restored, just in case.
            HttpContext.User = currentUser;
        }
    }

    private static bool IsContinue(string submitString) =>
        submitString?.EndsWithOrdinalIgnoreCase("AndContinue") == true;

    private static string GetName(ContentItem contentItem) =>
        string.IsNullOrWhiteSpace(contentItem.DisplayText)
            ? contentItem.ContentType
            : $"\"{contentItem.DisplayText}\"";
}
