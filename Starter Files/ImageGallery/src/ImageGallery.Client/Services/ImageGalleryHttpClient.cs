using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ImageGallery.Client.Services
{
    public class ImageGalleryHttpClient : IImageGalleryHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpClient _httpClient = new HttpClient();

        public ImageGalleryHttpClient(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<HttpClient> GetClient()
        {

            string accessToken = string.Empty;

            // Get the current httpcontext
            var currentContext = _httpContextAccessor.HttpContext;

            // request the access token
            accessToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            // add the access token as a beaere token. Give access to the bearer of this token.
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                _httpClient.SetBearerToken(accessToken);
            }



            _httpClient.BaseAddress = new Uri("https://localhost:44303//");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
        }        
    }
}

