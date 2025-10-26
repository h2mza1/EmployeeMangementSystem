using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var key = Encoding.ASCII.GetBytes("bA93jsK!29LpQ#6mX0@cH5zNf2rT7wUe");

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // لا تسمح بوقت تأخير إضافي
                }
            });
        }
    }
}
