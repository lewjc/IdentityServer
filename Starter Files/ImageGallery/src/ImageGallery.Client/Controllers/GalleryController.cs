﻿using IdentityModel.Client;
using ImageGallery.Client.Services;
using ImageGallery.Client.ViewModels;
using ImageGallery.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImageGallery.Client.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    {
        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;

        public GalleryController(IImageGalleryHttpClient imageGalleryHttpClient)
        {
            _imageGalleryHttpClient = imageGalleryHttpClient;
        }

        public async Task<IActionResult> Index()
        {

            WriteOutIdentityInformation();
            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient(); 

            var response = await httpClient.GetAsync("api/images").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var imagesAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var galleryIndexViewModel = new GalleryIndexViewModel(
                    JsonConvert.DeserializeObject<IList<Image>>(imagesAsString).ToList());

                return View(galleryIndexViewModel);
            } 
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {

                return RedirectToAction("AccessDenied", "Authorization");
            }

            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

        public async Task<IActionResult> EditImage(Guid id)
        {
            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.GetAsync($"api/images/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var imageAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var deserializedImage = JsonConvert.DeserializeObject<Image>(imageAsString);

                var editImageViewModel = new EditImageViewModel()
                {
                    Id = deserializedImage.Id,
                    Title = deserializedImage.Title
                };
                
                return View(editImageViewModel);
            }
           
            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImage(EditImageViewModel editImageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // create an ImageForUpdate instance
            var imageForUpdate = new ImageForUpdate()
                { Title = editImageViewModel.Title };

            // serialize it
            var serializedImageForUpdate = JsonConvert.SerializeObject(imageForUpdate);

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.PutAsync(
                $"api/images/{editImageViewModel.Id}",
                new StringContent(serializedImageForUpdate, System.Text.Encoding.Unicode, "application/json"))
                .ConfigureAwait(false);                        

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
          
            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

        public async Task<IActionResult> DeleteImage(Guid id)
        {
            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.DeleteAsync($"api/images/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
       
            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }
        
        public IActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles ="PayingUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(AddImageViewModel addImageViewModel)
        {   
            if (!ModelState.IsValid)
            {
                return View();
            }

            // create an ImageForCreation instance
            var imageForCreation = new ImageForCreation()
                { Title = addImageViewModel.Title };

            // take the first (only) file in the Files list
            var imageFile = addImageViewModel.Files.First();

            if (imageFile.Length > 0)
            {
                using (var fileStream = imageFile.OpenReadStream())
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    imageForCreation.Bytes = ms.ToArray();                     
                }
            }
            
            // serialize it
            var serializedImageForCreation = JsonConvert.SerializeObject(imageForCreation);

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.PostAsync(
                $"api/images",
                new StringContent(serializedImageForCreation, System.Text.Encoding.Unicode, "application/json"))
                .ConfigureAwait(false); 

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }  
        

        public async Task WriteOutIdentityInformation()
        {
            
            var identityToekn = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            Debug.WriteLine($"Identity Token: {identityToekn}");

            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim Type: {claim.Type} - Claim Value: {claim.Value}");
            }

        }

        public async Task Logout()
        {
            // Signs us out from the site and the identity server.
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");

            
        }

        [Authorize(Policy = "CanOrderFrame")]
        public async Task<IActionResult> OrderFrame()
        {
            // Gets the meta data document from the idp endpoint
            var discoveryClient = new DiscoveryClient("https://localhost:44356");

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
            var address = response.Claims.FirstOrDefault(c => c.Type == "address")?.Value;

            return View(new OrderFrameViewModel(address));

        }

    }
}
