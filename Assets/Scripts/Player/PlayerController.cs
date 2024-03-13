using FishNet.Object;
using FishNet.Object.Synchronizing;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IDamageable
{
    #region HEALTH

    [SyncVar] public float currentHealth;
    [SerializeField] private float maxHealt = 10f;

    #endregion

    public float movementSpeed = 10f;
    public float rotationSpeed = 5f;

    public Transform cameraContainer;
    public GameObject mainCamera;
    public CharacterController characterController;

    private float rotX = 0.0f;

    public Sword sword;

    public bool isClientInit = false;

    private PlayerNetworkSync playerNetworkSync;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<PlayerController>().enabled = false;
        }
        else {
            isClientInit = true;

            mainCamera = GameManager.Instance.mainCamera;
            mainCamera.transform.parent = cameraContainer;
            mainCamera.transform.position = cameraContainer.position;
            mainCamera.transform.rotation = cameraContainer.rotation;

            playerNetworkSync = GetComponent<PlayerNetworkSync>();
        }
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        currentHealth = maxHealt;
    }

    void Update()
    {
        if (!isClientInit)
            return;

        #region PLAYER MOVEMENT & VIEW

        float rotY = Input.GetAxis("Mouse X") * rotationSpeed;
        rotX -= Input.GetAxis("Mouse Y") * rotationSpeed;

        rotX = Mathf.Clamp(rotX, -90f, 90f);

        transform.Rotate(0, rotY, 0);
        mainCamera.transform.localRotation = Quaternion.Euler(rotX, 0, 0);

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

        characterController.Move(moveDirection * Time.deltaTime);

        #endregion

        #region PLAYER ATTACK

        if (Input.GetKeyDown(KeyCode.Space))
        {
            LocalAttack(true);
            playerNetworkSync.AttackServer(this, true, TimeManager.Tick);
        }
        else if (Input.GetKeyUp(KeyCode.Space)) {
            LocalAttack(false);
            playerNetworkSync.AttackServer(this, false, TimeManager.Tick);
        }

        #endregion
    }

    public void LocalAttack(bool value) 
    {
        sword.Attack(value);
    }

    public void PerformAttackFromServer(PlayerController player, bool value, float timeDiff) 
    {
        sword.AttackPredict(value, timeDiff);
    }

    public void Takedamage(GameObject gameObject, float value)
    {
        if (gameObject.transform != sword.transform)
        {
            currentHealth -= value;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                ResetHealth();
            }
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealt;
    }
}