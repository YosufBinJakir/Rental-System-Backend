using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalSystem.ViewModels;
using System.Text;

namespace RentalSystem.Controllers
{
    
    public abstract class BaseController : ControllerBase
    {
        public static class JwtHelper
        {
            public static string Base64UrlDecode(string input)
            {
                input = input.Replace('-', '+').Replace('_', '/');
                switch (input.Length % 4)
                {
                    case 2: input += "=="; break;
                    case 3: input += "="; break;
                }
                return Encoding.UTF8.GetString(Convert.FromBase64String(input));
            }
        }

        public JwtClaims GetUserIdFromToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    throw new ArgumentException("Token is null or empty.");
                }

                var tokenParts = token.Split('.');
                if (tokenParts.Length != 3)
                {
                    throw new FormatException("Invalid token structure.");
                }

                var payload = tokenParts[1];
                var jsonPayload = JwtHelper.Base64UrlDecode(payload);

                var jwtPayload = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                return new JwtClaims
                {
                    Id = jwtPayload.ContainsKey("sub") ? jwtPayload["sub"] : null,
                    UserName = jwtPayload.ContainsKey("name") ? jwtPayload["name"] : null,
                    Email = jwtPayload.ContainsKey("email") ? jwtPayload["email"] : null,
                    PhoneNumber = jwtPayload.ContainsKey("phone_number") ? jwtPayload["phone_number"] : null,
                    Role = jwtPayload.ContainsKey("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                        ? jwtPayload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
                        : null
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding token: {ex.Message}");
                throw; 
            }
        }
    }
}
