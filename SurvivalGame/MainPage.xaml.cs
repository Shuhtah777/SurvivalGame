using SurvivalGame.Models;
using SurvivalGame.Pages;

namespace SurvivalGame;

public partial class MainPage : ContentPage
{
    private Button[] storageButtons = Array.Empty<Button>();

    public MainPage()
    {
        InitializeComponent();

        storageButtons = new[]
        {
            Slot0, Slot1, Slot2,
            Slot3, Slot4, Slot5,
            Slot6, Slot7, Slot8
        };

        Dispatcher.StartTimer(TimeSpan.FromSeconds(3), () =>
        {
            GameState.UpdateSurvival();
            RefreshInventory();
            return true;
        });
    }

    private void OnUseMedkitClicked(object sender, EventArgs e)
    {
        StatusLabelControl.Text = GameState.UseMedkitFromBackpack();
        RefreshInventory();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshInventory();
    }

    private async void OnMedicalClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MedicalPage());
    }

    private async void OnForestClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ForestPage());

        RefreshInventory();
    }

    private void OnStoreAllClicked(object sender, EventArgs e)
    {
        StatusLabelControl.Text = GameState.StoreAll();
        RefreshInventory();
    }

    private void OnEatBerriesClicked(object sender, EventArgs e)
    {
        StatusLabelControl.Text = GameState.EatBerriesFromStorage();
        RefreshInventory();
    }

    private void OnStorageSlotClicked(object sender, EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not string value ||
            !int.TryParse(value, out int index))
        {
            return;
        }

        StatusLabelControl.Text = GameState.TakeFromStorage(index);
        RefreshInventory();
    }


    private void RefreshInventory()
    {
        HealthLabel.Text =
            $"❤️ Здоров'я: {GameState.Health} / 100";

        HungerLabel.Text =
            $"🍗 Голод: {GameState.Hunger} / 100";

        BackpackLabelControl.Text = GameState.BackpackDescription();

        for (int i = 0; i < storageButtons.Length; i++)
        {
            var item = GameState.Storage[i];

            storageButtons[i].Text = item == null
                ? $"Комірка {i + 1}\nПорожньо"
                : $"{item.Icon} {item.Name}\n× {item.Quantity}";

            storageButtons[i].LineBreakMode = LineBreakMode.WordWrap;
            storageButtons[i].Padding = new Thickness(2);
        }
    }

    private Label HealthLabelControl => this.FindByName<Label>("HealthLabel");
    private Label HungerLabelControl => this.FindByName<Label>("HungerLabel");
    private Label BackpackLabelControl => this.FindByName<Label>("BackpackLabel");
    private Label StatusLabelControl => this.FindByName<Label>("StatusLabel");
}
