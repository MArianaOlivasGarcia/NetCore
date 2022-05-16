using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApiINMO.DTOs;

namespace WebApiINMO.Services
{
    public class HashService
    {


        public HashResult Hash(string textPlane)
        {

            var salt = new byte[16];

            using ( var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            return Hash(textPlane, salt);

        }



        public HashResult Hash(string textPlane, byte[] salt)
        {
            var keyDerivation = KeyDerivation.Pbkdf2(
                    password: textPlane,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 32
                );

            var hash = Convert.ToBase64String(keyDerivation);

            return new HashResult()
            {
                Hash = hash,
                Salt = salt
            };
        }
    }
}
