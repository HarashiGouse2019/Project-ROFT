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
    public static float screenWidth;
    public static float screenHeight;

    #region Public Members
    //To get our cell size for each region
    //There will be 9
    public List<Vector2> regionCells = new List<Vector2>();

    //Padding
    public static float left = 150f;
    public static float right = 150f;
    public static float top = 100f;
    public static float bottom = 100f;

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

    //KeyClusters: TBR_ALL
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
        UpdateScreenSize();
        CreateRegionalGrid();
    }

    private void Update()
    {
        UpdateScreenSize();

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

    private bool CompareNumericalDifference(string _sign, int _leftNumeric, int _rightNumberic)
    {
        switch (_sign)
        {
            case ">":
                return (_leftNumeric > _rightNumberic);

            case "<":
                return (_leftNumeric < _rightNumberic);

            default:
                Debug.LogError("This is an invalid symbol for comparing.");
                return false;
        }
    }

    private bool BetweenValues(int _value1, int _value2, int _setValue)
    {
        return (_setValue > _value1 && _setValue < _value2);
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
                //If all keys are being used, we need values 0 through 8
                #region If TBR_ALL
                if (GameManager.Instance.gameMode == GameManager.GameMode.TBR_ALL && (_position.x == xSide && _position.y == ySide))
                    return (xSide + (ySide * cellDivisor));
                #endregion

                //However if we are only using homerows, we take our normal equation and add or 
                //subtract cellDivisor (3) to get all home row keys (3 - 5)
                #region If TBR_HomeRow
                else if (GameManager.Instance.gameMode == GameManager.GameMode.TBR_HOMEROW &&
            (_position.x == xSide && _position.y == ySide))
                {
                    int cellEquation = xSide + (ySide * cellDivisor);

                    //Made functions to not make long if statments
                    bool lessThan = CompareNumericalDifference("<", cellDivisor, cellEquation);
                    bool moreThan = CompareNumericalDifference(">", cellDivisor, cellEquation);

                    if (!BetweenValues(3, 5, cellEquation))
                    {
                        if (lessThan) return (cellEquation - cellDivisor);
                        else if (moreThan) return (cellEquation + cellDivisor);
                    }
                } 
                #endregion
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

    public void UpdateScreenSize()
    {
        screenWidth = Screen.width - right;
        screenHeight = Screen.height - top;
    }
}