using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            //Adding Claims
            var claim = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId,user.UserName)
            };

            //Creating Credentials
            var creds = new SigningCredentials(_key,SecurityAlgorithms.HmacSha512Signature);

            //Describing how a token is gonna look
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            //Token Handler
            var tokenHandler = new JwtSecurityTokenHandler();

            //Create the token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //Return the written token to whoever needs it
            return tokenHandler.WriteToken(token);
        }
    }
}