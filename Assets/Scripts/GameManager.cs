using FishNet.Component.Spawning;
using FishNet.Managing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameManager Script
/// Es el escript principal de Juego y es quien debe controlar el estado del mismo
/// En el caso particular de este proyecto solo mantiene referencias a la camara
/// </summary>
public class GameManager : MonoBehaviour
{
    #region SINGLETON

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject("GameManager");
                    _instance = singleton.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    #endregion

    #region GENERAL REFERENCES
    // En este caso y para simplificar las variables son publicas y asignadas por inspector
    // En caso de ser necesario se pueden volver privadas y con sus respectivos Gets y Sets

    // Referencias a la camara principal para emparentar segun sea necesario
    public GameObject mainCamera;

    // Referencia a un gizmo hover para el estado previo al juego
    public Transform hoverCameraGizmo;

    #endregion

    #region AWAKE

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

    #endregion
}