using System.Text;
using System.Text.Json;
using ApiDbUi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiDbUi.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGet()
        {
            await CreateContact();
            await GetAllContacts();
        }

        private async Task CreateContact()
        {
            ContactModel contact = new ContactModel
            {
                FirstName = "jimmy",
                LastName = "hendrix",
            };
            contact.EmailAddresses.Add(new EmailAddressModel { EmailAddress = "jimmy@gmail.com" });
            contact.EmailAddresses.Add(new EmailAddressModel { EmailAddress = "hendrix@gmail.com" });
            contact.PhoneNumbers.Add(new PhoneNumberModel { PhoneNumber = "66600666" });

            var _client = _httpClientFactory.CreateClient();
            var response = await _client.PostAsync(
                "https://localhost:44338/api/contacts",
                    new StringContent(JsonSerializer.Serialize(contact), Encoding.UTF8, "application/json"));
        }

        private async Task GetAllContacts()
        {
            var _client = _httpClientFactory.CreateClient();
            var response = await _client.GetAsync("https://localhost:44338/api/contacts");

            List<ContactModel> contacts;

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };
                string responseText = await response.Content.ReadAsStringAsync();
                contacts = JsonSerializer.Deserialize<List<ContactModel>>(responseText, options);
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
