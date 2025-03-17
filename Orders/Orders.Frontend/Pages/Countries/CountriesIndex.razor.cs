using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.Countries
{
    public partial class CountriesIndex
    {
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private SweetAlertService sweetAlertService { get; set; } = null!;
        public List<Country>? Countries { get; set; }

        private int currentPage = 1;
        private int totalPages;

        [Parameter]
        [SupplyParameterFromQuery]
        public string Page { get; set; } = "";

        [Parameter]
        [SupplyParameterFromQuery]
        public string Filter { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadAsync();
        }

        private async Task SelectedPageAsync(int page)
        {
            currentPage = page;
            await LoadAsync(page);
        }

        private async Task LoadAsync(int page = 1)
        {
            if (!string.IsNullOrWhiteSpace(Page))
            {
                page = Convert.ToInt32(Page);
            }

            string url1 = string.Empty;
            string url2 = string.Empty;

            if (string.IsNullOrEmpty(Filter))
            {
                url1 = $"api/countries?page={page}";
                url2 = $"api/countries/totalPages";
            }
            else
            {
                url1 = $"api/countries?page={page}&filter={Filter}";
                url2 = $"api/countries/totalPages?filter={Filter}";
            }


            var responseHppt = await repository.GetAsync<List<Country>>(url1);
            var responseHppt2 = await repository.GetAsync<int>(url2);
            Countries = responseHppt.Response!;
            totalPages = responseHppt2.Response!;
        }


        private async Task DeleteAsync(Country country)
        {
            var result = await sweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Esta seguro que quieres borrar el registro?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });

            var confirm = string.IsNullOrEmpty(result.Value);

            if (confirm)
            {
                return;
            }

            var responseHTTP = await repository.DeleteAsync($"api/countries/{country.Id}");

            if (responseHTTP.Error)
            {
                if (responseHTTP.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // navigationManager.NavigateTo("/");
                }
                else
                {
                    var mensajeError = await responseHTTP.GetErrorMessageAsync();
                    await sweetAlertService.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
                }
            }
            else
            {
                await LoadAsync();
            }
        }


        private async Task CleanFilterAsync()
        {
            Filter = string.Empty;
            await ApplyFilterAsync();
        }

        private async Task ApplyFilterAsync()
        {
            int page = 1;
            await LoadAsync(page);
            await SelectedPageAsync(page);
        }
    }

    //private async Task LoadAsync()
    //{
    //    var responseHttp = await repository.GetAsync<List<Country>>("/api/countries");
    //    Countries = responseHttp.Response;
    //}
}
