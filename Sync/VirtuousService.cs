using RestSharp;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Sync
{
    /// <summary>
    /// API Docs found at https://docs.virtuoussoftware.com/
    /// </summary>
    internal class VirtuousService
    {
        private readonly RestClient _restClient;

        public VirtuousService(IConfiguration configuration) 
        {
            var apiBaseUrl = configuration.GetValue("VirtuousApiBaseUrl");
            var apiKey = configuration.GetValue("VirtuousApiKey");

            var options = new RestClientOptions(apiBaseUrl)
            {
                Authenticator = new RestSharp.Authenticators.OAuth2.OAuth2AuthorizationRequestHeaderAuthenticator(apiKey)
            };

            _restClient = new RestClient(options);
        }

        public async Task<List<AbbreviatedContact>> GetContactsByStateAsync(int skip, int take, string state)
        {
            var request = new RestRequest("/api/Contact/Query", Method.Post);
            request.AddQueryParameter("Skip", skip);
            request.AddQueryParameter("Take", take);

            var body = new ContactQueryRequest();
            request.AddJsonBody(body);

            var allContacts = await _restClient.PostAsync<PagedResult<AbbreviatedContact>>(request);

            //this is assuming all contacts have US addresses
            //if not, use api/ContactAddress/ByContact/:contactId and filter by state
            var contactsByState = ContactsHelper.FilterContactsByState(allContacts.List, state);
            return contactsByState;
        }
    }
}
