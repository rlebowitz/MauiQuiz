namespace MauiQuizChoice.Models;

public class MatchingRound
{
    public string Instruction { get; set; } = string.Empty;
    public List<MatchPromptItem> Prompts { get; set; } = [];
    public List<MatchTargetItem> Targets { get; set; } = [];
}