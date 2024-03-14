using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// PlayerController Scrip
/// Este escript es el responsable de controllar los movimientos y acciones del Jugador
/// Contiene ademas una variables sincronizada de Health
/// </summary>
public class PlayerController : NetworkBehaviour, IDamageable
{
    #region HEALTH

    [SyncVar] public float currentHealth; // Valor de la vida actual del jugador sincronizada

    [SerializeField] 
    private float maxHealt = 10f; // valor maximo por defecto de la vida del jugador

    public Slider thirdPersonHealthSlider; // Referencia la Slider en 3ra persona

    #endregion

    #region MOVEMENT VARIABLES

    [SerializeField]
    private float movementSpeed = 10f; // Velocidad de movimiento del jugador

    [SerializeField]
    private float rotationSpeed = 5f; // Velocidad de rotacion de la camara del jugador (Transfiere su rotacion al propio transform en el eje Z)

    #endregion

    #region PUBLIC REFERENCES

    public Transform cameraContainer;
    
    public GameObject mainCamera;

    public CharacterController characterController;

    public Sword sword;

    #endregion

    #region PRIVATE VARIABLES

    private float rotX = 0.0f;

    private bool isClientInit = false;
    
    private PlayerNetworkSync playerNetworkSync;

    #endregion

    #region FISHNET CLIENT EVENTS

    // Evento de FishNet al iniciar el Cliente
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            // FishNet me indica si este objeto es o no el Owner del Cliente
            // Para evitar controlar a otras instancias de jugadores, desactivo el script
            GetComponent<PlayerController>().enabled = false;
        }
        else {
            // En caso de ser este objeto el Owner del Cliente
            // Esta bandera es necesaria para prevenir que se ejecute el update antes de que este listo el cliente
            isClientInit = true;

            // Obtengo la referencia de la mainCamera del GameManager y se la asigno a el jugador
            mainCamera = GameManager.Instance.mainCamera;
            mainCamera.transform.parent = cameraContainer;
            mainCamera.transform.position = cameraContainer.position;
            mainCamera.transform.rotation = cameraContainer.rotation;

            // Get del componente PlayerNetworkSync
            playerNetworkSync = GetComponent<PlayerNetworkSync>();

            // Asignacion de un nombre al jugador en base a su Id (Para distinguir los clones)
            transform.name = "Player_1" + base.OwnerId.ToString();

            // Seteo la vida actual del jugador a su valor maximo
            currentHealth = maxHealt;

            // LLamada al UIManager para intercambiar el panel de lobby a la UI del jugador
            UIManager.Instance.SwitchLobbyPanel(false);
        }
    }

    // Evento de FishNet al detener el Cliente
    public override void OnStopClient()
    {
        base.OnStopClient();
        if (base.IsOwner)
        {
            // En caso de que se detenga el Cliente por alguna razon
            // Desactivo la bandera para evitar que el update siga funcionando
            isClientInit = false;

            // Se obtiene la referencia del Gizmo en el GameManager y se asigna la camara
            mainCamera.transform.parent = GameManager.Instance.hoverCameraGizmo;
            mainCamera.transform.position = GameManager.Instance.hoverCameraGizmo.position;
            mainCamera.transform.rotation = GameManager.Instance.hoverCameraGizmo.rotation;

            // LLamada al UIManager para intercambiar al lobbyPanel
            UIManager.Instance.SwitchLobbyPanel(true);
        }
    }

    #endregion

    private void Start()
    {
        // Al inicio del script obtengo la referencia del CharacterController
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {

        #region CLIENT CHECK

        // Verificacion de bandera para asegurarme de que no se ejecute el update antes de que el Cliente este listo
        if (!isClientInit)
            return;

        #endregion

        #region PLAYER MOVEMENT & VIEW

        // Obtengo los valores de X e Y del mouse para asigna rotacion a la camamara en primera persona
        float rotY = Input.GetAxis("Mouse X") * rotationSpeed;
        rotX -= Input.GetAxis("Mouse Y") * rotationSpeed;

        rotX = Mathf.Clamp(rotX, -90f, 90f);

        transform.Rotate(0, rotY, 0);
        mainCamera.transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // Creo dos variables horizontal y vertical que van a servir de vector direccion para el movimiento del jugador
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.W))
            verticalInput = 1f;
        if (Input.GetKey(KeyCode.S))
            verticalInput = -1f;
        if (Input.GetKey(KeyCode.D))
            horizontalInput = 1f;
        if (Input.GetKey(KeyCode.A))
            horizontalInput = -1f;

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= movementSpeed;

        // Con el vector direccion del jugador y su velocidad, desplazo el CharacterController
        characterController.Move(moveDirection * Time.deltaTime);

        #endregion

        #region PLAYER ATTACK

        // Detectando la tecla "Espacio" (SpaceBar) sincronizo el ataque del jugador con el servidor
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Ejecuto el ataque localmente
            LocalAttack(true);

            // Aviso al servidor que se realizo el ataque y envio una referencia del Tick para sincronizar con el resto de los clientes
            playerNetworkSync.AttackServer(this, true, TimeManager.Tick);
        }
        else if (Input.GetKeyUp(KeyCode.Space)) {

            // Cancelo el ataque localmente
            LocalAttack(false);

            // Aviso al servidor de la cancelacion del ataque y envio una referencia del Tick para sincronizar con el resto de los clientes
            playerNetworkSync.AttackServer(this, false, TimeManager.Tick);
        }

        #endregion
    }

    // Metodo para realizar el ataque local
    public void LocalAttack(bool value) 
    {
        // LLamada al metodo "Attack" del arma
        sword.Attack(value);
    }

    // Este metodo es llamado por el servidor en los Cliente para ejecutar el ataque
    // El metodo recibe un timeDiff que se utiliza en el arma para predecir la diferencia de tiempo
    // entre la ejecucion del ataque en el Cliente original y su replica en el resto de los clientes
    public void PerformAttackFromServer(PlayerController player, bool value, float timeDiff) 
    {
        // Llamada al metodo de ataque predictivo del arma
        sword.AttackPredict(value, timeDiff);
    }

    // Este metodo se encarga de procesar el daño rebicido por el jugador de forma local
    public void Takedamage(GameObject weapon, float damage)
    {
        // Antes que nada se verifica que el arma atacante no es la propia del jugador
        if (weapon.transform != sword.transform)
        {

            // Disminucion de la variable sincronizada Health
            currentHealth -= damage;

            // Verificacion de vida por debajo de 0
            if (currentHealth <= 0)
            {
                currentHealth = 0;
            }

            // Actualizacion local de la barra de vida en primera persona
            float healthSliderValue = currentHealth / maxHealt;
            UIManager.Instance.firstPersonHealthSlider.value = healthSliderValue;

            // Notificacion al servidor sobre el cambio
            playerNetworkSync.UpdateHealth(this, currentHealth);
        }
    }

    // Este metodo actualiza la barra de vida en tercera persona para el resto de los jugadores
    public void UpdatePlayerHealth() 
    {
        float healthSliderValue = currentHealth / maxHealt;

        // Asignacion del valor del slider en 3ra persona en funcion de la vida actual del jugador
        thirdPersonHealthSlider.value = healthSliderValue;
    }

    // Reseteo del Health del jugador
    public void ResetHealth()
    {
        // Reseteo local
        currentHealth = maxHealt;

        // Notificacion al servidor sobre el cambio
        playerNetworkSync.UpdateHealth(this, maxHealt);
    }

    // Metodo que controla la situacion en caso de que el jugador pierda toda su vida
    public void PlayerDead() 
    {
        // Muevo la camara del Jugador al GameManager
        mainCamera.transform.parent = GameManager.Instance.hoverCameraGizmo;
        mainCamera.transform.position = GameManager.Instance.hoverCameraGizmo.position;
        mainCamera.transform.rotation = GameManager.Instance.hoverCameraGizmo.rotation;

        // Intercambio al panel de lobby
        UIManager.Instance.SwitchLobbyPanel(true);

        // Desactivo este GameObject (El jugador)
        this.gameObject.SetActive(false);
    }
}