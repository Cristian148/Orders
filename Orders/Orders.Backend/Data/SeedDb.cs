﻿using Microsoft.EntityFrameworkCore;
using Orders.Backend.Services;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IApiService _apiService;

        public SeedDb(DataContext context,IApiService apiService)
        {
            _context = context;
            _apiService = apiService;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            //await CheckContruiesAsync();
            await CheckContruiesApiAsync();
        }

        private async Task CheckContruiesAsync()
        {
            //Si no hay registros
            if (!_context.Countries.Any())
            {
                _context.Add(new Country
                {
                    Name = "Argentina",
                    States = new List<State>
                    {
                        new State{
                                Name= "Santa fe",
                                Cities = new List<City>
                                {
                                    new City{ Name="Santa fe de la Vera cruz"},
                                    new City{ Name="Rosario"}
                                }},
                        new State{ Name= "Entre Rios" },
                        new State{ Name= "Cordoba" },
                    }
                });

                _context.Add(new Country { Name = "Colombia" });
                _context.Add(new Country { Name = "Perü" });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckContruiesApiAsync()
        {
            //https://www.youtube.com/watch?v=PNEgCAqKOQo&t=3269s
            //Si no hay registros
            if (!_context.Countries.Any())
            {
                Response responseCountries = await _apiService.GetListAsync<CountryResponse>("/v1", "/countries");
                if (responseCountries.IsSuccess)
                {
                    List<CountryResponse> countries = (List<CountryResponse>)responseCountries.Result!;
                    foreach (CountryResponse countryResponse in countries)
                    {
                        Country country = await _context.Countries!.FirstOrDefaultAsync(c => c.Name == countryResponse.Name!)!;
                        if (country == null)
                        {
                            country = new() { Name = countryResponse.Name!, States = new List<State>() };
                            Response responseStates = await _apiService.GetListAsync<StateResponse>("/v1", $"/countries/{countryResponse.Iso2}/states");
                            if (responseStates.IsSuccess)
                            {
                                List<StateResponse> states = (List<StateResponse>)responseStates.Result!;
                                foreach (StateResponse stateResponse in states!)
                                {
                                    State state = country.States!.FirstOrDefault(s => s.Name == stateResponse.Name!)!;
                                    if (state == null)
                                    {
                                        state = new() { Name = stateResponse.Name!, Cities = new List<City>() };
                                        Response responseCities = await _apiService.GetListAsync<CityResponse>("/v1", $"/countries/{countryResponse.Iso2}/states/{stateResponse.Iso2}/cities");
                                        if (responseCities.IsSuccess)
                                        {
                                            List<CityResponse> cities = (List<CityResponse>)responseCities.Result!;
                                            foreach (CityResponse cityResponse in cities)
                                            {
                                                if (cityResponse.Name == "Mosfellsbær" || cityResponse.Name == "Șăulița")
                                                {
                                                    continue;
                                                }
                                                City city = state.Cities!.FirstOrDefault(c => c.Name == cityResponse.Name!)!;
                                                if (city == null)
                                                {
                                                    state.Cities.Add(new City() { Name = cityResponse.Name! });
                                                }
                                            }
                                        }
                                        if (state.CitiesNumber > 0)
                                        {
                                            country.States.Add(state);
                                        }
                                    }
                                }
                            }
                            if (country.StatesNumber > 0)
                            {
                                _context.Countries.Add(country);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }

            }
        }
    }
}