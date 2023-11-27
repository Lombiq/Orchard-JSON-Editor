using Lombiq.JsonEditor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.Contents;
using System.Threading.Tasks;
using YesSql;

namespace Lombiq.JsonEditor.Controllers;

public class ContentController : Controller
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IContentManager _contentManager;
    private readonly ISession _session;

    public ContentController(
        IAuthorizationService authorizationService,
        IContentManager contentManager,
        ISession session)
    {
        _authorizationService = authorizationService;
        _contentManager = contentManager;
        _session = session;
    }

    [Admin]
    [Route("/Contents/ContentItems/{contentItemId}/Edit/Json")]
    public async Task<IActionResult> Edit(string contentItemId)
    {
        if (string.IsNullOrWhiteSpace(contentItemId) ||
            await _contentManager.GetAsync(contentItemId, VersionOptions.Latest) is not { } contentItem ||
            !await CanEditAsync(contentItem))
        {
            return NotFound();
        }

        return View(new EditContentItemViewModel(contentItem, JsonConvert.SerializeObject(contentItem)));
    }

    [Admin]
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

        await _contentManager.PublishAsync(contentItem);
        _session.Save(contentItem);
        return RedirectToAction(nameof(Edit));
    }

    private Task<bool> CanEditAsync(ContentItem contentItem) =>
        _authorizationService.AuthorizeAsync(User, CommonPermissions.EditContent, contentItem);
}
