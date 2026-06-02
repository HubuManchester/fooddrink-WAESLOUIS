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

    protected override void OnDisappearing()
    {
        searchDebounce?.Cancel();
        base.OnDisappearing();
    }

    private async Task LoadFoodItemsAsync(string? query = null)
    {
        LoadingBar.IsVisible = true;
        try
        {
            var items = await FoodCatalogService.SearchAsync(query);
            FoodCollection.ItemsSource = items;
            UpdateDashboard(items);
        }
        finally
        {
            LoadingBar.IsVisible = false;
        }
    }

    private void UpdateDashboard(IReadOnlyList<Models.FoodItem> items)
    {
        int totalCal = 0, totalProtein = 0, totalCarbs = 0, totalFat = 0;
        foreach (var item in items)
        {
            totalCal += item.Calories;
            totalProtein += item.Protein;
            totalCarbs += item.Carbs;
            totalFat += item.Fat;
        }

        DashCalories.Text = totalCal.ToString("N0");
        DashProtein.Text = totalProtein.ToString("N0");
        DashCarbs.Text = totalCarbs.ToString("N0");
        DashFat.Text = totalFat.ToString("N0");
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

    private async void OnDeleteInvoked(object? sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is string id)
        {
            var item = await FoodCatalogService.GetByIdAsync(id);
            var name = item?.Name ?? "this entry";

            bool confirm = await DisplayAlert(
                "Delete Record",
                $"Are you sure you want to delete \"{name}\"?",
                "Delete",
                "Cancel");

            if (confirm)
            {
                await FoodCatalogService.DeleteAsync(id);
                await LoadFoodItemsAsync(SearchFoodBar.Text);
                SemanticScreenReader.Announce($"{name} deleted successfully.");
            }
        }
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadFoodItemsAsync(SearchFoodBar.Text);
        FoodRefreshView.IsRefreshing = false;
        var source = FoodCatalogService.LastLoadUsedMockApi ? "mockapi.io" : "local fallback data";
        SemanticScreenReader.Announce($"Food and drink list refreshed. Current source: {source}.");
    }
}
