using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Email;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;

namespace BrainstormSessions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var emailLogsSmtpInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("emailLogsSmtpInfo.json"));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File("Logs\\logs.log")
                .WriteTo.Email(new EmailConnectionInfo
                {
                    FromEmail = emailLogsSmtpInfo["usernameEmail"],
                    ToEmail = emailLogsSmtpInfo["toEmail"],
                    MailServer = emailLogsSmtpInfo["emailServer"],
                    NetworkCredentials = new NetworkCredential
                    {
                        UserName = emailLogsSmtpInfo["username"],
                        Password = emailLogsSmtpInfo["password"]
                    },
                    EnableSsl = true,
                    Port = Int32.Parse(emailLogsSmtpInfo["port"]),
                    EmailSubject = emailLogsSmtpInfo["subject"]
                })
                .CreateLogger();

            Log.Information("Starting web host");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
