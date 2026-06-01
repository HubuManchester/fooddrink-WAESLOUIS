using FoodDrinkApp.Services;

namespace FoodDrinkApp;

public partial class MainPage : ContentPage
{
    private CancellationTokenSource? searchDebounce;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        await LoadFoodItemsAsync(SearchFoodBar.Text);
    }

    private async Task LoadFoodItemsAsync(string? query = null)
    {
        FoodCollection.ItemsSource = await FoodCatalogService.SearchAsync(query);
    }

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddItemPage));
    }

    private async void OnDetailsClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string id)
        {
            await Shell.Current.GoToAsync($"{nameof(FoodDetailPage)}?id={Uri.EscapeDataString(id)}");
        }
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        searchDebounce?.Cancel();
        searchDebounce = new CancellationTokenSource();
        var token = searchDebounce.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(300, token);
                if (!token.IsCancellationRequested)
                {
                    await Dispatcher.DispatchAsync(() => LoadFoodItemsAsync(e.NewTextValue));
                }
            }
            catch (TaskCanceledException)
            {
                // Debounce cancelled — expected.
            }
        }, token);
    }

    private async void OnSearchButtonPressed(object? sender, EventArgs e)
    {
        searchDebounce?.Cancel();
        await LoadFoodItemsAsync(SearchFoodBar.Text);
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadFoodItemsAsync(SearchFoodBar.Text);
        FoodRefreshView.IsRefreshing = false;
        var source = FoodCatalogService.LastLoadUsedMockApi ? "mockapi.io" : "local fallback data";
        SemanticScreenReader.Announce($"Food and drink list refreshed. Current source: {source}.");
    }
}
