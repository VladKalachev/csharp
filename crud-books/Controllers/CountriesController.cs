using System.Linq;
using BookApiProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApiProject.Controllers
{

    [Route("api/[contoller]")]
    [ApiController]
    public class CountriesController : Controller
    {
        private ICountryRepository _countryRepository;

        public CountriesController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }
        //api/countries
        [HttpGet]
        public IActionResult GetCountries()
        {   
            var countries = _countryRepository.GetCountries().ToList();

            return Ok(countries);
        }

    }
}