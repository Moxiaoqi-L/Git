using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissilePrefabs : MonoBehaviour
{

    public static MissilePrefabs Get = null;

    public GameObject[] Prefabs;

    private void Awake() {
        Get = this;
    }
}
