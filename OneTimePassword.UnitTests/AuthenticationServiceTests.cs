using System;
using NUnit.Framework;
using NSubstitute;

namespace OneTimePassword.UnitTests
{
    public class AuthenticationServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Valid_Password_Using_Fake()
        {
            var authenticationService = new AuthenticationService(new FakeService(), new FakeRepo());
            var result = authenticationService.IsValid("Yvonne", "abc123");
            
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Valid_Password_Using_NSub()
        {
            var repo = Substitute.For<UserRepo>();
            repo.GetPasswordFromDb("Yvonne").Returns("abc");
            
            var otp = Substitute.For<OtpService>();
            otp.GetOtp("Yvonne").Returns("123");

            var authenticationService = new AuthenticationService(otp, repo);
            var result = authenticationService.IsValid("Yvonne", "abc123");
            
            Assert.IsTrue(result);
        }
    }

    public class FakeRepo : UserRepo
    {
        public override string GetPasswordFromDb(string account)
        {
            if (account == "Yvonne")
                return "abc";
            return String.Empty;
        }
    }

    public class FakeService : OtpService
    {
        public override string GetOtp(string account)
        {
            if (account == "Yvonne")
                return "123";
            return String.Empty;
        }
    }
}