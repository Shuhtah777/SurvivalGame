using SurvivalGame.Models;

namespace SurvivalGame.Pages;

public partial class BoarFightPage : ContentPage
{
    private readonly Random random = new();
    private int boarHealth = 3;
    private bool fightOver;

    public BoarFightPage()
    {
        InitializeComponent();
        RefreshHealth();
    }

    private void OnRockClicked(object sender, EventArgs e)
    {
        PlayRound("Камінь");
    }

    private void OnScissorsClicked(object sender, EventArgs e)
    {
        PlayRound("Ножиці");
    }

    private void OnPaperClicked(object sender, EventArgs e)
    {
        PlayRound("Папір");
    }

    private void PlayRound(string playerChoice)
    {
        if (fightOver)
            return;

        string[] choices = { "Камінь", "Ножиці", "Папір" };
        string boarChoice = choices[random.Next(choices.Length)];

        string result =
            $"Твій вибір: {playerChoice}\n" +
            $"Вибір кабана: {boarChoice}\n\n";

        if (playerChoice == boarChoice)
        {
            result += "?? Нічия! Ніхто не отримав шкоди.";
        }
        else if (
            (playerChoice == "Камінь" && boarChoice == "Ножиці") ||
            (playerChoice == "Ножиці" && boarChoice == "Папір") ||
            (playerChoice == "Папір" && boarChoice == "Камінь"))
        {
            boarHealth--;
            result += "?? Ти переміг у раунді! Кабан втрачає 1 здоров'я.";
        }
        else
        {
            GameState.DamagePlayer(10);
            result += "?? Кабан переміг у раунді! Ти втрачаєш 10 здоров'я.";
        }

        RefreshHealth();

        if (boarHealth <= 0)
        {
            fightOver = true;
            GameState.AddSkillPoint();
            result += "\n\n?? Кабан утік! Ти переміг! Отримано 1 очко розвитку!";

        }
        else if (GameState.Health <= 0)
        {
            fightOver = true;
            result += "\n\n?? Ти більше не можеш продовжувати бій.";
        }

        ResultLabel.Text = result;
    }
    private void OnUseMedkitClicked(object sender, EventArgs e)
    {
        if (fightOver)
        {
            ResultLabel.Text = "Бій уже завершено.";
            
            return;
        }

        ResultLabel.Text = GameState.UseMedkitFromBackpack();
        RefreshHealth();
    }

    private void RefreshHealth()
    {
        PlayerHealthLabel.Text =
            $"?? Твоє здоров'я: {GameState.Health} / 100";

        BoarHealthLabel.Text =
            $"?? Здоров'я кабана: {boarHealth} / 3";
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
