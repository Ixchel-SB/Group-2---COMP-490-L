using UnityEngine;
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Languages : MonoBehaviour
{
    [System.Serializable]
    public struct LanguageButton
    {
        public Button button;
        public Locale locale;
    }

    // Assign these in the inspector
    public LanguageButton[] languageButtons;

    IEnumerator Start()
    {
        // Wait for Localization to initialize
        yield return LocalizationSettings.InitializationOperation;

        // Load saved language (or use device default)
        LoadSavedLanguage();

        // Set up button click events
        foreach (var langBtn in languageButtons)
        {
            if (langBtn.button == null || langBtn.locale == null)
                continue;

            // create local copy to avoid closure capture issues
            var localeCopy = langBtn.locale;
            langBtn.button.onClick.AddListener(() => ChangeLanguage(localeCopy));
        }
    }

    void LoadSavedLanguage()
    {
        // 1. Try to load saved language
        string savedLangCode = PlayerPrefs.GetString("SelectedLanguage", "");

        if (!string.IsNullOrEmpty(savedLangCode))
        {
            Locale savedLocale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(savedLangCode));
            if (savedLocale != null)
            {
                LocalizationSettings.SelectedLocale = savedLocale;
                Debug.Log("Loaded saved language: " + savedLangCode);
                return;
            }
        }

        // 2. Use device language
        Locale deviceLocale = LocalizationSettings.AvailableLocales.GetLocale(Application.systemLanguage);
        if (deviceLocale != null)
        {
            LocalizationSettings.SelectedLocale = deviceLocale;
            Debug.Log("Using device language: " + Application.systemLanguage);
        }
        else
        {
            // 3. First available Locale (fallback)
            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (locales != null && locales.Count > 0)
            {
                LocalizationSettings.SelectedLocale = locales[0];
                Debug.LogWarning("Using default language (first available).");
            }
            else
            {
                Debug.LogError("No available locales found in LocalizationSettings.");
            }
        }
    }

    void ChangeLanguage(Locale targetLocale)
    {
        if (targetLocale == null)
        {
            Debug.LogWarning("ChangeLanguage called with null locale.");
            return;
        }

        LocalizationSettings.SelectedLocale = targetLocale;
        PlayerPrefs.SetString("SelectedLanguage", targetLocale.Identifier.Code);
        PlayerPrefs.Save(); // Force save
        Debug.Log("Language saved: " + targetLocale.Identifier.Code);
    }
}
