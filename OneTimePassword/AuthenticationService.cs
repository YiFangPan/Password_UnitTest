using System;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;

namespace OneTimePassword
{
    public interface IOtpService
    {
        string GetOtp(string account);
    }

    public class OtpService : IOtpService
    {
        public string GetOtp(string account)
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
    }

    public interface IUserRepo
    {
        string GetPasswordFromDb(string account);
    }

    public class UserRepo : IUserRepo
    {
        public string GetPasswordFromDb(string account)
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

    public class AuthenticationService
    {
        private readonly IOtpService _otpService;
        private readonly IUserRepo _userRepo;

        public AuthenticationService(IOtpService service, IUserRepo repo)
        {
            _otpService = service;
            _userRepo = repo;
        }
        
        public bool IsValid(String account, String password)
        {
            var passwordFromDb = _userRepo.GetPasswordFromDb(account);

            var otp = _otpService.GetOtp(account);

            return String.Equals(password, $"{passwordFromDb}{otp}");
        }
    }
}