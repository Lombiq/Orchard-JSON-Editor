using Lombiq.JsonEditor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using OrchardCore.Contents;
using System.Threading.Tasks;
using YesSql;

namespace Lombiq.JsonEditor.Controllers;

public class AdminController : Controller
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IContentManager _contentManager;
    private readonly ISession _session;

    public AdminController(
        IAuthorizationService authorizationService,
        IContentManager contentManager,
        ISession session)
    {
        _authorizationService = authorizationService;
        _contentManager = contentManager;
        _session = session;
    }

    public async Task<IActionResult> Edit(string contentItemId, string returnUrl)
    {
        if (string.IsNullOrWhiteSpace(contentItemId) ||
            await _contentManager.GetAsync(contentItemId, VersionOptions.Latest) is not { } contentItem ||
            !await CanEditAsync(contentItem))
        {
            return NotFound();
        }

        return View(new EditContentItemViewModel(contentItem, JsonConvert.SerializeObject(contentItem)));
    }

    [ValidateAntiForgeryToken]
    [HttpPost, ActionName(nameof(Edit))]
    public async Task<IActionResult> EditPost(string contentItemId, string json)
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
        return RedirectToAction(nameof(Edit), new { contentItemId });
    }

    private Task<bool> CanEditAsync(ContentItem contentItem) =>
        _authorizationService.AuthorizeAsync(User, CommonPermissions.EditContent, contentItem);
}
