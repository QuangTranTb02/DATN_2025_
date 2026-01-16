using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string nameEssentialScene;
    [SerializeField] string nameNewGameStartScene;

    [SerializeField] PlayerData playerData;

    public Gender selectedGender;
    public TMP_InputField nameInputField;
    public TMP_Text GenderText;

    private void Start()
    {
        SetGenderFemale();
        UpdateName();
    }
    public void ExitGame() 
    { 
        Application.Quit();
        Debug.Log("Quit Game");
    }
    public void StartNewGame()
    {
        SceneManager.LoadScene(nameNewGameStartScene, LoadSceneMode.Single);
        SceneManager.LoadScene(nameEssentialScene, LoadSceneMode.Additive);
        
    }
    public void SetGenderMale()
    {
        selectedGender = Gender.Male;
        playerData.characterGender = selectedGender;
        GenderText.text = "Male";
    }
    public void SetGenderFemale()
    {
        selectedGender = Gender.Female;
        playerData.characterGender = selectedGender;
        GenderText.text = "Female";
    }
    public void UpdateName()
    {
        playerData.characterName = nameInputField.text;
    }
    public void SetSavingSlot(int num)
    {
        playerData.saveSlotId = num;
    }
}
