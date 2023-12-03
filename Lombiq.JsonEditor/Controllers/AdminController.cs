using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Lombiq.JsonEditor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using OrchardCore.Contents;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.Title.ViewModels;
using System.Threading.Tasks;
using YesSql;

namespace Lombiq.JsonEditor.Controllers;

public class AdminController : Controller
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IContentManager _contentManager;
    private readonly ILayoutAccessor _layoutAccessor;
    private readonly ISession _session;
    private readonly IShapeFactory _shapeFactory;
    private readonly IStringLocalizer<AdminController> T;

    public AdminController(
        IAuthorizationService authorizationService,
        IContentManager contentManager,
        ILayoutAccessor layoutAccessor,
        ISession session,
        IShapeFactory shapeFactory,
        IStringLocalizer<AdminController> stringLocalizer)
    {
        _authorizationService = authorizationService;
        _contentManager = contentManager;
        _layoutAccessor = layoutAccessor;
        _session = session;
        _shapeFactory = shapeFactory;
        T = stringLocalizer;
    }

    public async Task<IActionResult> Edit(string contentItemId)
    {
        if (string.IsNullOrWhiteSpace(contentItemId) ||
            await _contentManager.GetAsync(contentItemId, VersionOptions.Latest) is not { } contentItem ||
            !await CanEditAsync(contentItem))
        {
            return NotFound();
        }

        var titleShape = await _shapeFactory.CreateAsync<TitlePartViewModel>("TitlePart", model =>
        {
            model.Title = T["Edit {0} as JSON", contentItem.ContentType];
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

        if (await _contentManager.GetAsync(contentItem.ContentItemId, VersionOptions.Latest) is { } existing)
        {
            existing.Latest = false;
            existing.Published = false;
            _session.Save(existing);
            contentItem.ContentItemVersionId = null;
        }

        await _contentManager.PublishAsync(contentItem);
        _session.Save(contentItem);

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
