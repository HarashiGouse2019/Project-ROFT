using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArrowDirectionSet : MonoBehaviour
{
    public static ArrowDirectionSet Instance;
    public enum Direction
    {
        LEFT = 180,
        RIGHT = 0,
        UP = 90,
        DOWN = 270
    }

    private Direction enum_Direction;

    [SerializeField] private float coolDownRate = 0.25f;

    private int setHorizontal;

    private int setVertical;

    private bool coolDown = false;

    private float time;

    private GameObject attachedCircle;
    public GameObject AttachedCircle
    {
        set
        {
            attachedCircle = value;
        }
    }

    //Mouse Event to read input
    public MouseEvent m_Event;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        m_Event = FindObjectOfType<MouseEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        CoolDown();
        transform.eulerAngles = new Vector3(0, 0, (float)enum_Direction);
    }

    //This is used in Note Effector
    //It takes what's in file of the direction
    //of the arrows, and setting it based on that values,
    //which them using the value of enum_Direction
    //to change the rotation
    public void SetDirection(int _direction)
    {
        switch (_direction)
        {
            case 0:
                enum_Direction = Direction.RIGHT;
                setHorizontal = 1;
                break;

            case 1:
                enum_Direction = Direction.UP;
                setVertical = 0;
                break;

            case 2:
                enum_Direction = Direction.LEFT;
                setHorizontal = 0;
                break;

            case 3:
                enum_Direction = Direction.DOWN;
                setVertical = 1;
                break;
        }
    }

    //With a given note appears of Type Burst,
    //it will check if the player response matches to the
    //direction of the arrow
    public bool Detect()
    {
        if (!coolDown &&
    (m_Event.horIndex == setHorizontal && (enum_Direction == Direction.LEFT || enum_Direction == Direction.RIGHT)) ||
    (m_Event.verIndex == setVertical && (enum_Direction == Direction.UP || enum_Direction == Direction.DOWN)))
        {
            m_Event.Refresh();
            m_Event.horIndex = 2;
            m_Event.verIndex = 2;
            coolDown = true;
            return coolDown;
        }
        return false;
    }

    //This will prevent multiple hits to occur when this method is called
    void CoolDown()
    {
        if (coolDown)
        {
            time += Time.deltaTime;
            if (time > coolDownRate)
            {
                coolDown = false;
                time = 0f;
            }
        }
    }

    public GameObject GetAttachedCircle()
    {
        return attachedCircle;
    }
}
