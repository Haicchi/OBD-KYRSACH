using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace TourAgency.Services
{
    public class AuthService
    {
        public bool Register(string name, string email, string phone, string password, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                message = "Усі обов'язкові поля повинні бути заповнені!";
                return false;
            }

            if (email.Count(c => c == '@') != 1)
            {
                message = "Email повинен містити рівно один символ '@'!";
                return false;
            }

            if (password.Length < 6)
            {
                message = "Пароль має бути не менше 6 символів!";
                return false;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    if (db.Accounts.Any(a => a.Email == email))
                    {
                        message = "Користувач з таким Email вже існує!";
                        return false;
                    }
                    string salt = GenerateSalt();
                    string hashedPassword = HashPassword(password, salt);
                    var newAccount = new Account
                    {
                        Name = name,
                        Email = email,
                        Phone = phone,
                        Password = hashedPassword, 
                        Salt = salt,               
                        Role = "User"
                    };

                    db.Accounts.Add(newAccount);
                    db.SaveChanges();
                }

                message = "Реєстрація успішна!";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Помилка бази даних: {ex.Message}";
                return false;
            }
        }

        
        public Account Login(string email, string password, out string message)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var account = db.Accounts.FirstOrDefault(a => a.Email == email);

                    if (account != null)
                    {
                      
                        string hashedInput = HashPassword(password, account.Salt);

                        if (hashedInput == account.Password)
                        {
                            message = "Вхід успішний!";
                            return account;
                        }
                    }

                    message = "Невірний Email або пароль.";
                    return null;
                }
            }
            catch (Exception ex)
            {
                message = $"Помилка з'єднання: {ex.Message}";
                return null;
            }
        }

        private static string GenerateSalt(int length = 32)
        {
            byte[] saltBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private static string HashPassword(string password, string salt)
        {
            string saltedPassword = password + salt;
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}