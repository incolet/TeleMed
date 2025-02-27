using TeleMed.States;

namespace TeleMed.Common.Extensions;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> ExtSendRequestAsync<TBody>(this HttpClient httpClient, HttpRequestMessage request,TBody body, bool attachToken = true)
    {
        try
        {
            
            if (attachToken)
            {
                // Attach token to request header
                var token = Constants.JwtToken; 
                request.Headers.Add("Authorization", $"Bearer {token}");
            }

            if (body == null) return await httpClient.SendAsync(request);
            
            // Serialize the body and set it in the request content
            var serializedBody = Newtonsoft.Json.JsonConvert.SerializeObject(body); 
            request.Content = new StringContent(serializedBody, System.Text.Encoding.UTF8, "application/json");

            return httpClient.Send(request);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    public static async Task<HttpResponseMessage> ExtSendRequestAsyncNoBody(this HttpClient httpClient, HttpRequestMessage request, bool attachToken = true)
    {
        try
        {
            if (!attachToken) return await httpClient.SendAsync(request);
            // Attach token to request header
            var token = Constants.JwtToken; 
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = httpClient.Send(request);
            return response;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}