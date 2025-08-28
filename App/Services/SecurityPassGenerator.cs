using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTag.App.Services
{
    public static class SecurityPassGenerator
    {
        private static readonly Random rnd = new();
        public static string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            char[] res = new char[length];
            for (int i = 0; i < length; i++)
                res[i] = validChars[rnd.Next(validChars.Length)];
            return new string(res);
        }
        public static string GenerateUniqueCode(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            char[] res = new char[length];
            for (int i = 0; i < length; i++)
                res[i] = validChars[rnd.Next(validChars.Length)];
            return new string(res);
        }
    }
}
