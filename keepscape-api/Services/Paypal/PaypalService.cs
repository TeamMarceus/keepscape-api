using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace keepscape_api.Services.Paypal
{
    public class PaypalService : IPaypalService
    {
        private readonly string _clientId = "AVF1QLI9DgwDJoA76NSIc7BhFEGKMDS1_E_A7vM_0Pkj0tsNLSTu7BNllTwztJ3Jtajz7kzgAjheEBqM";
        private readonly string _clientSecret = "EHxFuttafNKVEpPhS_R94MvSqGJYs648ZgCHIseDFlTXyqG_ozoFN_SmGs8WgkfYe9CioH6sCmJmd_Im";
        private readonly string _sandboxUrl = "https://api.sandbox.paypal.com/v2/checkout/orders/";
        private readonly string _authUrl = "https://api.sandbox.paypal.com/v1/oauth2/token";

        public async Task<bool> ValidatePaypalPayment(Guid userId, string paypalOrderId)
        {
            using (var httpClient = new HttpClient())
            {
                string APIUrl = _sandboxUrl + paypalOrderId;

                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(_clientId + ":" + _clientSecret));

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                
                // Add body to http request
                var body = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" }
                };

                var content = new FormUrlEncodedContent(body);

                var response = await httpClient.PostAsync(_authUrl, content);

                var jsonAccess = JObject.Parse(await response.Content.ReadAsStringAsync());

                var access_token = jsonAccess["access_token"];

                if (access_token == null)
                {
                    return false;
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token.ToString());

                response = await httpClient.GetAsync(APIUrl);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadAsStringAsync();

                        var json = JObject.Parse(result);
                        var status = json["status"];

                        if (status != null)
                        {
                            if (status.ToString() == "COMPLETED")
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }
    }
}
