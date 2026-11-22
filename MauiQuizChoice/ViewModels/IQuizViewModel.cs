using System.Collections;
using System.Windows.Input;

namespace MauiQuizChoice.ViewModels;

public interface IQuizViewModel
{
    bool CanGoBack { get; }
    double ProgressValue { get; }
    string ProgressText { get; }
    bool IsNextEnabled { get; }
    bool IsAnalyzing { get; }
    bool IsSummaryVisible { get; }
    string ScoreSummaryText { get; }
    IEnumerable ResultsSource { get; }

    ICommand NextCommand { get; }
    ICommand PreviousCommand { get; }
    ICommand RestartCommand { get; }
    ICommand GoHomeCommand { get; }
}