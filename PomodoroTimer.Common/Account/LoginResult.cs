﻿namespace PomodoroTimer.Common.Account
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string? Error { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
