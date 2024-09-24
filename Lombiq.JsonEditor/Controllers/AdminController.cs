using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;
using Lombiq.HelpfulLibraries.OrchardCore.Validation;
using Lombiq.JsonEditor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.Contents;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.DisplayManagement.Title;
using OrchardCore.Title.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Settings;
using System.Threading.Tasks;

namespace Lombiq.JsonEditor.Controllers;

public sealed class AdminController : Controller
{
    private static readonly JsonMergeSettings _updateJsonMergeSettings = new()
    {
        MergeArrayHandling = MergeArrayHandling.Replace,
    };

    private readonly IAuthorizationService _authorizationService;
    private readonly IContentManager _contentManager;
    private readonly IContentDefinitionManager _contentDefinitionManager;
    private readonly ILayoutAccessor _layoutAccessor;
    private readonly INotifier _notifier;
    private readonly IPageTitleBuilder _pageTitleBuilder;
    private readonly IShapeFactory _shapeFactory;
    private readonly IStringLocalizer<AdminController> T;
    private readonly IHtmlLocalizer<AdminController> H;

    public AdminController(
        IContentDefinitionManager contentDefinitionManager,
        ILayoutAccessor layoutAccessor,
        INotifier notifier,
        IPageTitleBuilder pageTitleBuilder,
        IShapeFactory shapeFactory,
        IOrchardServices<AdminController> services)
    {
        _authorizationService = services.AuthorizationService.Value;
        _contentManager = services.ContentManager.Value;
        _contentDefinitionManager = contentDefinitionManager;
        _layoutAccessor = layoutAccessor;
        _notifier = notifier;
        _pageTitleBuilder = pageTitleBuilder;
        _shapeFactory = shapeFactory;
        T = services.StringLocalizer.Value;
        H = services.HtmlLocalizer.Value;
    }

    [Admin("Contents/ContentItems/{contentItemId}/Edit/Json")]
    public async Task<IActionResult> Edit(string contentItemId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(contentItemId) ||
            await _contentManager.GetAsync(contentItemId, VersionOptions.Latest) is not { } contentItem ||
            !await CanEditAsync(contentItem))
        {
            return NotFound();
        }

        var title = T["Edit {0} as JSON", GetName(contentItem)].Value;
        _pageTitleBuilder.AddSegment(new StringHtmlContent(title));
        var titleShape = await _shapeFactory.CreateAsync<TitlePartViewModel>("TitlePart", model =>
        {
            model.Title = title;
            model.ContentItem = contentItem;
        });
        await _layoutAccessor.AddShapeToZoneAsync("Title", titleShape);

        var definition = await _contentDefinitionManager.GetTypeDefinitionAsync(contentItem.ContentType);
        return View(new EditContentItemViewModel(contentItem, definition, JsonSerializer.Serialize(contentItem)));
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
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(contentItemId) ||
            string.IsNullOrWhiteSpace(json) ||
            JsonSerializer.Deserialize<ContentItem>(json) is not { } contentItem)
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
                await _notifier.ErrorAsync(new LocalizedHtmlString(details.Detail, details.Detail));
                return await Edit(contentItem.ContentItemId);
            case OkObjectResult:
                await _notifier.SuccessAsync(H["Content item {0} has been successfully saved.", GetName(contentItem)]);
                break;
            default:
                await _notifier.ErrorAsync(H["The submission has failed, please try again."]);
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

    private Task<IActionResult> UpdateContentAsync(ContentItem contentItem, bool isDraft) =>
        PostContentAsync(contentItem, isDraft);

    private static bool IsContinue(string submitString) =>
        submitString?.EndsWithOrdinalIgnoreCase("AndContinue") == true;

    private static string GetName(ContentItem contentItem) =>
        string.IsNullOrWhiteSpace(contentItem.DisplayText)
            ? contentItem.ContentType
            : $"\"{contentItem.DisplayText}\"";

    // Based on the OrchardCore.Contents.Controllers.ApiController.Post action that was deleted in
    // https://github.com/OrchardCMS/OrchardCore/commit/d524386b2f792f35773324ae482247e80a944266 to replace with minimal
    // APIs that can't be reused the same way.
    private async Task<IActionResult> PostContentAsync(ContentItem model, bool draft)
    {
        // It is really important to keep the proper method calls order with the ContentManager
        // so that all event handlers gets triggered in the right sequence.

        if (await _contentManager.GetAsync(model.ContentItemId, VersionOptions.DraftRequired) is { } contentItem)
        {
            if (!await _authorizationService.AuthorizeAsync(User, CommonPermissions.EditContent, contentItem))
            {
                return this.ChallengeOrForbid("Api");
            }

            contentItem.Merge(model, _updateJsonMergeSettings);

            await _contentManager.UpdateAsync(contentItem);
            var result = await _contentManager.ValidateAsync(contentItem);
            if (CheckContentValidationResult(result) is { } problem) return problem;
        }
        else
        {
            if (string.IsNullOrEmpty(model.ContentType) || await _contentDefinitionManager.GetTypeDefinitionAsync(model.ContentType) == null)
            {
                return BadRequest();
            }

            contentItem = await _contentManager.NewAsync(model.ContentType);
            contentItem.Owner = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await _authorizationService.AuthorizeAsync(User, CommonPermissions.PublishContent, contentItem))
            {
                return this.ChallengeOrForbid("Api");
            }

            contentItem.Merge(model);

            var result = await _contentManager.UpdateValidateAndCreateAsync(contentItem, VersionOptions.Draft);
            if (CheckContentValidationResult(result) is { } problem) return problem;
        }

        if (draft)
        {
            await _contentManager.SaveDraftAsync(contentItem);
        }
        else
        {
            await _contentManager.PublishAsync(contentItem);
        }

        return Ok(contentItem);
    }

    private ActionResult CheckContentValidationResult(ContentValidateResult result)
    {
        if (!result.Succeeded)
        {
            // Add the validation results to the ModelState to present the errors as part of the response.
            result.AddValidationErrorsToModelState(ModelState);
        }

        // We check the model state after calling all handlers because they trigger WF content events so, even they are not
        // intended to add model errors (only drivers), a WF content task may be executed inline and add some model errors.
        if (!ModelState.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(ModelState)
            {
                Title = T["One or more validation errors occurred."],
                Detail = string.Join(", ", ModelState.Values.SelectMany(state => state.Errors.Select(error => error.ErrorMessage))),
                Status = (int)HttpStatusCode.BadRequest,
            });
        }

        return null;
    }
}
