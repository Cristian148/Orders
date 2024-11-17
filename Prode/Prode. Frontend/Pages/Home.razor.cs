using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Prode.Shared.Resources;

namespace Prode._Frontend.Pages
{
    public partial class Home
    {
        [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    }
}