using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System;
using UnityEditor.PackageManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// PlayerController Scrip
/// This script is responsible for controlling the movements and actions of the Player
/// Also contains a synchronized Health variable and implements the IDamageable interface
/// </summary>
public class PlayerController : NetworkBehaviour, IDamageable
{
    #region HEALTH VARIABLES

    // This currentHealth var is synchronized in the server
    // The parrameters ares mostly default but I add an callback method to update de health view
    [SyncVar(Channel = Channel.Unreliable, ReadPermissions = ReadPermission.Observers, SendRate = 0.1f,  OnChange = nameof(OnHealthChange))] 
    public float currentHealth;

    [SerializeField] 
    private float maxHealt = 10f; // The max value of Health

    public Slider thirdPersonHealthSlider; // 3rd person health slider reference

    #endregion

    #region MOVEMENT VARIABLES

    [SerializeField]
    private float movementSpeed = 10f; // The player movement speed

    [SerializeField]
    private float rotationSpeed = 5f; // Camera rotation speed (This is used for transform Z rotation)

    #endregion

    #region PUBLIC REFERENCES

    public Transform cameraContainer; // Camera container Reference inside the Player Trasnform

    public Sword sword; // Sword weapon script reference

    #endregion

    #region PRIVATE VARIABLES

    private float rotX = 0.0f; // Variable to keep the clamped mouse rotation values in X axis

    private bool isClientInit = false; // Flag to verify if the client is started
    
    private PlayerNetworkSync playerNetworkSync; // PlayerNetworkSync script reference

    private CharacterController characterController; // Character Controller reference

    private GameObject mainCamera; // Main Camera Reference
    
    #endregion

    #region FISHNET CLIENT EVENTS

    // FishNet event on start Client
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            // FishNet tells me whether or not this object is the Owner of the Client
            // To avoid controlling other player instances, I disable the script
            GetComponent<PlayerController>().enabled = false;
        }
        else {
            // If this object is the Owner of the Client
            // This flag is necessary to prevent the update from being executed before the client is ready
            isClientInit = true;

            // Get the mainCamera reference from the GameManager and assign it to the player
            mainCamera = GameManager.Instance.mainCamera;
            mainCamera.transform.parent = cameraContainer;
            mainCamera.transform.position = cameraContainer.position;
            mainCamera.transform.rotation = cameraContainer.rotation;

            // Get del componente PlayerNetworkSync
            playerNetworkSync = GetComponent<PlayerNetworkSync>();

            // Assigning a name to the player based on their ID (To distinguish clones)
            transform.name = "Player_1" + base.OwnerId.ToString();

            // Set the player's current life to its maximum value
            currentHealth = maxHealt;

            // Call to UIManager to swap the lobby panel to the player UI
            UIManager.Instance.SwitchLobbyPanel(false);
        }
    }

    // FishNet event when stopping the Client
    public override void OnStopClient()
    {
        base.OnStopClient();
        if (base.IsOwner)
        {
            // In case the Client stops for some reason
            // I disable the flag to prevent the update from continuing to work
            isClientInit = false;

            // The Gizmo reference is obtained in the GameManager and the camera is assigned
            mainCamera.transform.parent = GameManager.Instance.hoverCameraGizmo;
            mainCamera.transform.position = GameManager.Instance.hoverCameraGizmo.position;
            mainCamera.transform.rotation = GameManager.Instance.hoverCameraGizmo.rotation;

            // Call to UIManager to exchange lobbyPanel
            UIManager.Instance.SwitchLobbyPanel(true);
        }
    }

    #endregion

    #region START ADN UPDATED

    private void Start()
    {
        // At the beginning of the script I get the reference of the CharacterController
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        #region CLIENT CHECK

        // Flag check to make sure the update is not run before the Client is ready
        if (!isClientInit)
            return;

        #endregion

        #region PLAYER MOVEMENT & VIEW

        // I get the X and Y values ​​of the mouse to assign rotation to the camera in first person
        float rotY = Input.GetAxis("Mouse X") * rotationSpeed;
        rotX -= Input.GetAxis("Mouse Y") * rotationSpeed;

        rotX = Mathf.Clamp(rotX, -90f, 90f);

        transform.Rotate(0, rotY, 0);
        mainCamera.transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // I create two horizontal and vertical variables that will serve as direction vector for the player movement
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

        // With the direction vector of the player and his speed, I move the CharacterController
        characterController.Move(moveDirection * Time.deltaTime);

        #endregion

        #region PLAYER ATTACK

        // Detecting the "Space" key (SpaceBar) I synchronize the player's attack with the server
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Execute the attack locally
            LocalAttack(true);

            // Notification to the server that the attack was carried out and sent a Tick reference to synchronize with the rest of the clients
            playerNetworkSync.AttackServer(this, true, TimeManager.Tick);
        }
        else if (Input.GetKeyUp(KeyCode.Space)) {

            // Cancel the attack locally
            LocalAttack(false);

            // Notify the server of the cancellation of the attack and send a Tick reference to synchronize with the rest of the clients
            playerNetworkSync.AttackServer(this, false, TimeManager.Tick);
        }

        #endregion
    }

    #endregion

    #region METHODS

    // Method to carry out the local attack
    public void LocalAttack(bool value) 
    {
        // Call to the "Attack" method of the weapon
        sword.Attack(value);
    }

    // This method is called by the Server on the Client to execute the attack
    // The method receives a timeDiff that is used in the weapon to predict the time difference
    // between the execution of the attack on the original Client and its replication on the rest of the clients
    public void PerformAttackFromServer(PlayerController player, bool value, float timeDiff) 
    {
        // Call to the weapon's predictive attack method
        sword.AttackPredict(value, timeDiff);
    }

    // This method is responsible for processing the damage received by the player locally
    public void Takedamage(GameObject weapon, float damage)
    {
        // First of all it is verified that the attacking weapon is not the player's own
        if (weapon.transform != sword.transform)
        {
            // Decrease the synchronized variable Health
            currentHealth -= damage;

            // Life check below 0
            if (currentHealth <= 0)
            {
                currentHealth = 0;
            }

            // Local lifeBar update in first person
            float healthSliderValue = currentHealth / maxHealt;
            UIManager.Instance.firstPersonHealthSlider.value = healthSliderValue;

            // Notification to the server about the change
            playerNetworkSync.UpdateHealth(this, currentHealth);
        }
    }

    // Reset the player's health
    public void ResetHealth()
    {
        // Local reset
        currentHealth = maxHealt;

        // Notification to the server about the change
        playerNetworkSync.UpdateHealth(this, maxHealt);
    }

    // Method that controls the situation in case the player loses all his life
    public void PlayerDead() 
    {
        // I move the camera from the Player to the GameManager
        mainCamera.transform.parent = GameManager.Instance.hoverCameraGizmo;
        mainCamera.transform.position = GameManager.Instance.hoverCameraGizmo.position;
        mainCamera.transform.rotation = GameManager.Instance.hoverCameraGizmo.rotation;

        // Switch to lobby panel
        UIManager.Instance.SwitchLobbyPanel(true);

        // I deactivate this GameObject (The player)
        this.gameObject.SetActive(false);
    }

    #endregion

    #region EVENTS

    // This Method is called in the observers when is a change in the sync currentHealth var
    public void OnHealthChange(float oldValue, float newValue, bool asServer)
    {
        if (!asServer)
        {
            // Assign the new currentHealth value
            currentHealth = newValue;

            // Now we have a new variable with the normaliced value of the current health (Between 0 and 1)
            float healthSliderValue = currentHealth / maxHealt;

            // Now we assign the new value to the 3rd person healthBar
            thirdPersonHealthSlider.value = healthSliderValue;
        }
    }

    #endregion
}