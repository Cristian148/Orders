using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Prode._Frontend.Repositories;
using Prode.Shared.Entites;
using Prode.Shared.Resources;

namespace Prode._Frontend.Pages.Countries
{
    public partial class CountriesIndex
    {
        [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        private List<Country>? Countries { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var responseHppt = await Repository.GetAsync<List<Country>>("api/countries");
            Countries = responseHppt.Response!;
        }
    }
}