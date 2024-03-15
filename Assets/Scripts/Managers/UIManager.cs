using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIManager Script
/// Is in charge of controlling the general state of the UI
/// In the case of this game, it includes references to the only two elements of the game
/// </summary>
public class UIManager : MonoBehaviour
{
    #region SINGLETON

    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject("GameManager");
                    _instance = singleton.AddComponent<UIManager>();
                }
            }
            return _instance;
        }
    }

    #endregion

    #region GENERAL REFERENCES

    // In this case and to simplify the variables are public and assigned by inspector
    // If necessary, they can be made private and with their respective Gets and Sets

    // Reference to the lobby panel
    public GameObject lobbyPanel;

    // Reference to the player's first-person HealthBar
    // (If the game were more complex, this reference would most likely end up in its own class or View)
    public Slider firstPersonHealthSlider;

    #endregion

    #region AWAKE and START

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // The game starts with the lobby panel active
    private void Start()
    {
        SwitchLobbyPanel(true);
    }

    #endregion

    #region METHODS

    // This method activates the lobby panel and disables the health bar in first perso
    public void SwitchLobbyPanel(bool value) 
    {
        lobbyPanel.SetActive(value);
        firstPersonHealthSlider.gameObject.SetActive(!value);
    }

    #endregion
}