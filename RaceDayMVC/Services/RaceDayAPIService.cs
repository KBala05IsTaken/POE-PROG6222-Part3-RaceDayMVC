using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace RaceDayMVC.Services
{
    public class RaceDayApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RaceDayApiService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var token = _httpContextAccessor.HttpContext?
                .Session.GetString("AuthToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var client = CreateClient();
            var response = await client.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
            string endpoint, TRequest data)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync(endpoint, data);

            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<bool> PostAsync<TRequest>(
            string endpoint, TRequest data)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync(endpoint, data);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutAsync<TRequest>(
            string endpoint, TRequest data)
        {
            var client = CreateClient();
            var response = await client.PutAsJsonAsync(endpoint, data);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync(endpoint);

            return response.IsSuccessStatusCode;
        }

        // Sends a file and other form fields to the API.
        // This is used for event banners and participant profile pictures.
        public async Task<bool> PostFileAsync(
            string endpoint,
            Dictionary<string, string> fields,
            IFormFile? file,
            string fileFieldName)
        {
            var client = CreateClient();

            using var content = new MultipartFormDataContent();

            // Add the ordinary form fields.
            foreach (var field in fields)
            {
                content.Add(
                    new StringContent(field.Value ?? ""),
                    field.Key);
            }

            // Add the image only if the user selected one.
            if (file != null && file.Length > 0)
            {
                var streamContent = new StreamContent(file.OpenReadStream());

                streamContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(
                        file.ContentType);

                content.Add(
                    streamContent,
                    fileFieldName,
                    Path.GetFileName(file.FileName));
            }

            var response = await client.PostAsync(endpoint, content);

            return response.IsSuccessStatusCode;
        }

        // Sends a multipart update to the API.
        public async Task<bool> PutFileAsync(
            string endpoint,
            Dictionary<string, string> fields,
            IFormFile? file,
            string fileFieldName)
        {
            var client = CreateClient();

            using var content = new MultipartFormDataContent();

            foreach (var field in fields)
            {
                content.Add(new StringContent(field.Value ?? ""), field.Key);
            }

            if (file != null && file.Length > 0)
            {
                var streamContent = new StreamContent(file.OpenReadStream());

                streamContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(
                        file.ContentType);

                content.Add(
                    streamContent,
                    fileFieldName,
                    Path.GetFileName(file.FileName));
            }

            var response = await client.PutAsync(endpoint, content);

            return response.IsSuccessStatusCode;
        }
    }


}
