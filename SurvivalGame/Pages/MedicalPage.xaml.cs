
using SurvivalGame.Models;

namespace SurvivalGame.Pages;

public partial class MedicalPage : ContentPage
{
    private readonly Random random = new();
    private bool craftingActive;
    private int targetValue;

    public MedicalPage()
    {
        InitializeComponent();
        RefreshInventory();
    }
    private void OnUpgradeLiteracyClicked(object sender, EventArgs e)
    {
        ResultLabel.Text = GameState.UpgradeSkill("Начитаність");
        RefreshInventory();
    }

    private void OnUpgradeCamouflageClicked(object sender, EventArgs e)
    {
        ResultLabel.Text = GameState.UpgradeSkill("Маскування");
        RefreshInventory();
    }

    private void OnUpgradeLuckClicked(object sender, EventArgs e)
    {
        ResultLabel.Text = GameState.UpgradeSkill("Успіх");
        RefreshInventory();
    }

    private void OnStartCraftingClicked(object sender, EventArgs e)
    {
        if (craftingActive)
            return;

        if (!GameState.TryStartMedkitCrafting(
            out int cost, out string message))
        {
            ResultLabel.Text = message;
            RefreshInventory();
            return;
        }

        craftingActive = true;
        targetValue = random.Next(20, 81);

        SkillTargetLabel.Text = $"?? Ціль: {targetValue}";
        SkillSlider.Value = 50;

        SkillTargetLabel.IsVisible = true;
        SkillSlider.IsVisible = true;
        SkillHintLabel.IsVisible = true;
        CheckButton.IsVisible = true;
        StartButton.IsEnabled = false;

        int tolerance = 8 + GameState.LiteracyLevel * 3;

        ResultLabel.Text =
            $"{message}\n" +
            $"📚 Допустиме відхилення: ±{tolerance}\n" +
            "Налаштуй повзунок і перевір точність!";


        RefreshInventory();
    }

    private void OnCheckSkillClicked(object sender, EventArgs e)
    {
        if (!craftingActive)
            return;

        bool success =
            Math.Abs(SkillSlider.Value - targetValue)
                <= 8 + GameState.LiteracyLevel * 3;

        GameState.FinishMedkitCrafting(success);

        craftingActive = false;

        StartButton.IsEnabled = true;
        SkillTargetLabel.IsVisible = false;
        SkillSlider.IsVisible = false;
        SkillHintLabel.IsVisible = false;
        CheckButton.IsVisible = false;

        ResultLabel.Text = success
            ? "?? Успіх! Ти виготовив аптечку ??"
            : "? Не вдалося! Трави витрачено, аптечку не отримано.";

        RefreshInventory();
    }

    private void OnUseMedkitClicked(object sender, EventArgs e)
    {
        ResultLabel.Text = GameState.UseMedkitFromBackpack();
        RefreshInventory();
    }


    private void RefreshInventory()
    {
        InventoryLabel.Text =
            $"❤️ Здоров'я: {GameState.Health} / 100\n" +
            $"🎒 Рюкзак:\n{GameState.BackpackDescription()}";

        SkillPointsLabel.Text =
            $"⭐ Очки розвитку: {GameState.SkillPoints}";

        LiteracyLabel.Text =
            $"📚 Начитаність: {GameState.LiteracyLevel}/5\n" +
            $"Допустиме відхилення: ±{8 + GameState.LiteracyLevel * 3}";

        CamouflageLabel.Text =
            $"🌿 Маскування: {GameState.CamouflageLevel}/5\n" +
            $"Шанс зустрічі з кабаном: {Math.Max(1, 8 - GameState.CamouflageLevel)}%";

        LuckLabel.Text =
            $"🍀 Успіх: {GameState.LuckLevel}/5\n" +
            $"Шанс додаткового луту: {GameState.LuckLevel * 10}%";
    }


    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
