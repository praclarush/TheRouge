using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//http://www.csharpprogramming.tips/2013/07/Rouge-like-dungeon-generation.html

namespace TheRouge
{
    public class CaveGenerator
    {
        private readonly Random _random;
        public int[,] _map;

        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public int PercentAreWalls { get; set; }

        public CaveGenerator(int mapWidth, int mapHeight, int percentAsWalls)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            PercentAreWalls = percentAsWalls;
            _random = new Random();
        }

        private void MakeCaverns()
        {
            for (int column = 0, row = 0; row <= MapHeight-1; row++) {
                for (column = 0; column < MapWidth - 1; column++)
                {
                    _map[column, row] = PlaceWallLogic(column, row);
                }                
            }
        }

        private int PlaceWallLogic(int x, int y)
        {
            int numWalls = GetAdjacentWalls(x, y, 1, 1);

            if (_map[x, y] == 1)
            {
                if (numWalls >= 4)
                {
                    return 1;
                }
                return 0;
            }
            else
            {
                if (numWalls >= 5)
                {
                    return 1;
                }
            }
            return 0;
        }

        private int GetAdjacentWalls(int x, int y, int scopeX, int scopeY)
        {
            int startX = x - scopeX;
            int startY = y - scopeY;
            int endX = x + scopeX;
            int endY = y + scopeY;
            int iX = startX;
            int iY = startY;
            int wallCounter = 0;

            for (iY = startY; iY <= endY; iY++)
            {
                for (iX = startX; iX <= endX; iX++)
                {
                    if (!(iX == x && iY == y)){
                        if (IsWall(iX, iY)){
                            wallCounter += 1;
                        }
                    }
                }
            }
            return wallCounter;
        }

        private bool IsWall(int x, int y)
        {
            if (IsOutOfBounds(x, y))
            {
                return true;
            }

            if (_map[x, y] == 1)
            {
                return true; 
            }

            if (_map[x, y] == 0)
            {
                return false; 
            }
            return false;
        }

        private bool IsOutOfBounds(int x, int y)
        {
            if (x < 0 || y < 0) {
                return true;
            }
            else if (x > MapWidth - 1 || y > MapHeight - 1)
            {
                return true;
            }

            return false;
        }        

        public string MapToString(){
            throw new NotImplementedException();
        }
    }
}
