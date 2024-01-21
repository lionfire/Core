using System;

namespace LionFire.Notifications;


public class Alert
{
    public string User { get; set; }
    public string Profile { get; set; }

    public string Category { get; set; }
    public string Object { get; set; }

    public string Title { get; set; }
    public string Message { get; set; }
    public string Verbose { get; set; }
    //public string EncodedMessage { get; set; }
    public int Priority { get; set; }
    public DateTimeOffset Date { get; set; }
    public int DispatchFailCount { get; set; }

    private static string SampleJson = """
        {
          "Title": "Test title",
          "User": "Test User",
          "Profile": "Test Profile",
          "Category": "Test Category",
          "Message": "Test message",
          "Verbose": "Test verbose",
          "Priority": 1,
          "Date": "2020-12-31T23:59:59.9999999-05:00"
        }
        """;
}
