﻿namespace InDream.Common.Configuration;

public class EmailSettings
{
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string FromEmail { get; set; }
    public string Password { get; set; }
}
