
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurvivalGame.Models;

public class InventoryItem
{
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public int Quantity { get; set; }

    public InventoryItem Clone()
    {
        return new InventoryItem
        {
            Name = Name,
            Icon = Icon,
            Quantity = Quantity
        };
    }
}

public static class GameState
{
    private static readonly Random random = new();

    public static Dictionary<string, InventoryItem> Backpack { get; } = new();

    public static InventoryItem?[] Storage { get; } = new InventoryItem?[9];

    public static int Health { get; private set; } = 100;
    public static int Hunger { get; private set; } = 100;

    public static int SkillPoints { get; private set; } = 3;

    public static int LiteracyLevel { get; private set; } = 0;
    public static int CamouflageLevel { get; private set; } = 0;
    public static int LuckLevel { get; private set; } = 0;

    public static string UpgradeSkill(string skill)
    {
        if (SkillPoints <= 0)
            return "❌ Немає очок розвитку!";

        switch (skill)
        {
            case "Начитаність":
                if (LiteracyLevel >= 5)
                    return "📚 Начитаність уже максимальна!";

                LiteracyLevel++;
                break;

            case "Маскування":
                if (CamouflageLevel >= 5)
                    return "🌿 Маскування вже максимальне!";

                CamouflageLevel++;
                break;

            case "Успіх":
                if (LuckLevel >= 5)
                    return "🍀 Успіх уже максимальний!";

                LuckLevel++;
                break;

            default:
                return "Невідома навичка.";
        }

        SkillPoints--;

        return $"✨ Навичку «{skill}» покращено!";
    }

    public static void AddSkillPoint()
    {
        SkillPoints++;
    }

    public static string SearchRandom()
    {
        if (random.Next(100) < 35)
            return "🔎 Нічого не знайдено.";

        string result;
        int item = random.Next(3);

        switch (item)
        {
            case 0:
                int berries = random.Next(1, 4);
                AddToBackpack("Ягоди", "🫐", berries);
                result = $"🫐 Знайдено ягід: {berries}.";
                break;

            case 1:
                int herbs = random.Next(1, 3);
                AddToBackpack("Лікарські трави", "🌿", herbs);
                result = $"🌿 Знайдено лікарських трав: {herbs}.";
                break;

            default:
                int wood = random.Next(1, 4);
                AddToBackpack("Деревина", "🪵", wood);
                result = $"🪵 Знайдено деревини: {wood}.";
                break;
        }

        if (LuckLevel > 0 &&
            random.Next(100) < LuckLevel * 10)
        {
            int bonus = random.Next(3);

            switch (bonus)
            {
                case 0:
                    AddToBackpack("Ягоди", "🫐", 1);
                    result += "\n🍀 Бонусний лут: +1 ягода!";
                    break;

                case 1:
                    AddToBackpack("Лікарські трави", "🌿", 1);
                    result += "\n🍀 Бонусний лут: +1 лікарська трава!";
                    break;

                default:
                    AddToBackpack("Деревина", "🪵", 1);
                    result += "\n🍀 Бонусний лут: +1 деревина!";
                    break;
            }
        }

        return result;
    }

    public static bool TryEncounterBoar()
    {
        int chance = Math.Max(1, 8 - CamouflageLevel);
        return random.Next(100) < chance;
    }

    public static void DamagePlayer(int damage)
    {
        Health = Math.Max(0, Health - damage);
    }


    public static bool TryStartMedkitCrafting(
        out int herbCost, out string message)
    {
        herbCost = 0;

        if (!Backpack.TryGetValue("Лікарські трави", out var herbs)
            || herbs.Quantity < 5)
        {
            message = "🌿 Потрібно щонайменше 5 лікарських трав.";
            return false;
        }

        // Витрачаємо випадково від 5 до 10 трав,
        // але не більше, ніж є в рюкзаку.
        int maxCost = Math.Min(10, herbs.Quantity);
        herbCost = random.Next(5, maxCost + 1);

        herbs.Quantity -= herbCost;

        if (herbs.Quantity == 0)
            Backpack.Remove("Лікарські трави");

        message = $"Витрачено лікарських трав: {herbCost}.";
        return true;
    }

    public static void FinishMedkitCrafting(bool success)
    {
        if (success)
        {
            AddToBackpack("Аптечка", "🩹", 1);
        }
    }

    public static string UseMedkitFromBackpack()
    {
        if (Health >= 100)
            return "❤️ Здоров'я вже повне!";

        if (!Backpack.TryGetValue("Аптечка", out var medkit)
            || medkit.Quantity <= 0)
        {
            return "🩹 У рюкзаку немає аптечок!";
        }

        medkit.Quantity--;

        if (medkit.Quantity == 0)
            Backpack.Remove("Аптечка");

        int oldHealth = Health;
        Health = Math.Min(100, Health + 20);

        return $"🩹 Використано аптечку! Здоров'я: {oldHealth} → {Health}.";
    }



    public static void AddToBackpack(string name, string icon, int quantity)
    {
        const int backpackLimit = 16;

        if (Backpack.TryGetValue(name, out var item))
        {
            int freeSpace = backpackLimit - item.Quantity;
            int added = Math.Min(quantity, freeSpace);

            if (added > 0)
                item.Quantity += added;
        }
        else
        {
            int added = Math.Min(quantity, backpackLimit);

            if (added > 0)
            {
                Backpack[name] = new InventoryItem
                {
                    Name = name,
                    Icon = icon,
                    Quantity = added
                };
            }
        }
    }


    public static string BackpackDescription()
    {
        if (Backpack.Count == 0)
            return "Рюкзак порожній.";

        return string.Join(
            "\n",
            Backpack.Values.Select(item =>
                $"{item.Icon} {item.Name} × {item.Quantity}"));
    }


    public static string StoreAll()
    {
        const int storageLimit = 32;
        int stored = 0;

        foreach (var item in Backpack.Values.ToList())
        {
            int remaining = item.Quantity;

            // Спочатку шукаємо комірку з таким самим предметом,
            // у якій ще є вільне місце.
            int index = Array.FindIndex(
                Storage,
                slot => slot != null
                    && slot.Name == item.Name
                    && slot.Quantity < storageLimit);

            // Якщо такої немає — шукаємо порожню комірку.
            if (index == -1)
            {
                index = Array.FindIndex(
                    Storage,
                    slot => slot == null);
            }

            if (index == -1)
                continue;

            if (Storage[index] == null)
            {
                int added = Math.Min(remaining, storageLimit);

                Storage[index] = new InventoryItem
                {
                    Name = item.Name,
                    Icon = item.Icon,
                    Quantity = added
                };

                remaining -= added;
                stored += added;
            }
            else
            {
                int freeSpace = storageLimit - Storage[index]!.Quantity;
                int added = Math.Min(remaining, freeSpace);

                Storage[index]!.Quantity += added;
                remaining -= added;
                stored += added;
            }

            if (remaining == 0)
                Backpack.Remove(item.Name);
            else
                item.Quantity = remaining;
        }

        if (stored == 0)
            return "Немає місця на складі або немає ресурсів для зберігання.";

        return $"На склад перенесено предметів: {stored}.";
    }


    public static string TakeFromStorage(int index)
    {
        if (index < 0 || index >= Storage.Length)
            return "Неправильний номер комірки.";

        var item = Storage[index];

        if (item == null)
            return "Ця комірка порожня.";

        AddToBackpack(item.Name, item.Icon, item.Quantity);

        string name = item.Name;
        int quantity = item.Quantity;

        Storage[index] = null;

        return $"Забрано: {name} × {quantity}.";
    }


    public static void UpdateSurvival()
    {
        Hunger = Math.Max(0, Hunger - 1);

        if (Hunger == 0)
        {
            Health = Math.Max(0, Health - 2);
        }
    }

    public static string EatBerriesFromStorage()
    {
        const string berryName = "Ягоди";

        int index = Array.FindIndex(
            Storage,
            item => item != null
                && item.Name == berryName
                && item.Quantity > 0);

        if (index == -1)
        {
            return "🫐 На складі немає ягід!";
        }

        Storage[index]!.Quantity--;

        if (Storage[index]!.Quantity == 0)
        {
            Storage[index] = null;
        }

        Hunger = Math.Min(100, Hunger + 20);

        return "🫐 Ти з'їв ягоди. Голод зменшився!";
    }

}
