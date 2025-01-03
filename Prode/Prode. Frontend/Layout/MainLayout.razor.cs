using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Prode.Shared.Resources;

namespace Prode._Frontend.Layout
{
    public partial class MainLayout
    {
        [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    }
}