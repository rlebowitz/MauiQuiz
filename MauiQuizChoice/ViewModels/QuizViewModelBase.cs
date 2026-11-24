using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.Collections.ObjectModel;

namespace MauiQuizChoice.ViewModels;

public abstract partial class QuizViewModelBase<TItem, TReview> : ObservableObject
{
    private readonly List<TItem> _items = [];
    private int _currentIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoBack))]
    [NotifyPropertyChangedFor(nameof(ProgressValue))]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    [NotifyPropertyChangedFor(nameof(IsNextEnabled))]
    public partial TItem CurrentItem { get; set; } = default!;

    [ObservableProperty]
    public partial bool IsAnalyzing { get; set; }

    [ObservableProperty]
    public partial bool IsSummaryVisible { get; set; }

    [ObservableProperty]
    public partial string ScoreSummaryText { get; set; } = string.Empty;

    public ObservableCollection<TReview> ReviewResults { get; } = [];
 

    protected IReadOnlyList<TItem> Items => _items;
    protected int CurrentIndex => _currentIndex;

    public bool CanGoBack => _currentIndex > 0;
    public double ProgressValue => _items.Count == 0 ? 0 : (double)(_currentIndex + 1) / _items.Count;
    public string ProgressText => $"{_currentIndex + 1}/{_items.Count}";

    /// <summary>
    /// Subclasses define when the current question/round has a valid answer to enable "Next".
    /// </summary>
    public abstract bool IsNextEnabled { get; }

    /// <summary>
    /// Subclasses provide a fresh list of questions or rounds when starting or restarting.
    /// </summary>
    protected abstract List<TItem> CreateQuizItems();

    /// <summary>
    /// Subclasses evaluate all completed items and populate ReviewResults and ScoreSummaryText.
    /// </summary>
    protected abstract void EvaluateResults(IReadOnlyList<TItem> items);

    protected void InitializeQuiz()
    {
        _items.Clear();
        _items.AddRange(CreateQuizItems());

        ReviewResults.Clear();
        IsAnalyzing = false;
        IsSummaryVisible = false;
        ScoreSummaryText = string.Empty;

        SetCurrentIndex(0);
    }

    protected void SetCurrentIndex(int index)
    {
        if (index < 0 || index >= _items.Count)
            return;

        _currentIndex = index;
        CurrentItem = _items[_currentIndex];
        NotifyQuizStateChanged();
    }

    /// <summary>
    /// Call this from subclasses whenever user input changes on the current item.
    /// </summary>
    protected void NotifyQuizStateChanged()
    {
        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(ProgressValue));
        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(IsNextEnabled));
    }

    [RelayCommand]
    protected virtual async Task NextAsync()
    {
        if (!IsNextEnabled)
            return;

        if (_currentIndex < _items.Count - 1)
        {
            SetCurrentIndex(_currentIndex + 1);
            return;
        }

        IsAnalyzing = true;
        await Task.Delay(1200);

        EvaluateResults(_items);

        IsAnalyzing = false;
        IsSummaryVisible = true;
    }

    [RelayCommand]
    protected virtual void Previous()
    {
        if (CanGoBack)
        {
            SetCurrentIndex(_currentIndex - 1);
        }
    }

    [RelayCommand]
    protected virtual void Restart()
    {
        InitializeQuiz();
    }

    [RelayCommand]
    protected virtual async Task GoHomeAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}