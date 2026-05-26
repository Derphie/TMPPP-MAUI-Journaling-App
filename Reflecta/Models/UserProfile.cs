using SQLite;

namespace Reflecta.Models;

[Table("UserProfile")]
public class UserProfile
{
    [PrimaryKey]
    public int Id { get; set; } = 1; 
    public string Name { get; set; } = "Journaler";
    public string AvatarEmoji { get; set; } = "🌟";
    public bool NotificationsEnabled { get; set; } = true;
    public string ReminderTime { get; set; } = "21:00";
    public string ActiveStrategy { get; set; } = "Simple"; 
    public bool OnboardingCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
