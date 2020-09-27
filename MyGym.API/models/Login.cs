namespace MyGym.API.models
{
    public class Login
    {
        public string CardNumber { get; set; }
        public string Email { get; set; }
        public string GoogleCaptcha { get; set; }
        public string Password { get; set; }
    }
}