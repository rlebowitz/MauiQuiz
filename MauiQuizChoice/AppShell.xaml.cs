

using MauiQuizChoice.Views;
namespace MauiQuizChoice
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MultipleChoiceQuizPage), typeof(MultipleChoiceQuizPage));
            Routing.RegisterRoute(nameof(MatchingQuizPage), typeof(MatchingQuizPage));
            Routing.RegisterRoute(nameof(TrueFalseQuizPage), typeof(TrueFalseQuizPage));
            Routing.RegisterRoute(nameof(MultiSelectQuizPage), typeof(MultiSelectQuizPage));
            Routing.RegisterRoute(nameof(FillInQuizPage), typeof(FillInQuizPage));
            Routing.RegisterRoute(nameof(FlashcardQuizPage), typeof(FlashcardQuizPage));
            Routing.RegisterRoute(nameof(OrderingQuizPage), typeof(OrderingQuizPage));
        }
    }
}
