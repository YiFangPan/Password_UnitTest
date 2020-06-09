using System;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;

namespace OneTimePassword
{
    public class AuthenticationService
    {
        public bool IsValid(String account, String password)
        {
            var passwordFromDb = String.Empty;

            using (var cn = new SqlConnection("connection"))
            {
                passwordFromDb = cn.Query<String>(
                    "select password from userDB where Account = @Account",
                    new
                    {
                        Account = account
                    }).FirstOrDefault();
            }

            var httpClient = new HttpClient() { BaseAddress = new Uri("webapi")};

            var response = httpClient.PostAsJsonAsync("api/otps", account).Result;
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"web api error, account:{account}");
            }

            var otp = response.Content.ReadAsAsync<String>().Result;
            
            return  String.Equals(password, $"{passwordFromDb}{otp}");
        }
    }
}