using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using System.Collections;

public class LoaderUI : MonoBehaviour
{
    private Loader loader;

    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI errorText;

    [SerializeField] private GameObject error;

    private PlayerProfile playerProfile;


    private void Start()
    {
        playerProfile = PlayerDataManager.Instance.PlayerProfile;
        StartCoroutine(WaitForLoader());
    }
    public void OnClickLogin()
    {
        if (ErroroLogin())
        {
            playerProfile.GetNewProfile(playerNameInput.text, passwordInput.text);
            errorText.text = "";
            loader.Loading();
        }
        else
        {
            error.SetActive(true);
        }
    }
    public void OnErrorClose()
    {
        error.SetActive(false);
    }

    private bool ErroroLogin()
    {
        string username = playerNameInput.text;
        string password = passwordInput.text;

        // Проверка длины
        if (username.Length > 15)
        {
            errorText.text = "Логин не должен превышать 15 символов.";
            return false;
        }
        if (password.Length > 15)
        {
            errorText.text = "Пароль не должен превышать 15 символов.";
            return false;
        }

        // Проверка на допустимые символы (только буквы и цифры)
        Regex validChars = new Regex("^[a-zA-Z0-9]+$");

        if (!validChars.IsMatch(username))
        {
            errorText.text = "Логин содержит недопустимые символы.";
            return false;
        }

        if (!validChars.IsMatch(password))
        {
            errorText.text = "Пароль содержит недопустимые символы.";
            return false;
        }

        return true;
    }
    private IEnumerator WaitForLoader()
    {
        
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Loader") != null);

        GameObject loaderObj = GameObject.FindGameObjectWithTag("Loader");
        loader = loaderObj.GetComponent<Loader>();

        if (loader == null)
        {
            Debug.LogError("Компонент 'Loader' не найден на объекте с тегом 'Loader'!");
        }
        else
        {
            Debug.Log("Loader найден и сохранён.");
        }
    }
}

