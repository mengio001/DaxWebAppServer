namespace QuizTowerPlatform.BFF.Entities;

public interface IConcurrencyAware
{
    string ConcurrencyStamp { get; set; }
}