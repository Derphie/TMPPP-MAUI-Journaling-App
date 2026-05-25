using Reflecta.Repositories;

namespace Reflecta.Services;

public interface IExportService
{
    Task<string> ExportToTextAsync();
}

public class ExportService : IExportService
{
    private readonly IJournalRepository _repo;

    public ExportService(IJournalRepository repo) => _repo = repo;

    public async Task<string> ExportToTextAsync()
    {
        var entries = await _repo.GetAllAsync();
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("# Reflecta Journal Export");
        sb.AppendLine($"Exported: {DateTime.Now:yyyy-MM-dd HH:mm}");
        sb.AppendLine();

        foreach (var e in entries)
        {
            sb.AppendLine($"## {e.Title}");
            sb.AppendLine($"Date: {e.CreatedAt:yyyy-MM-dd HH:mm}  |  Mood: {e.Mood}  |  Tags: {e.Tags}");
            sb.AppendLine();
            sb.AppendLine(e.Body);
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
