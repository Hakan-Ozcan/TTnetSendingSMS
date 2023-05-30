using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Net;
using static System.Net.WebRequestMethods;

namespace MyNamespace
{
    public class SmsSonuc
    {
        public string Sonuc { get; set; }
        public string Kontor { get; set; }
        public string Message { get; set; }

    }

    class Program
    {
        string  GetToken()
        {
            string Token = "";
            var ApiUrl = "https://restapi.ttmesaj.com/ttmesajToken";
            var login = new Dictionary<string, string>
               {
                   {"grant_type", "password"},
                   {"username", "ttapiuser1"},
                   {"password", "ttapiuser1123"},
               };




            using (HttpClient httpClient = new HttpClient())
            {
                var response = httpClient.PostAsync(ApiUrl, new FormUrlEncodedContent(login)).Result;

                if (response.IsSuccessStatusCode)
                {
                    Dictionary<string, string> tokenDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result);

                    string token = tokenDetails.FirstOrDefault().Value;
                    Console.WriteLine("Token: " + token);
                    return token;
                }
                else
                {
                    Console.WriteLine("Token bilgisi okunamadı.");
                    return null;
                }
            }
        
        }

        void SendSMSTTNet()
        {
            var token = GetToken();

            string ApiUrl = "https://restapi.ttmesaj.com/";
            string description = string.Empty;

            var data = new
            {
                username = "rentgo",
                password = "R8K9L4M1",
                numbers = "905369900342",
                message = "TUNALAR",
                origin = "Rent Go",
                sd = "0",
                ed = "0",
                isNotification = true
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

            using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
            {
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Token Hatalı");
                    Console.ReadLine();
                    //initializer.TraceMe("Token Hatalı");             
                }
                else
                {

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var httppost = httpClient.PostAsync(ApiUrl + "api/SendSms/SendSingle", content).Result;
                    var response = JsonConvert.DeserializeObject<SmsSonuc>(httppost.Content.ReadAsStringAsync().Result);

                    if (response.Sonuc.Contains("*OK*")) // success
                    {
                        description = response.Sonuc.Replace("*OK*", "");

                        Console.WriteLine("Mesajınız başarıyla teslim edilmiştir." + Environment.NewLine + Environment.NewLine + "Message Id : " + description + Environment.NewLine + "Kontor : " + response.Kontor);
                        Console.ReadLine();
                        //initializer.TraceMe("Mesajınız başarıyla teslim edilmiştir." + Environment.NewLine + Environment.NewLine + "Message Id : " + description + Environment.NewLine + "Kontor : " + response.Kontor);
                    }
                    else
                    {
                        //initializer.TraceMe("Hata : " + response.Message);
                        Console.WriteLine("Hata : " + response.Message);
                        Console.ReadLine();
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            Program program = new Program();
            program.SendSMSTTNet();
        }
    }
}


