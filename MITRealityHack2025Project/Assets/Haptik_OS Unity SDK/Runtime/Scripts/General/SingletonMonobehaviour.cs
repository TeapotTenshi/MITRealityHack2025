using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type typeOfComponent = typeof(T);

                if (typeOfComponent == typeof(SceneLoader))
                    Instantiate(HaptikosResources.Instance.sceneLoader);

                if (typeOfComponent == typeof(IMUCalibrationManager))
                    Instantiate(HaptikosResources.Instance.calibrationManager);

            }

            return instance;
        }
    }
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else
            Destroy(gameObject);

    }

    protected virtual void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
