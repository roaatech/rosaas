﻿namespace Roaa.Rosas.Application.Services.Identity.Auth.Models;

public record SignUpUserByEmailModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
}