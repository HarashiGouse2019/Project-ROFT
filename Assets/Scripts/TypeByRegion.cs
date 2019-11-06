using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TypeByRegion : MonoBehaviour
{
    /*We'll need the width and height of 
     * our screen, since this will be important for
     * setting up our regions
     */
    readonly float screenWidth = Screen.width;
    readonly float screenHeight = Screen.height;

    //To get our cell size for each region
    //There will be 9
    public List<Vector2> regionCells = new List<Vector2>();

    //A const for generating our 9, which will be 3
    const int cellDivisor = 3;

    //MousePosition
    public Vector3 mousePosition;
    public Vector3 mousePositionRegionWise;

    //Region Position
    float regionGridX = 0;
    float regionGridY = 0;

    public int cellNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        CreateRegionalGrid();
    }

    private void Update()
    {
        mousePosition = Input.mousePosition;

        mousePositionRegionWise.x = CheckRegionalPositionFrom(Input.mousePosition).x;
        mousePositionRegionWise.y = CheckRegionalPositionFrom(Input.mousePosition).y;

        cellNum = RegionalPositionToCellNumber(CheckRegionalPositionFrom(Input.mousePosition));
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
        //For now... we'll prototype it...
        switch (_position.x)
        {
            case 0:
                switch (_position.y)
                {
                    case 0:
                        return 0;
                    case 1:
                        return 3;
                    case 2:
                        return 6;
                }
                break;
            case 1:
                switch (_position.y)
                {
                    case 0:
                        return 1;
                    case 1:
                        return 4;
                    case 2:
                        return 7;
                }
                break;
            case 2:
                switch (_position.y)
                {
                    case 0:
                        return 2;
                    case 1:
                        return 5;
                    case 2:
                        return 8;
                }
                break;
            default:
                break;
        }
        //Stupid... I know...
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
}