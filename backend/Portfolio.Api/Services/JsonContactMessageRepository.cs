using System.Text.Json;
using Portfolio.Api.Models;

namespace Portfolio.Api.Services;

public sealed class JsonContactMessageRepository : IContactMessageRepository
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public JsonContactMessageRepository(IWebHostEnvironment environment)
    {
        var dataDirectory = Path.Combine(environment.ContentRootPath, "Data");
        Directory.CreateDirectory(dataDirectory);
        _filePath = Path.Combine(dataDirectory, "contact-messages.json");
    }

    public async Task<ContactMessage> CreateAsync(ContactMessage message, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var messages = await ReadMessagesAsync(cancellationToken);
            messages.Add(message);

            await using var stream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(stream, messages, _jsonOptions, cancellationToken);
            return message;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<ContactMessage>> ReadMessagesAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<ContactMessage>>(stream, _jsonOptions, cancellationToken) ?? [];
    }
}
