
using SurvivalGame.Models;

namespace SurvivalGame.Pages;

public partial class ForestPage : ContentPage
{
    public ForestPage()
    {
        InitializeComponent();
        RefreshInventory();
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        if (GameState.TryEncounterBoar())
        {
            await Navigation.PushAsync(new BoarFightPage());
            return;
        }

        ResultLabel.Text = GameState.SearchRandom();
        RefreshInventory();
    }

    private void OnUseMedkitClicked(object sender, EventArgs e)
    {
        ResultLabel.Text = GameState.UseMedkitFromBackpack();
        RefreshInventory();
    }

    private void RefreshInventory()
    {
        BackpackLabel.Text = GameState.BackpackDescription();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
