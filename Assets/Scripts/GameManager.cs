using FishNet.Component.Spawning;
using FishNet.Managing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameManager Script
/// It is the main Game script and is the one who must control its status
/// In the particular case of this project it only maintains references to the camera
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

    // In this case and to simplify the variables are public and assigned by inspector
    // If necessary, they can be made private and with their respective Gets and Sets

    // References to the main camera to match as necessary
    public GameObject mainCamera;

    // Reference to a gizmo hover for the pre-game state
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