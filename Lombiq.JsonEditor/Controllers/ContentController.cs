using Lombiq.JsonEditor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.Contents;
using System.Threading.Tasks;

namespace Lombiq.JsonEditor.Controllers;

public class ContentController : Controller
{
    private readonly IContentManager _contentManager;
    private readonly IAuthorizationService _authorizationService;

    public ContentController(IContentManager contentManager, IAuthorizationService authorizationService)
    {
        _contentManager = contentManager;
        _authorizationService = authorizationService;
    }

    [Admin]
    [Route("/Contents/ContentItems/{contentItemId}/Edit/Json")]
    public async Task<IActionResult> Edit(string contentItemId)
    {
        if (string.IsNullOrWhiteSpace(contentItemId) ||
            await _contentManager.GetAsync(contentItemId, VersionOptions.Latest) is not { } contentItem ||
            !await _authorizationService.AuthorizeAsync(User, CommonPermissions.EditContent, contentItem))
        {
            return NotFound();
        }

        return View(new EditContentItemViewModel(contentItem, JsonConvert.SerializeObject(contentItem)));
    }
}
