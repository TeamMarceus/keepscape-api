﻿namespace keepscape_api.Services.Emails
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
