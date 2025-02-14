using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSetupUI : MonoBehaviour
{
    [SerializeField]
    private GameObject gameSetupUIArea;

    [SerializeField]
    private TextMeshProUGUI moneyText;

    [SerializeField]
    private TMP_InputField seedInputField;
    [SerializeField]
    private TMP_InputField nameInputField;

    [SerializeField]
    private GuildToggleGroupUI guildPrefabUI;

    [SerializeField]
    private TraitSelectionManagerUI traitSelectionManagerUI;

    private string characterName;
    private int money;
    private List<Item> inventoryItems;
    private Guild guild;
    private int seed;
    private List<Trait> traits;

    private GuildManager currentGuildManager;

    /// <summary>
    /// Shows the player game setup screen.
    /// </summary>
    public void Show()
    {
        gameSetupUIArea.SetActive(true);

        // Show initial values
        SetupValues();
    }

    public void Hide()
    {
        gameSetupUIArea.SetActive(false);
    }

    public void StartButton()
    {
        if (TryCreatePlayerFromInputs(out Player createdPlayer) == false)
        {
            return;
        }

        GameInitializer.Instance.InitializeGame(
            createdPlayer, seed, currentGuildManager);
        
        Hide();
    }

    /// <summary>
    /// Input from name UI input field.
    /// </summary>
    /// <param name="characterName"></param>
    public void InputCharacterName(string characterName)
    {
        this.characterName = characterName;

        CreateTraits();
    }

    /// <summary>
    /// Input from seed UI input field. Converts string to int
    /// </summary>
    /// <param name="seedText"></param>
    public void InputSeed(string seedText)
    {
        System.Security.Cryptography.MD5 md5Hasher =
            System.Security.Cryptography.MD5.Create();

        byte[] hashed = md5Hasher.ComputeHash(
            System.Text.Encoding.UTF8.GetBytes(seedText));

        int seed = System.BitConverter.ToInt32(hashed, 0);
        this.seed = seed;

        CreateGuilds();
    }

    public void RandomSeedButton()
    {
        seed = Random.Range(-1000000, 1000000);
        // display seed in input box
        seedInputField.text = seed.ToString();

        CreateGuilds();
    }

    public void RandomNameButton()
    {
        characterName = NameGenerator.GenerateName();

        // display seed in input box
        nameInputField.text = characterName;

        CreateTraits();
    }

    private bool TryCreatePlayerFromInputs(out Player p)
    {
        p = null;

        if (characterName == null ||
            guild == null)
        {
            Debug.Log("Incomplete character");
            return false;
        }

        // Create blank inventory list if no starting items
        if (inventoryItems == null)
        {
            inventoryItems = new List<Item>();
        }

        // Get player selected traits
        traits = traitSelectionManagerUI.GetTraits();

        p = new Player(
            EntityType.Player,
            inventoryItems,
            money,
            VisibilityLevel.Visible,
            "player",
            characterName,
            guild,
            traits);

        return true;
    }

    private void SetupValues()
    {
        money = 100;
        UpdateMoneyTextDisplay();

        seed = Random.Range(-10000, 10000);
        UpdateSeedTextDisplay();

        CreateGuilds();

        RandomNameButton();
    }

    private void CreateGuilds()
    {
        currentGuildManager = new GuildManager(seed);
        UpdateGuildManagerDisplay();
    }

    private void CreateTraits()
    {
        System.Security.Cryptography.MD5 md5Hasher =
            System.Security.Cryptography.MD5.Create();

        byte[] hashed = md5Hasher.ComputeHash(
            System.Text.Encoding.UTF8.GetBytes(characterName));

        int traitSeed = System.BitConverter.ToInt32(hashed, 0);

        traits = traitSelectionManagerUI.GenerateTraits(traitSeed);
        UpdateTraitDisplay();
    }

    public void SelectedGuild(Guild selectedGuild)
    {
        guild = selectedGuild;
    }

    private void UpdateGuildManagerDisplay()
    {
        guildPrefabUI.DisplayGuilds(currentGuildManager.guilds, this);
    }

    private void UpdateTraitDisplay()
    {
        traitSelectionManagerUI.Display();
    }

    private void UpdateSeedTextDisplay()
    {
        seedInputField.text = seed.ToString();
    }

    private void UpdateMoneyTextDisplay()
    {
        moneyText.SetText("$" + money.ToString());
    }
}
