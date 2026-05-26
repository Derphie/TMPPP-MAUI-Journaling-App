namespace Reflecta.Services;

public interface IShareService
{
    Task ShareTextAsync(string title, string text);
}


public class AndroidShareService : IShareService
{
    public Task ShareTextAsync(string title, string text) =>
        Share.RequestAsync(new ShareTextRequest { Title = title, Text = text });
}

public class IosShareService : IShareService
{
    public Task ShareTextAsync(string title, string text) =>
        Share.RequestAsync(new ShareTextRequest { Title = title, Text = text });
}
