﻿using DevCreedApi_Net6Movies.Models;

namespace DevCreedApi_Net6Movies.Services
{
    public interface IAuthenticationSevice
    {
        Task<AuthenticationModel> Register(RegisterModel registerModel);
        Task<AuthenticationModel> GetLogin(LoginModel loginModel);
    }
}
