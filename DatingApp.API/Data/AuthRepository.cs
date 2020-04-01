using System.Threading.Tasks;
using DatingApp.API.Models;
using System.Security.Cryptography;
using System;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string Username, string Password)
        {
           var user = await _context.Users.FirstOrDefaultAsync(x => x.Username==Username);

           //Checking User Exists
           if (user==null)
                return null;
        
            if(!VerifyComputedHash(Password,user.PasswordHash,user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyComputedHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new HMACSHA512(passwordSalt))
            {
               
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for(int i=0;i<computedHash.Length;i++){
                    if(computedHash[i]!=passwordHash[i]) return false;
                }

            }
            return true;
        }

        public async Task<User> Register(User user, string Password)
        {
            byte[] passwordHash, passwordSalt;

           //Creating Password Hash using HMACSHA512
            CreatePasswordHash(Password, out passwordHash, out passwordSalt);

            user.PasswordHash=passwordHash;
            user.PasswordSalt=passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;

        }

        public void CreatePasswordHash(string Password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
            }
        }
        public async Task<bool> UserExists(string Username)
        {
            if(await _context.Users.AnyAsync(x => x.Username==Username))
                 return true;
            return false;   
        }
    }
}