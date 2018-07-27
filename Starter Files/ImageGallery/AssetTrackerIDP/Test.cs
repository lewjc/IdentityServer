using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssetTrackerIDP
{
    public class Test
    {


		// Tester method to get information from the user info point.
		public void GetInfoFromAccessPoint()
        {
            // Gets the meta data document from the idp endpoint
			var discoveryClient = new DiscoveryClient("https://localhost:44319/");

			// get meta data from idp
			var metaDataResponse = await discoveryClient.GetAsync();

			// pass the endpoint to the user info client
			var userInfoClient = new UserInfoClient(metaDataResponse.UserInfoEndpoint);

			// get an access token
			var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

			// pass that access token to the endpoint and get info from the user info endpoint.
			var response = await userInfoClient.GetAsync(accessToken);

			if (response.IsError)
			{
				throw new Exception("Error accessing user info endpoint for IDP", response.Exception);
			}

			// Here we search through all of the claims and grab the first claim that is an address, if it is not null we grab its value.
			var claims = response.Claims.Select(c => c.Type == "Group");

				foreach(var c in claims)
				{
					Debug.WriteLine(c);
				}
        }
    }
}
