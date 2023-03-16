using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerState
{
    public string playerName;

    public bool alive;

    public int map;

    public float positionX;
    public float positionY;
    public float positionZ;

    public float rotationX;
    public float rotationY;
    public float rotationZ;

    public float velocityX;
    public float velocityY;
    public float velocityZ;

    public string message;
}
