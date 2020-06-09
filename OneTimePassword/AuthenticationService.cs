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
            var passwordFromDb = GetPasswordFromDb(account);

            var otp = GetOtp(account);

            return String.Equals(password, $"{passwordFromDb}{otp}");
        }

        private static string GetOtp(string account)
        {
            var httpClient = new HttpClient() {BaseAddress = new Uri("webapi")};

            var response = httpClient.PostAsJsonAsync("api/otps", account).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"web api error, account:{account}");
            }

            var otp = response.Content.ReadAsAsync<String>().Result;
            return otp;
        }

        private static string GetPasswordFromDb(string account)
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

            return passwordFromDb;
        }
    }
}