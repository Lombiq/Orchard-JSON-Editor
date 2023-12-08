using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;
using Lombiq.JsonEditor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using OrchardCore.Contents;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.DisplayManagement.Title;
using OrchardCore.Title.ViewModels;
using System.Threading.Tasks;
using YesSql;

namespace Lombiq.JsonEditor.Controllers;

public class AdminController : Controller
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IContentItemIdGenerator _contentItemIdGenerator;
    private readonly IContentManager _contentManager;
    private readonly ILayoutAccessor _layoutAccessor;
    private readonly IPageTitleBuilder _pageTitleBuilder;
    private readonly ISession _session;
    private readonly IShapeFactory _shapeFactory;
    private readonly IStringLocalizer<AdminController> T;

    public AdminController(
        IContentItemIdGenerator contentItemIdGenerator,
        ILayoutAccessor layoutAccessor,
        IPageTitleBuilder pageTitleBuilder,
        IShapeFactory shapeFactory,
        IOrchardServices<AdminController> services)
    {
        _authorizationService = services.AuthorizationService.Value;
        _contentItemIdGenerator = contentItemIdGenerator;
        _contentManager = services.ContentManager.Value;
        _layoutAccessor = layoutAccessor;
        _pageTitleBuilder = pageTitleBuilder;
        _session = services.Session.Value;
        _shapeFactory = shapeFactory;
        T = services.StringLocalizer.Value;
    }

    public async Task<IActionResult> Edit(string contentItemId)
    {
        if (string.IsNullOrWhiteSpace(contentItemId) ||
            await _contentManager.GetAsync(contentItemId, VersionOptions.Latest) is not { } contentItem ||
            !await CanEditAsync(contentItem))
        {
            return NotFound();
        }

        var name = string.IsNullOrWhiteSpace(contentItem.DisplayText)
            ? contentItem.ContentType
            : $"\"{contentItem.DisplayText}\"";
        var title = T["Edit {0} as JSON", name].Value;
        _pageTitleBuilder.AddSegment(new StringHtmlContent(title));
        var titleShape = await _shapeFactory.CreateAsync<TitlePartViewModel>("TitlePart", model =>
        {
            model.Title = title;
            model.ContentItem = contentItem;
        });
        await _layoutAccessor.AddShapeToZoneAsync("Title", titleShape);

        return View(new EditContentItemViewModel(contentItem, JsonConvert.SerializeObject(contentItem)));
    }

    [ValidateAntiForgeryToken]
    [HttpPost, ActionName(nameof(Edit))]
    public async Task<IActionResult> EditPost(
        string contentItemId,
        string json,
        string returnUrl,
        [Bind(Prefix = "submit.Publish")] string submitPublish)
    {
        if (string.IsNullOrWhiteSpace(contentItemId) ||
            string.IsNullOrWhiteSpace(json) ||
            JsonConvert.DeserializeObject<ContentItem>(json) is not { } contentItem)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(contentItem.ContentItemId)) contentItem.ContentItemId = contentItemId;
        if (!await CanEditAsync(contentItem))
        {
            return NotFound();
        }

        await _contentManager.LoadAsync(contentItem);

        if (await _contentManager.GetAsync(contentItem.ContentItemId, VersionOptions.Latest) is { } existing)
        {
            existing.Latest = false;
            _session.Save(existing);
            contentItem.ContentItemVersionId = _contentItemIdGenerator.GenerateUniqueId(existing);
        }

        contentItem.Published = false;
        await _contentManager.PublishAsync(contentItem);

        if (!string.IsNullOrEmpty(returnUrl) &&
            submitPublish != "submit.PublishAndContinue" &&
            Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Edit), new { contentItemId, returnUrl });
    }

    private Task<bool> CanEditAsync(ContentItem contentItem) =>
        _authorizationService.AuthorizeAsync(User, CommonPermissions.EditContent, contentItem);
}
