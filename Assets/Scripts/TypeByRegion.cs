using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TypeByRegion : MonoBehaviour
{
    public static TypeByRegion Instance;

    /*We'll need the width and height of 
* our screen, since this will be important for
* setting up our regions
*/
    public readonly static float screenWidth = Screen.width;
    public readonly static float screenHeight = Screen.height;

    #region Public Members
    //To get our cell size for each region
    //There will be 9
    public List<Vector2> regionCells = new List<Vector2>();

    //Padding
    public float left = 1;
    public float right = 1;
    public float top = 1;
    public float bottom = 1;

    //MousePosition
    public Vector3 mousePosition;
    public Vector3 mousePositionRegionWise;

    public int cellNum = 0; 
    #endregion

    #region Private Members

    //A const for generating our 9, which will be 3
    const int cellDivisor = 3;

    //Region Position
    float regionGridX = 0;
    float regionGridY = 0;

    //KeyClusters
    readonly string[] keyClusters =
    {
        "zxc",
        "vbnm",
        ",./",
        "asd",
        "fghj",
        "kl;",
        "qwe",
        "rtyu",
        "iop"
    };
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        CreateRegionalGrid();
    }

    private void Update()
    {
        if (this.enabled)
        {
            mousePosition = Input.mousePosition;

            mousePositionRegionWise.x = CheckRegionalPositionFrom(Input.mousePosition).x;
            mousePositionRegionWise.y = CheckRegionalPositionFrom(Input.mousePosition).y;

            cellNum = RegionalPositionToCellNumber(CheckRegionalPositionFrom(Input.mousePosition));
        }
    }

    //A function that creates our grid
    void CreateRegionalGrid()
    {
        /*First, we need 2 variables:
         * calculatedCellX and
         * calculatedCellY.
         */
        float calculatedCellX = CalculateXCell();
        float calculatedCellY = CalculateYCell();

        //Now with our calculated X and Y, we do for loops
        for (float yRegion = 0; yRegion < screenHeight; yRegion += calculatedCellY)
        {
            for (float xRegion = 0; xRegion < screenWidth; xRegion += calculatedCellX)
            {
                if (xRegion % calculatedCellX == 0 && yRegion % calculatedCellY == 0)
                {
                    Vector2 newCell = new Vector2(xRegion, yRegion);
                    regionCells.Add(newCell);
                }
            }
        }
    }

    private float CalculateXCell()
    {
        return screenWidth / cellDivisor;
    }

    private float CalculateYCell()
    {
        return screenHeight / cellDivisor;
    }

    public int RegionalPositionToCellNumber(Vector2 _position)
    {
        /* This will iterate though the x and y coordinates.
         * If the parameter x and y coordinates matches
         * the function will return
         * xSide + (ySide * cellDivisor)
         * to give us a value between 0 and 8
         * 
         * Doing it this way instead of a switch statement looks so much nicer.
         */
        for (int xSide = 0; xSide < cellDivisor; xSide++)
        {
            for (int ySide = 0; ySide < cellDivisor; ySide++)
            {
                if (_position.x == xSide && _position.y == ySide)
                    return (xSide + (ySide * cellDivisor));
            }
        }
        return -1;
    }

    public Vector3 CheckRegionalPositionFrom(Vector3 _position)
    {
        //Check for x position
        for (int cellX = 0; cellX < cellDivisor; cellX++)
        {
            if (_position.x > regionCells[cellX].x)
                regionGridX = cellX;
        }

        //Check for y position
        for (int cellY = 0; cellY < cellDivisor; cellY++)
        {
            if (_position.y > regionCells[cellY * cellDivisor].y)
                regionGridY = cellY;
        }

        Vector3 v_positionRegionWise = new Vector3(regionGridX, regionGridY);
        return v_positionRegionWise;
    }

    public string GetKeyClusterFromCellNum(int _cellNum)
    {
        return keyClusters[_cellNum];
    }
}