using System;
using System.Collections.Generic;
using System.Linq;

//http://www.roguebasin.com/index.php?title=CSharp_Example_of_a_Dungeon-Building_Algorithm

namespace TheRouge {
    public class DungeonGenerator {
        private const string MSG_X_SIZE = "X size of Dungeon: {0}\t";
        private const string MSG_Y_SIZE = "Y size of Dungeon: {0}\t";
        private const string MSG_MAX_OBJECTS = "Max # of Objects: {0}\t";
        private const string MSG_NUMBER_OF_OBJECT = "# of Objects: {0}\t";
        private const short MAX_NUM_TRIES = 1000;
        private const int SQ_ROOM_SIZE = 4;

        private readonly int _xMax; //80 - Columns
        private readonly int _yMax; //25 - Rows

        private readonly int _roomGenChance; //Chance to generate chance
        private readonly Random _rnd;

        private int _xSize;
        private int _ySize;

        private int _numObject;

        private TileType[] _dungeonMap = {};

        private readonly Action<string> _logger;
        private readonly Action<char> _writer;

        public int Corridors { get; private set; }

        public DungeonGenerator(int xMax, int yMax, int roomChance, Action<char> writer, Action<string> logger) {
            _xMax = xMax;
            _yMax = yMax;
            _roomGenChance = roomChance;
            _writer = writer;
            _logger = logger;

            _rnd = new Random();
        }

        private bool IsWall(int x, int y, int xLength, int yLength, int xT, int yT, Direction direction) {
            bool ret = false;

            switch (direction) {
                case Direction.North:
                    ret = xT == GetFeatureLowerBound(x, xLength) || xT == IsFeatureWallBound(x, xLength) || yT == y || yT == y - (yLength + 1);
                    break;
                case Direction.East:
                    ret = xT == x || xT == x + (xLength - 1) || yT == GetFeatureLowerBound(y, yLength) || yT == IsFeatureWallBound(y, yLength);
                    break;
                case Direction.South:
                    ret = xT == GetFeatureLowerBound(x, xLength) || xT == IsFeatureWallBound(x, xLength) || yT == y || yT == y + (yLength - 1);
                    break;
                case Direction.West:
                    ret = xT == x || xT == x - (xLength + 1) || yT == GetFeatureLowerBound(y, yLength) || yT == IsFeatureWallBound(y, yLength);
                    break;
                default:
                    throw new InvalidOperationException("Invalid Direction"); //TODO(Nathan): Create Custom Exception for this
            }

            return ret;
        }

        private IEnumerable<Point> GetRoomPoints(int x, int y, int xLength, int yLength, Direction direction) {
            switch (direction) {
                case Direction.North: {
                    for (int xt = GetFeatureLowerBound(x, xLength); xt < GetFeatureUpperBound(x, xLength); xt++) {
                        for (int yt = y; yt > y - yLength; yt--) {
                            yield return new Point {X = xt, Y = yt};
                        }
                    }
                }
                    break;
                case Direction.East: {
                    for (int xt = x; xt < x + xLength; xt++) {
                        for (int yt = GetFeatureLowerBound(y, yLength); yt < GetFeatureUpperBound(y, yLength); yt++) {
                            yield return new Point {X = xt, Y = yt};
                        }
                    }
                }
                    break;
                case Direction.South: {
                    for (int xt = GetFeatureLowerBound(x, xLength); xt < GetFeatureUpperBound(x, xLength); xt++) {
                        for (int yt = y; yt < y + yLength; yt++) {
                            yield return new Point {X = xt, Y = yt};
                        }
                    }
                }
                    break;
                case Direction.West: {
                    for (int xt = x; xt > x - xLength; xt--) {
                        for (int yt = GetFeatureLowerBound(y, yLength); yt < GetFeatureUpperBound(y, yLength); yt++) {
                            yield return new Point {X = xt, Y = yt};
                        }
                    }
                }
                    break;
                default:
                    yield break;
            }
        }

        private void SetCell(int x, int y, TileType cellType) {
            _dungeonMap[x + _xSize * y] = cellType;
        }

        private bool MakeCorridor(int x, int y, int length, Direction direction) {
            int len = GetRandomNumber(2, length);

            TileType floor = TileType.Corridor;

            int xTemp = 0;
            int yTemp = 0;

            switch (direction) {
                case Direction.North: {
                    if (x < 0 || x > _xSize) { return false; }

                    xTemp = x;

                    for (int ytemp = y; ytemp > (y - len); ytemp--) {
                        if (yTemp < 0 || ytemp > _ySize) { return false; }
                        if (GetCellType(xTemp, ytemp) != TileType.Unused) { return false; }
                    }

                    Corridors++;

                    for (yTemp = y; yTemp > (y - len); yTemp--) {
                        SetCell(xTemp, yTemp, floor);
                    }
                }
                    break;
                case Direction.East: {
                    if (y < 0 || y > _ySize) { return false; }
                    yTemp = y;

                    for (xTemp = x; xTemp < (x + len); xTemp++) {
                        if (xTemp < 0 || xTemp > _xSize) { return false; }
                        if (GetCellType(xTemp, yTemp) != TileType.Unused) { return false; }
                    }

                    Corridors++;
                    for (xTemp = x; xTemp < (x + len); xTemp++) {
                        SetCell(xTemp, yTemp, floor);
                    }
                }
                    break;
                case Direction.South: {
                    if (x < 0 || x > _xSize) { return false; }

                    xTemp = x;

                    for (yTemp = y; yTemp < (y + len); yTemp++) {
                        if (yTemp < 0 || yTemp > _ySize) { return false; }
                        if (GetCellType(xTemp, yTemp) != TileType.Unused) { return false; }
                    }

                    Corridors++;

                    for (yTemp = y; yTemp < (y + len); yTemp++) {
                        SetCell(xTemp, yTemp, floor);
                    }
                }
                    break;
                case Direction.West: {
                    if (yTemp < 0 || yTemp > _ySize) { return false; }

                    yTemp = y;

                    for (xTemp = x; xTemp > (x - len); xTemp--) {
                        if (xTemp < 0 || xTemp > _xSize) { return false; }
                        if (GetCellType(xTemp, yTemp) != TileType.Unused) { return false; }
                    }

                    Corridors++;

                    for (xTemp = x; xTemp > (x - len); xTemp--) {
                        SetCell(xTemp, yTemp, floor);
                    }
                }
                    break;
            }

            return true;
        }

        private bool MakeRoom(int x, int y, int xLength, int yLength, Direction direction) {
            int xLen = GetRandomNumber(SQ_ROOM_SIZE, xLength);
            int yLen = GetRandomNumber(SQ_ROOM_SIZE, yLength);

            TileType floor = TileType.DirtFloor;
            TileType wall = TileType.DirtWall;

            Point[] points = GetRoomPoints(x, y, xLength, yLength, direction).ToArray();

            if (points.Any(p => p.Y < 0 || p.Y > _ySize || p.X < 0 || p.X > _xSize || GetCellType(p.X, p.Y) != TileType.Unused)) {
                return false;
            }

            _logger(string.Format("Making Room: int x={0}, int y={1}, int xLength={2}, int yLengt={3}, int direction={4}", x, y, xLength, yLength, direction));

            foreach (Point point in points) {
                SetCell(point.X, point.Y, IsWall(x, y, xLen, yLen, point.X, point.Y, direction) ? wall : floor);
            }

            return true;
        }

        private IEnumerable<Tuple<Point, Direction>> GetSurroundingPoints(Point value) {
            Tuple<Point, Direction>[] points = {Tuple.Create(new Point {X = value.X, Y = value.Y + 1}, Direction.North), Tuple.Create(new Point {X = value.X - 1, Y = value.Y}, Direction.East), Tuple.Create(new Point {X = value.X, Y = value.Y - 1}, Direction.South), Tuple.Create(new Point {X = value.X + 1, Y = value.Y}, Direction.West)};
            return points.Where(x => InBounds(x.Item1));
        }

        private IEnumerable<Tuple<Point, Direction, TileType>> GetSurroundings(Point value) {
            return GetSurroundingPoints(value).Select(x => Tuple.Create(x.Item1, x.Item2, GetCellType(x.Item1.X, x.Item1.Y)));
        }

        private TileType GetCellType(int x, int y) {
            try {
                return _dungeonMap[x + _xSize * y];
            }
            catch (IndexOutOfRangeException ex) {
                //TODO(Nathan): Add Logging
                throw;
            }
        }

        private char GetCellTile(int x, int y) {
            switch (GetCellType(x, y)) {
                case TileType.Unused:
                    return ' ';
                case TileType.DirtWall:
                    return '|';
                case TileType.DirtFloor:
                    return '_';
                case TileType.StoneWall:
                    return ' ';
                case TileType.Corridor:
                    return '#';
                case TileType.Door:
                    return 'D';
                case TileType.Upstairs:
                    return '+';
                case TileType.Downstairs:
                    return '-';
                case TileType.Chest:
                    return 'C';
                default:
                    throw new ArgumentOutOfRangeException("x,y"); //TODO(Nathan): Make a custom Exception for this.                    
            }
        }
        
        private Direction RandomDirection() {
            int direction = GetRandomNumber(0, 4);
            switch (direction) {
                case 0:
                    return Direction.North;
                case 1:
                    return Direction.East;
                case 2:
                    return Direction.South;
                case 3:
                    return Direction.West;
                default:
                    throw new InvalidOperationException("Random direction chosen was different then the four cardinal directions");
            }
        }

        private void AddObjects() {
            int state = 0;
            while (state != 10) {
                for (int tries = 0; tries < MAX_NUM_TRIES; tries++) {
                    int newX = GetRandomNumber(1, _xSize - 1);
                    int newY = GetRandomNumber(1, _ySize - 2);

                    int sides = 4;

                    if (GetCellType(newX, newY + 1) == TileType.DirtFloor || GetCellType(newX, newY + 1) == TileType.Corridor) {
                        if (GetCellType(newX, newY + 1) != TileType.Door) {
                            sides--;
                        }
                    }

                    if (GetCellType(newX - 1, newY) == TileType.DirtFloor || GetCellType(newX - 1, newY) == TileType.Corridor) {
                        if (GetCellType(newX - 1, newY) != TileType.Door) {
                            sides--;
                        }
                    }

                    if (GetCellType(newX, newY - 1) == TileType.DirtFloor || GetCellType(newX, newY - 1) == TileType.Corridor) {
                        if (GetCellType(newX, newY - 1) != TileType.Door) {
                            sides--;
                        }
                    }

                    if (GetCellType(newX + 1, newY) == TileType.DirtFloor || GetCellType(newX + 1, newY) == TileType.Corridor) {
                        if (GetCellType(newX + 1, newY) != TileType.Door) {
                            sides--;
                        }
                    }

                    if (state == 0) {
                        if (sides == 0) {
                            SetCell(newX, newY, TileType.Upstairs);
                            state = 1;
                            break;
                        }
                    } else if (state == 1) {
                        if (sides == 0) {
                            SetCell(newX, newY, TileType.Downstairs);
                            state = 9;
                            break;
                        }
                    } else if (state == 9) {
                        if (sides == 0) {
                            SetCell(newX, newY, TileType.Chest);
                            state = 10;
                            break;
                        }
                    }
                }
            }
        }

        private void BuildMapEdge() {
            for (int y = 0; y < _ySize; y++) {
                for (int x = 0; x < _xSize; x++) {
                    if (y == 0 || y == _ySize - 1 || x == 0 || x == _xSize - 1) {
                        SetCell(x, y, TileType.StoneWall);
                    } else {
                        SetCell(x, y, TileType.Unused);
                    }
                }
            }
        }

        private bool InBounds(int x, int y) {
            return x > 0 && x < _xMax && y > 0 && y < _yMax;
        }

        private bool InBounds(Point value) {
            return InBounds(value.X, value.Y);
        }

        private int GetRandomNumber(int min, int max) {
            return _rnd.Next(min, max);
        }

        private int GetFeatureLowerBound(int value, int length) {
            return value - (length / 2);
        }

        private int GetFeatureUpperBound(int value, int length) {
            return value + ((length + 1) / 2);
        }

        private int IsFeatureWallBound(int value, int length) {
            return value + ((length - 1) / 2);
        }
        
        public TileType[] GetDungeon() {
            return _dungeonMap;
        }
        
        public bool GenerateDungeon(int width, int height, int numObjects) {
            int currentFeatures = 0;

            _numObject = numObjects < 1 ? 10 : numObjects;

            if (width < 3) {
                _xSize = 3;
            }
            else if (width > _xMax) {
                _xSize = _xMax;
            }
            else {
                _xSize = width;
            }

            if (height < 3) {
                _ySize = 3;
            }
            else if (height > _yMax) {
                _ySize = _yMax;
            }
            else {
                _ySize = height;
            }

            _logger(string.Format("Max Map Width: {0}", _xSize));
            _logger(string.Format("Max Map Height: {0}", _ySize));
            _logger(string.Format("Max Number of Objects: {0}", _numObject));

            _dungeonMap = new TileType[_xSize * _ySize];

            BuildMapEdge();

            MakeRoom(_xSize / 2, _ySize / 2, 8, 6, RandomDirection());

            currentFeatures += 1;

            for (int tries = 0; tries < MAX_NUM_TRIES; tries++) {
                if (currentFeatures == _numObject) {
                    break;
                }

                int newX = 0;
                int xMod = 0;
                int newY = 0;
                int yMod = 0;
                Direction? validTile = null;

                for (int tests = 0; tests < MAX_NUM_TRIES; tests++) {
                    newX = GetRandomNumber(1, _xSize - 1);
                    newY = GetRandomNumber(1, _ySize - 1);

                    if (GetCellType(newX, newY) == TileType.DirtWall || GetCellType(newX, newY) == TileType.Corridor) {
                        IEnumerable<Tuple<Point, Direction, TileType>> surroundings = GetSurroundings(new Point {X = newX, Y = newY});

                        Tuple<Point, Direction, TileType> canReach = surroundings.FirstOrDefault(x => x.Item3 == TileType.Corridor || x.Item3 == TileType.DirtFloor);

                        if (canReach == null) {
                            continue;
                        }

                        validTile = canReach.Item2;
                        switch (validTile) {
                            case Direction.North:
                                xMod = 0;
                                yMod = -1;
                                break;
                            case Direction.East:
                                xMod = 1;
                                yMod = 0;
                                break;
                            case Direction.South:
                                xMod = 0;
                                yMod = 1;
                                break;
                            case Direction.West:
                                xMod = -1;
                                yMod = 0;
                                break;
                            default:
                                throw new InvalidOperationException(); //TODO(Nathan): Make this a custom Exception
                        }

                        if (GetCellType(newX, newY + 1) == TileType.Door) {
                            validTile = null;
                        }
                        else if (GetCellType(newX - 1, newY) == TileType.Door) {
                            validTile = null;
                        }
                        else if (GetCellType(newX, newY - 1) == TileType.Door) {
                            validTile = null;
                        }
                        else if (GetCellType(newX + 1, newY) == TileType.Door) {
                            validTile = null;
                        }

                        if (validTile.HasValue) {
                            break;
                        }
                    }
                }

                if (validTile.HasValue) {
                    int feature = GetRandomNumber(0, 100);
                    if (feature <= _roomGenChance) {
                        if (MakeRoom(newX + xMod, newY + yMod, 8, 6, validTile.Value)) {
                            currentFeatures++;

                            SetCell(newX, newY, TileType.Door);

                            SetCell(newX + xMod, newY + yMod, TileType.DirtFloor);
                        }
                    }
                    else {
                        if (MakeCorridor(newX + xMod, newY + yMod, 6, validTile.Value)) {
                            currentFeatures++;
                            SetCell(newX, newY, TileType.Door);
                        }
                    }
                }
            }

            //AddObjects();

            _logger(MSG_NUMBER_OF_OBJECT + currentFeatures);
            return true;
        }

        public void PrintDungeon() {
            for (int y = 0; y < _ySize; y++) {
                for (int x = 0; x < _xSize; x++) {
                    _writer(GetCellTile(x, y));
                }
            }
        }
    }
}