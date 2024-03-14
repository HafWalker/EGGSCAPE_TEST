using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIManager Script
/// Es el encargado de controlar el estado general de la UI
/// En el caso de este juego contempla referencias a los dos unicos elementos del juego
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
    // En este caso y para simplificar las variables son publicas y asignadas por inspector
    // En caso de ser necesario se pueden volver privadas y con sus respectivos Gets y Sets

    // Referencia al panel de lobby
    public GameObject lobbyPanel;

    // Referencia a la barra de vida en primera persona del jugador
    // (Si el juego fuera mas complejo, lo mas probables es que esta referencia termine en su propia clase o View)
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

    // El juego inicia con el panel de lobby activo
    private void Start()
    {
        SwitchLobbyPanel(true);
    }

    #endregion

    #region METHODS

    // Este metodo activa el panel de lobby y desactiva el healthBar en primera persona y visebersa
    public void SwitchLobbyPanel(bool value) 
    {
        lobbyPanel.SetActive(value);
        firstPersonHealthSlider.gameObject.SetActive(!value);
    }

    #endregion
}