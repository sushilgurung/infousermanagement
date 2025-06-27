using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace BlazorClient.Extensions;

public static class HttpClientExtensions
{
    private static JsonSerializerOptions _propertyNameCaseInsensitiveSerializer => new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static JsonSerializerOptions _ignoreNullPropertyInsensitiveSerializer => new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };

    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
    {
        var dataAsString = JsonSerializer.Serialize(data, _ignoreNullPropertyInsensitiveSerializer);
        var content = new StringContent(dataAsString);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return httpClient.PostAsync(url, content);
    }

    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
    {
        var dataAsString = JsonSerializer.Serialize(data);
        var content = new StringContent(dataAsString);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return httpClient.PutAsync(url, content);
    }

    public static Task<HttpResponseMessage> GetWithParamAsync<T>(this HttpClient httpClient, string url, T data)
    {
        var queryString = data?.GetQueryString();
        return httpClient.GetAsync($"{url}{queryString}");
    }

    public static async Task<T> ReadContentAs<T>(this HttpResponseMessage response)
    {
        var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var result = JsonSerializer.Deserialize<T>(dataAsString, _propertyNameCaseInsensitiveSerializer);
        return result ?? throw new InvalidOperationException($"Unable to deserialize response to {typeof(T).Name}.");
    }
    public static string GetQueryString(this object obj)
    {
        var properties = from p in obj.GetType().GetProperties()
                         where p.GetValue(obj, null) != null
                         select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());
        return $"?{string.Join("&", properties.ToArray())}";
    }
}