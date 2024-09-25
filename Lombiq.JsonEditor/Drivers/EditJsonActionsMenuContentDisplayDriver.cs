using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.ViewModels;
using OrchardCore.Contents;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;

namespace Lombiq.JsonEditor.Drivers;

public sealed class EditJsonActionsMenuContentDisplayDriver : ContentDisplayDriver
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _hca;

    public EditJsonActionsMenuContentDisplayDriver(IAuthorizationService authorizationService, IHttpContextAccessor hca)
    {
        _authorizationService = authorizationService;
        _hca = hca;
    }

    public override async Task<IDisplayResult> DisplayAsync(ContentItem model, BuildDisplayContext context) =>
        await _authorizationService.AuthorizeAsync(_hca.HttpContext?.User, CommonPermissions.EditContent, model)
            ? Initialize<ContentItemViewModel>("Content_EditJsonActions", viewModel =>
                    viewModel.ContentItem = model.ContentItem)
                .Location("ActionsMenu:after")
            : null;
}
