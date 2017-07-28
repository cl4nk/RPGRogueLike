using System;
using System.Collections.Generic;
using Character;
using InteractableObjects;
using Item;
using Managers;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGeneration : MonoBehaviour
{
    public delegate void IntDelegate(int value);

    public delegate void SimpleDelegate();

    public enum DIRECTION
    {
        North,
        South,
        East,
        West,
        Count,
        None = -1
    }

    [Header("Stats")]
    public float baseDensity = 0.5f;
    public float baseDensitySpawn = 0.01f;
    public float baseDensityTreasure = 0.01f;
    public int baseEnemyNumber = 10;
    public int baseNbRooms = 2;

    [Header("Size")]
    public int baseWidth = 50, baseHeight = 50;
    public int cellSize = 5;
    public int minRoomSize = 5;

    [Header("Prefabs")]
    public GameObject cubePrefab;
    public GameObject groundPrefab;
    public GameObject bossPrefab;

    public GameObject[] decoPrefabs;
    public bool[] decoRandomDir;

    public Vector3[] decoScales;
    public GameObject torchPrefab;
    public GameObject treasurePrefab;
    public GameObject doorPrefab;

    private float density;
    public float densityDeco = 0.01f;
    private float densitySpawn;
    private float densityTreasure;
    private int enemyNumber;
    private GameObject enemySpawners;

    [Range(1, 100)] private int mFloor = 1;

    private int nbRooms;
    private Vector3 normalPlainScale;
    private Vector3 normalWallScale;

    private PoolObjects poolObjects;

    private Transform posPortal;

    private List<CellPosition> rooms;
    public float rotationWall = 90;
    public GameObject spawnEnemyPrefab;
    private CellPosition spawnPos;
    public GameObject spawnPrefab;

    private int spawnRoom;
    private GameObject stairsObj;
    private CellPosition stairsPos;
    public GameObject stairsPrefab;
    [Header("Step")]
    public float stepDensitySpawn = 0.01f;
    public float stepDensityTreasure = 0.01f;
    public int stepEnemyNumber = 10;
    public int stepNbRooms = 1;

    public int stepWidth = 15, stepHeight = 15;

    private Cell[,] tabCells;

    public float thickness = .1f;
    private int torchIndex;

    public int torchModulo = 3;
    

    private int width, height;

    public int Floor
    {
        get { return mFloor; }
        set { SetFloor(value); }
    }

    public event SimpleDelegate OnLevelCreation;
    public event SimpleDelegate OnLevelCreated;
    public event IntDelegate OnFloorChange;

    private void Awake()
    {
        enemySpawners = transform.Find("EnemySpawners").gameObject;
        EnemyManager.Instance.EnemySpawners = enemySpawners;

        OnLevelCreation += UIGame.Instance.ShowLoadingScreen;
        OnLevelCreated += UIGame.Instance.HideLoadingScreen;
    }

    private void Start()
    {
        poolObjects = new PoolObjects();
        rooms = new List<CellPosition>();
        normalWallScale = new Vector3(1, 1, 0.1f);

        mFloor = PlayerPrefs.GetInt("LastReachedFloor", 1);
        SetFloor(mFloor);
    }

    public void NextFloor()
    {
        SetFloor(mFloor + 1);
    }

    public void SetFloor(int floor)
    {
        if (OnFloorChange != null)
            OnFloorChange(floor);

        PlayerPrefs.SetInt("LastReachedFloor", floor);

        mFloor = floor;

        if (mFloor % 5 == 0)
            CreateBossRoom();
        else
            InitNewMaze(baseWidth + stepWidth * mFloor,
                baseHeight + stepHeight * mFloor,
                cellSize,
                baseNbRooms + stepNbRooms * mFloor,
                baseDensity,
                baseDensityTreasure + stepDensityTreasure * mFloor,
                baseDensitySpawn + stepDensitySpawn * mFloor,
                baseEnemyNumber + stepEnemyNumber * mFloor,
                densityDeco);
    }

    public void InitNewMaze(int width = 50, int height = 50,
        int cellSize = 10,
        int nbRooms = 5,
        float density = 0.6f,
        float densityTreasure = 0.01f,
        float densitySpawn = 0.01f,
        int enemyNumber = 10,
        float densityDeco = 0.05f)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.nbRooms = nbRooms;
        this.density = density;
        this.densityTreasure = densityTreasure;
        this.densitySpawn = densitySpawn;
        this.enemyNumber = enemyNumber;
        this.densityDeco = densityDeco;

        if (OnLevelCreation != null)
            OnLevelCreation();

        Init();

        PlacePlayer();

        EnemyManager.Instance.CreateAllEnemies(enemyNumber);

        if ((mFloor - 1) % 5 == 0)
        {
            posPortal.gameObject.SetActive(true);
            UIGame.Instance.AddPointToCompass(posPortal.gameObject);
        }
        else
        {
            posPortal.gameObject.SetActive(false);
        }

        if (OnLevelCreated != null)
            OnLevelCreated();
    }

    private void PlacePlayer()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        player.position = posPortal.position + posPortal.right * 3;
        player.forward = posPortal.right;
    }

    private void Init()
    {
        if (nbRooms == 0)
            return;


        normalPlainScale = new Vector3(width * cellSize, thickness, height * cellSize);

        CleanMaze();

        InitTabCells();
        rooms.Clear();

        do
        {
            GenerateRoomsPositions();
        } while (!AreAllRoomsFarEnough());

        GenerateRoomsSpace();

        LinkAllRooms();
        GenerateRandomPaths();
        CloseMaze();

        RemoveUselessBorders();
        RemoveUselessWalls();
        RemoveUselessCorridors();

        GenerateDoors();
        GenerateSpawnPos();
        GenerateStairsPos();

        GenerateTreasures();
        GenerateSpawnsEnemy();
        GenerateDecos();

        BuildMaze();
    }

    private void BuildMaze()
    {
        poolObjects.InitIndexes();

        BuildTerrain();
        BuildRoof();

        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Cell cell = tabCells[x, y];
            if (cell.GetCaseType() == Cell.CASE_TYPE.Closed)
                continue;
            CellPosition pos = new CellPosition(x, y);
            if (cell.walls[(int) DIRECTION.North])
                BuildWall(pos, DIRECTION.North);
            if (cell.walls[(int) DIRECTION.East])
                BuildWall(pos, DIRECTION.East);
            if (cell.walls[(int) DIRECTION.South])
                BuildWall(pos, DIRECTION.South);
            if (cell.walls[(int) DIRECTION.West])
                BuildWall(pos, DIRECTION.West);

            BuildObject(pos);
        }
        poolObjects.Finish();
    }

    private void CreateBossRoom()
    {
        width = 20;
        height = 20;

        normalPlainScale = new Vector3(width * cellSize, thickness, height * cellSize);

        CleanMaze();

        InitTabCells();
        for (int i = 0; i < width; i++)
        for (int j = 0; j < height; j++)
            tabCells[i, j].Open();

        int x = width / 2;
        int y = height / 3;

        CloseMaze();
        GenerateSpawnPos(x, y);
        GenerateStairsPos(y, x);
        //GenerateDecos();

        BuildMaze();
        BuildBoss();

        PlacePlayer();
        posPortal.gameObject.SetActive(false);
        stairsObj.SetActive(false);
    }

    private void CleanMaze()
    {
        if (posPortal)
        {
            UIGame.Instance.RemovePointFromCompass(posPortal.gameObject);
            posPortal = null;
        }

        EnemyManager.Instance.DestroyAllEnemies();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != transform && child != enemySpawners.transform && !poolObjects.IsInPool(child.gameObject))
                Destroy(child.gameObject);
        }
    }

    private void InitTabCells()
    {
        tabCells = new Cell[width, height];
        for (int i = 0; i < width; i++)
        for (int j = 0; j < height; j++)
            tabCells[i, j] = new Cell(i, j);
    }

    private int CalcMaxSizeRoom(int width, int height, int nbRooms)
    {
        int area = width * height;
        area = (int) (area * density);
        float areaRoom = area / nbRooms;
        return Mathf.Max(1, (int) Mathf.Sqrt(areaRoom));
    }

    private void LinkPositions(CellPosition a, CellPosition b)
    {
        if (Random.Range(0, 1) == 0)
            CreateStraightXYPathBetween(a, b);
        else
            CreateStraightYXPathBetween(a, b);
    }

    private void LinkAllRooms()
    {
        for (int i = 0; i < rooms.Count - 1; i++)
        for (int j = i + 1; j < rooms.Count; j++)
            LinkPositions(rooms[i], rooms[j]);
    }

    private bool AreAllRoomsFarEnough()
    {
        for (int i = 0; i < rooms.Count - 1; i++)
        for (int j = i + 1; j < rooms.Count; j++)
            if (DistanceBetweenRooms(rooms[i], rooms[j]) < (width - minRoomSize) / nbRooms)
                return false;
        return true;
    }

    private void GenerateRoomsPositions()
    {
        rooms.Clear();
        for (int i = 0; i < nbRooms; i++)
        {
            int x = Random.Range(minRoomSize, width - minRoomSize);
            int y = Random.Range(minRoomSize, height - minRoomSize);
            CellPosition pos = new CellPosition(x, y);
            rooms.Add(pos);
        }
    }

    private void GenerateRoomsSpace()
    {
        foreach (CellPosition centerRoom in rooms)
            CreatePlain(centerRoom, new CellPosition(minRoomSize, minRoomSize));

        int i = 0;
        while (GetPercentOpenedCell() < baseDensity)
        {
            foreach (CellPosition centerRoom in rooms)
            {
                int x = Random.Range(-i, i);
                int y = Random.Range(-i, i);
                CellPosition pos = new CellPosition(CorrectX(centerRoom.x + x), CorrectY(centerRoom.y + y));
                CreatePlain(pos, new CellPosition(minRoomSize, minRoomSize));
                LinkPositions(centerRoom, pos);
            }
            i++;
        }
    }

    private int CorrectX(int x)
    {
        if (x < 0)
            return 0;
        if (x >= width)
            return width - 1;
        return x;
    }

    private int CorrectY(int y)
    {
        if (y < 0)
            return 0;
        if (y >= height)
            return height - 1;
        return y;
    }

    private void GenerateSpawnPos(int x = -1, int y = -1)
    {
        if (x >= 0 && y >= 0)
        {
            tabCells[x, y].obj = Cell.CASE_OBJECT.Spawn;
            spawnPos = new CellPosition(x, y);
            return;
        }

        int roomIndex = Random.Range(0, rooms.Count);

        spawnRoom = roomIndex;

        CellPosition spawnRoomPos = rooms[roomIndex];
        tabCells[spawnRoomPos.x, spawnRoomPos.y].obj = Cell.CASE_OBJECT.Spawn;
        spawnPos = new CellPosition(spawnRoomPos.x, spawnRoomPos.y);
    }

    private void GenerateStairsPos(int x = -1, int y = -1)
    {
        if (x >= 0 && y >= 0)
        {
            tabCells[x, y].obj = Cell.CASE_OBJECT.Stairs;
            stairsPos = new CellPosition(x, y);
            return;
        }

        int roomIndex;

        do
        {
            roomIndex = Random.Range(0, rooms.Count);
        } while (roomIndex == spawnRoom);

        CellPosition stairsRoomPos = rooms[roomIndex];
        tabCells[stairsRoomPos.x, stairsRoomPos.y].obj = Cell.CASE_OBJECT.Stairs;
        stairsPos = new CellPosition(stairsRoomPos.x, stairsRoomPos.y);
    }

    private void GenerateTreasures()
    {
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Cell.CASE_TYPE caseType = tabCells[x, y].GetCaseType();
            if (caseType != Cell.CASE_TYPE.Border && caseType != Cell.CASE_TYPE.Trap)
                continue;
            if (tabCells[x, y].obj != Cell.CASE_OBJECT.None)
                continue;
            if (IsObjNear(new CellPosition(x, y), Cell.CASE_OBJECT.Tresor))
                continue;
            if (Random.value < densityTreasure)
                tabCells[x, y].obj = Cell.CASE_OBJECT.Tresor;
        }
    }

    private void GenerateDecos()
    {
        if (decoPrefabs.Length == 0)
            return;
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Cell.CASE_TYPE caseType = tabCells[x, y].GetCaseType();
            if (caseType != Cell.CASE_TYPE.Plain)
                continue;
            if (tabCells[x, y].obj != Cell.CASE_OBJECT.None)
                continue;
            if (HasObjNear(new CellPosition(x, y)))
                continue;
            if (Random.value < densityDeco)
            {
                tabCells[x, y].obj = Cell.CASE_OBJECT.Decor;
                tabCells[x, y].decoIndex = Random.Range(0, decoPrefabs.Length);
            }
        }
    }

    private void GenerateSpawnsEnemy()
    {
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Cell.CASE_TYPE caseType = tabCells[x, y].GetCaseType();
            if (caseType != Cell.CASE_TYPE.Plain)
                continue;
            if (tabCells[x, y].obj != Cell.CASE_OBJECT.None)
                continue;
            if (IsObjNear(new CellPosition(x, y), Cell.CASE_OBJECT.SpawnEnemy))
                continue;
            if (Random.value < densitySpawn)
                tabCells[x, y].obj = Cell.CASE_OBJECT.SpawnEnemy;
        }
    }

    private void RemoveUselessCorridors()
    {
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Cell cell = tabCells[x, y];
            CellPosition pos = cell.Pos;
            if (cell.GetCaseType() != Cell.CASE_TYPE.Corridor)
                continue;
            DIRECTION dir = GetNearTypeDirection(pos, Cell.CASE_TYPE.Corridor);
            if (dir != DIRECTION.None)
            {
                DIRECTION opposedDir = GetOpposedDirection(dir);
                Cell other = GetCaseByDirection(pos, dir);
                if (other == null)
                    continue;
                if (!cell.walls[(int) dir] && !other.walls[(int) opposedDir])
                    continue;
                cell.walls[(int) dir] = false;
                other.walls[(int) opposedDir] = false;
            }
        }
    }

    private void RemoveUselessWalls()
    {
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Cell cell = tabCells[x, y];
            CellPosition pos = cell.Pos;
            if (cell.GetCaseType() == Cell.CASE_TYPE.Closed)
                continue;
            foreach (DIRECTION dir in Enum.GetValues(typeof(DIRECTION)))
            {
                if (dir == DIRECTION.Count || dir == DIRECTION.None)
                    continue;
                DIRECTION opposedDir = GetOpposedDirection(dir);
                Cell other = GetCaseByDirection(pos, dir);
                if (other == null)
                    continue;
                if (other.GetCaseType() == Cell.CASE_TYPE.Closed)
                    continue;
                if (cell.walls[(int) dir] && other.walls[(int) opposedDir])
                    cell.walls[(int) dir] = false;
            }
        }
    }

    private void RemoveUselessBorders()
    {
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            CellPosition pos = new CellPosition(x, y);
            DIRECTION dir = GetNearTypeDirection(pos, Cell.CASE_TYPE.Border);
            if (dir != DIRECTION.None)
            {
                DIRECTION opposedDir = GetOpposedDirection(dir);
                Cell me = tabCells[x, y];
                Cell other = GetCaseByDirection(pos, dir);
                if (other == null)
                    continue;
                if (me.walls[(int) dir] && other.walls[(int) opposedDir])
                {
                    me.walls[(int) dir] = false;
                    other.walls[(int) opposedDir] = false;
                }
            }
            else
            {
                dir = GetNearTypeDirection(pos, Cell.CASE_TYPE.Plain);
                if (dir == DIRECTION.None)
                    return;
                Cell me = tabCells[x, y];
                if (me.walls[(int) dir])
                    me.walls[(int) dir] = false;
            }
        }
    }

    private Cell GetCaseByDirection(CellPosition pos, DIRECTION dir)
    {
        int x = pos.x;
        int y = pos.y;
        switch (dir)
        {
            case DIRECTION.North:
                y++;
                break;
            case DIRECTION.South:
                y--;
                break;
            case DIRECTION.East:
                x++;
                break;
            case DIRECTION.West:
                x--;
                break;
        }
        if (!IsInWidth(x) || !IsInWidth(y))
            return null;
        return tabCells[x, y];
    }

    private DIRECTION GetOpposedDirection(DIRECTION dir)
    {
        switch (dir)
        {
            case DIRECTION.North:
                return DIRECTION.South;
            case DIRECTION.South:
                return DIRECTION.North;
            case DIRECTION.East:
                return DIRECTION.West;
            case DIRECTION.West:
                return DIRECTION.East;
            default:
                return DIRECTION.None;
        }
    }

    private int DistanceBetweenRooms(CellPosition room1, CellPosition room2)
    {
        return (int) Mathf.Sqrt((room2.x - room1.x) * (room2.x - room1.x) + (room2.y - room1.y) * (room2.y - room1.y)) +
               1;
    }

    private bool IsInWidth(int x)
    {
        return x >= 0 && x < width;
    }

    private bool IsInHeight(int y)
    {
        return y >= 0 && y < height;
    }

    private float GetPercentOpenedCell()
    {
        int max = width * height;
        int count = 0;
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
            if (tabCells[x, y].GetCaseType() != Cell.CASE_TYPE.Closed)
                count++;

        return count / (float) max;
    }

    private void CreatePlain(CellPosition centerPos, CellPosition size)
    {
        for (int i = centerPos.x - size.x / 2; i < centerPos.x + size.x / 2; i++)
        for (int j = centerPos.y - size.y / 2; j < centerPos.y + size.y / 2; j++)
        {
            if (!IsInHeight(i) || !IsInHeight(j))
                continue;
            Cell cell = tabCells[i, j];
            if (j != centerPos.y + size.y / 2 - 1)
                cell.walls[(int) DIRECTION.North] = false;
            if (i != centerPos.x + size.x / 2 - 1)
                cell.walls[(int) DIRECTION.East] = false;
            if (i != centerPos.x - size.x / 2)
                cell.walls[(int) DIRECTION.West] = false;
            if (j != centerPos.y - size.y / 2)
                cell.walls[(int) DIRECTION.South] = false;
        }
    }

    private void GenerateRandomPaths()
    {
        while (Random.value > density)
        {
            CellPosition from = new CellPosition(Random.Range(0, width), Random.Range(0, height));
            CellPosition to = new CellPosition(Random.Range(0, width), Random.Range(0, height));
            if (Random.Range(0, 1) == 0)
                CreateStraightXYPathBetween(from, to);
            else
                CreateStraightYXPathBetween(from, to);
        }
    }

    private void CreateStraightXYPathBetween(CellPosition from, CellPosition to)
    {
        CellPosition currentpos = new CellPosition(from.x, to.x);

        int xDist = to.x - from.x;
        int yDist = to.y - from.y;

        int xStart = xDist > 0 ? from.x : to.x;
        int xEnd = xDist > 0 ? to.x : from.x;

        int yStart = yDist > 0 ? from.y : to.y;
        int yEnd = yDist > 0 ? to.y : from.y;

        int x = xStart, y = yStart;

        Cell cell = tabCells[x, y];

        for (x = xStart; x < xEnd; x++)
        {
            cell = tabCells[x, y];
            cell.walls[(int) DIRECTION.East] = false;
            if (x != xStart)
                cell.walls[(int) DIRECTION.West] = false;
        }
        x = xEnd;

        cell = tabCells[x, y];
        cell.walls[(int) DIRECTION.West] = false;

        if (yDist > 0 && xDist < 0 || xDist > 0 && yDist < 0)
            x = xStart;

        for (y = yStart; y < yEnd; y++)
        {
            cell = tabCells[x, y];
            cell.walls[(int) DIRECTION.North] = false;
            if (y != yStart)
                cell.walls[(int) DIRECTION.South] = false;
        }
        y = yEnd;
        cell = tabCells[x, y];
        cell.walls[(int) DIRECTION.South] = false;
    }

    private void CreateStraightYXPathBetween(CellPosition from, CellPosition to)
    {
        CellPosition currentpos = new CellPosition(from.x, to.x);

        int xDist = to.x - from.x;
        int yDist = to.y - from.y;

        int xStart = xDist > 0 ? from.x : to.x;
        int xEnd = xDist > 0 ? to.x : from.x;

        int yStart = yDist > 0 ? from.y : to.y;
        int yEnd = yDist > 0 ? to.y : from.y;

        int x = xStart, y = yStart;

        Cell cell = tabCells[x, y];

        for (y = yStart; y < yEnd; y++)
        {
            cell = tabCells[x, y];
            cell.walls[(int) DIRECTION.North] = false;
            if (y != yStart)
                cell.walls[(int) DIRECTION.South] = false;
        }
        y = yEnd;
        cell = tabCells[x, y];
        cell.walls[(int) DIRECTION.South] = false;

        if (yDist > 0 && xDist < 0 || xDist > 0 && yDist < 0)
            y = xStart;

        for (x = xStart; x < xEnd; x++)
        {
            cell = tabCells[x, y];
            cell.walls[(int) DIRECTION.East] = false;
            if (x != xStart)
                cell.walls[(int) DIRECTION.West] = false;
        }

        x = xEnd;
        cell = tabCells[x, y];
        cell.walls[(int) DIRECTION.West] = false;
    }


    private void GenerateDoors()
    {
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Cell cell = tabCells[x, y];
            Cell.CASE_TYPE cellType = cell.GetCaseType();
            if (cellType == Cell.CASE_TYPE.Border || cellType == Cell.CASE_TYPE.Plain)
            {
                DIRECTION corridorDir = GetNearTypeDirection(new CellPosition(x, y), Cell.CASE_TYPE.Corridor);
                if (corridorDir != DIRECTION.None)
                {
                    Cell corridor = GetCaseByDirection(cell.Pos, corridorDir);
                    if (corridor.walls[(int) GetOpposedDirection(corridorDir)])
                        continue;
                    cell.obj = Cell.CASE_OBJECT.Door;
                    cell.objDirection = corridorDir;
                    cell.walls[(int) corridorDir] = false;
                }
            }
        }
    }

    private bool HasObjNear(CellPosition pos)
    {
        foreach (Cell.CASE_OBJECT objType in Enum.GetValues(typeof(Cell.CASE_OBJECT)))
        {
            if (objType == Cell.CASE_OBJECT.None)
                continue;
            if (objType == Cell.CASE_OBJECT.Count)
                continue;
            if (GetNearObjDirection(pos, objType) != DIRECTION.None)
                return true;
        }
        return false;
    }

    private bool IsObjNear(CellPosition pos, Cell.CASE_OBJECT objType)
    {
        return GetNearObjDirection(pos, objType) != DIRECTION.None;
    }

    private DIRECTION GetNearObjDirection(CellPosition pos, Cell.CASE_OBJECT objType)
    {
        if (IsInWidth(pos.x + 1))
            if (tabCells[pos.x + 1, pos.y].obj == objType)
                return DIRECTION.East;
        if (IsInWidth(pos.x - 1))
            if (tabCells[pos.x - 1, pos.y].obj == objType)
                return DIRECTION.West;
        if (IsInHeight(pos.y + 1))
            if (tabCells[pos.x, pos.y + 1].obj == objType)
                return DIRECTION.North;
        if (IsInHeight(pos.y - 1))
            if (tabCells[pos.x, pos.y - 1].obj == objType)
                return DIRECTION.South;
        return DIRECTION.None;
    }

    private DIRECTION GetNearTypeDirection(CellPosition pos, Cell.CASE_TYPE caseType)
    {
        if (IsInWidth(pos.x + 1))
            if (tabCells[pos.x + 1, pos.y].GetCaseType() == caseType)
                return DIRECTION.East;
        if (IsInWidth(pos.x - 1))
            if (tabCells[pos.x - 1, pos.y].GetCaseType() == caseType)
                return DIRECTION.West;
        if (IsInHeight(pos.y + 1))
            if (tabCells[pos.x, pos.y + 1].GetCaseType() == caseType)
                return DIRECTION.North;
        if (IsInHeight(pos.y - 1))
            if (tabCells[pos.x, pos.y - 1].GetCaseType() == caseType)
                return DIRECTION.South;
        return DIRECTION.None;
    }

    private void BuildBoss()
    {
        GameObject boss = EnemyManager.Instance.CreateBoss(bossPrefab);
        int x = width / 2;
        int y = height * 2 / 3;

        boss.transform.position = new Vector3((x + .5f) * cellSize, 0, (y + 0.5f) * cellSize);
        //boss.transform.localScale = new Vector3(2, 2, 2);

        boss.transform.SetParent(transform);

        Enemy enemyScript = boss.GetComponent<Enemy>();
        enemyScript.OnDead += () =>
        {
            if (stairsObj)
                stairsObj.SetActive(true);
        };
    }

    private void BuildWall(CellPosition pos, DIRECTION dir)
    {
        Cell.CASE_TYPE caseType = tabCells[pos.x, pos.y].GetCaseType();
        if (caseType == Cell.CASE_TYPE.Corridor || caseType == Cell.CASE_TYPE.Border)
        {
            if (torchIndex == 0)
                BuildWallWithTorch(pos, dir);
            else
                BuildScaledObject(pos, dir, cubePrefab, normalWallScale, false, "Wall");
            torchIndex = (torchIndex + 1) % torchModulo;
        }
        else
        {
            BuildScaledObject(pos, dir, cubePrefab, normalWallScale, false, "Wall");
        }
    }

    private void BuildWallWithTorch(CellPosition pos, DIRECTION dir)
    {
        GameObject wall = BuildScaledObject(pos, dir, cubePrefab, normalWallScale, false, "Wall");

        GameObject torch = poolObjects.GetTorch(torchPrefab);
        torch.transform.position = wall.transform.position;
        torch.transform.rotation = wall.transform.rotation;
        torch.transform.Rotate(new Vector3(-20, 0));
        Vector3 torchPosition = torch.transform.position - torch.transform.forward * wall.transform.localScale.z * 0.6f;
        torch.transform.position = torchPosition;
        torch.transform.SetParent(transform);
    }

    private GameObject BuildOrientedObject(CellPosition pos, DIRECTION dir, GameObject prefab, bool centerIt,
        string name = "OrientedObj")
    {
        return BuildScalableOrientableObject(pos, dir, prefab, false, Vector3.one, centerIt, name);
    }

    private GameObject BuildScaledObject(CellPosition pos, DIRECTION dir, GameObject prefab, Vector3 scale,
        bool centerIt, string name = "OrientedObj")
    {
        return BuildScalableOrientableObject(pos, dir, prefab, true, scale, centerIt, name);
    }

    private GameObject BuildScalableOrientableObject(CellPosition pos, DIRECTION dir, GameObject prefab, bool scaleIt,
        Vector3 scale, bool centerIt, string name = "OrientedObj")
    {
        int x = pos.x, y = pos.y;
        GameObject obj;
        if (name == "Wall")
            obj = poolObjects.GetWall(prefab);
        else if (name == "Door")
            obj = poolObjects.GetDoor(prefab);
        else
            obj = Instantiate(prefab);
        obj.name = name;
        obj.transform.SetParent(transform);
        Vector3 realPos = new Vector3(x * cellSize, 0, y * cellSize);
        if (scaleIt)
        {
            Vector3 defaultScale = obj.transform.localScale;
            Vector3 newScale = new Vector3(scale.x == 0 ? defaultScale.x : scale.x * cellSize,
                scale.y == 0 ? defaultScale.y : scale.y * cellSize,
                scale.z == 0 ? defaultScale.z : scale.z * cellSize);
            obj.transform.localScale = newScale;
            if (scale.y > 0)
                realPos.y = newScale.y / 2;
        }

        float rotation = 0;
        if (centerIt)
        {
            realPos.x += 0.5f * cellSize;
            realPos.z += 0.5f * cellSize;
        }

        switch (dir)
        {
            case DIRECTION.North:
                if (!centerIt)
                {
                    realPos.x += 0.5f * cellSize;
                    realPos.z += cellSize;
                }
                break;
            case DIRECTION.East:
                rotation = rotationWall;
                if (!centerIt)
                {
                    realPos.x += cellSize;
                    realPos.z += 0.5f * cellSize;
                }
                break;
            case DIRECTION.South:
                rotation = rotationWall * 2;
                if (!centerIt)
                    realPos.x += 0.5f * cellSize;
                break;
            case DIRECTION.West:
                rotation = rotationWall * 3;
                if (!centerIt)
                    realPos.z += 0.5f * cellSize;
                break;
            default:
                break;
        }

        obj.transform.Rotate(new Vector3(0, rotation));
        obj.transform.position = realPos;

        return obj;
    }

    private void BuildDeco(Cell cell)
    {
        if (cell.obj != Cell.CASE_OBJECT.Decor)
            return;
        DIRECTION dir = decoRandomDir[cell.decoIndex]
            ? (DIRECTION) Random.Range(0, (int) DIRECTION.Count)
            : DIRECTION.None;
        BuildScaledObject(cell.Pos, dir, decoPrefabs[cell.decoIndex], decoScales[cell.decoIndex], true,
            "Deco " + cell.decoIndex);
    }

    private GameObject BuildObject(CellPosition pos, GameObject prefab, string name = "Obj")
    {
        int x = pos.x, y = pos.y;
        GameObject obj;
        if (name == "SpawnEnemy")
            obj = poolObjects.GetSpawn(prefab);
        else
            obj = Instantiate(prefab);

        obj.name = name;
        obj.transform.SetParent(transform);
        float defaultY = prefab.transform.position.y;
        Vector3 realPos = new Vector3((x + .5f) * cellSize, defaultY, (y + 0.5f) * cellSize);
        obj.transform.position = realPos;

        return obj;
    }

    private GameObject BuildCube(CellPosition pos, Vector3 scale, string name = "Cube")
    {
        int x = pos.x, y = pos.y;
        GameObject obj = Instantiate(cubePrefab);
        obj.name = name;
        obj.transform.SetParent(transform);
        Vector3 realPos = new Vector3((x + .5f) * cellSize, 0, (y + 0.5f) * cellSize);
        obj.transform.localScale = scale;
        realPos.y = scale.y / 2;
        obj.transform.position = realPos;

        return obj;
    }

    private void BuildStairs(CellPosition pos)
    {
        int x = pos.x, y = pos.y;
        GameObject obj = Instantiate(stairsPrefab);
        obj.name = "Stairs";
        obj.transform.SetParent(transform);
        Vector3 realPos = new Vector3((x + .5f) * cellSize, 0, (y + 0.5f) * cellSize);
        Vector3 scale = new Vector3(cellSize, cellSize, cellSize);
        obj.transform.localScale = scale;
        realPos.y = scale.y / 2;
        obj.transform.position = realPos;

        stairsObj = obj;
    }

    private void BuildTerrain()
    {
        int x = width, y = height;
        GameObject obj = poolObjects.GetGround(groundPrefab);
        obj.name = "Terrain";
        obj.transform.SetParent(transform);
        Vector3 realPos = new Vector3(x * cellSize / 2, 0, y * cellSize / 2);
        realPos.y = -(normalPlainScale.y / 2);
        obj.transform.position = realPos;
        obj.transform.localScale = normalPlainScale;
    }

    private void BuildRoof()
    {
        int x = width, y = height;
        GameObject obj = poolObjects.GetRoof(groundPrefab);
        obj.name = "Roof";
        obj.transform.SetParent(transform);
        Vector3 realPos = new Vector3(x * cellSize / 2, cellSize, y * cellSize / 2);
        realPos.y += normalPlainScale.y / 2;
        obj.transform.position = realPos;
        obj.transform.localScale = normalPlainScale;
    }

    private void BuildObject(CellPosition pos)
    {
        Cell cell = tabCells[pos.x, pos.y];
        switch (cell.obj)
        {
            case Cell.CASE_OBJECT.Door:
                BuildScaledObject(pos, cell.objDirection, doorPrefab, normalWallScale, false, "Door");
                break;
            case Cell.CASE_OBJECT.Tresor:
                GameObject treasure = BuildObject(pos, treasurePrefab, "Treasure");
                FillChestLoot(treasure.GetComponent<LootInventory>(), 5);
                return;
            case Cell.CASE_OBJECT.SpawnEnemy:
                GameObject spawn = BuildObject(pos, spawnEnemyPrefab, "SpawnEnemy");
                spawn.transform.SetParent(enemySpawners.transform);
                return;
            case Cell.CASE_OBJECT.Spawn:
                posPortal = BuildObject(pos, spawnPrefab, "Portal").transform;
                return;
            case Cell.CASE_OBJECT.Stairs:
                BuildStairs(stairsPos);
                return;
            case Cell.CASE_OBJECT.Decor:
                BuildDeco(cell);
                return;
            default:
                return;
        }
    }

    private void CloseMaze()
    {
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Cell cell = tabCells[x, y];
            if (y == 0)
                cell.walls[(int) DIRECTION.South] = true;
            if (x == 0)
                cell.walls[(int) DIRECTION.West] = true;
            if (y == width - 1)
                cell.walls[(int) DIRECTION.North] = true;
            if (x == height - 1)
                cell.walls[(int) DIRECTION.East] = true;
        }
    }

    private void OnDrawGizmos()
    {
        int gizmosWidth = width == 0 ? baseWidth : width;
        int gizmosHeight = height == 0 ? baseHeight : height;
        Gizmos.color = Color.black;
        for (int x = 0; x < gizmosWidth; x++)
            Gizmos.DrawLine(new Vector3(x * cellSize, 0, 0), new Vector3(x * cellSize, 0, gizmosHeight * cellSize));
        for (int y = 0; y < gizmosHeight; y++)
            Gizmos.DrawLine(new Vector3(0, 0, y * cellSize), new Vector3(gizmosWidth * cellSize, 0, y * cellSize));
    }


    private void FillChestLoot(LootInventory inventory, int maxSize)
    {
        ItemData[] items = Resources.LoadAll<ItemData>("Items/");

        ItemData item = items[Random.Range(0, items.Length)];
        if (!item.isQuestItem)
            inventory.Items.Add(item);

        for (int i = 1; i < maxSize; i++)
        {
            if (Random.Range(0, 3) == 0)
                return;
            item = items[Random.Range(0, items.Length)];
            if (!item.isQuestItem)
                inventory.Items.Add(item);
        }
    }

    private class PoolObjects
    {
        private readonly List<GameObject> doors;
        private readonly List<GameObject> spawns;
        private readonly List<GameObject> torchs;
        private readonly List<GameObject> walls;
        private int doorIndex;
        private GameObject ground;
        private GameObject roof;
        private int spawnIndex;
        private int torchIndex;
        private int wallIndex;

        public PoolObjects()
        {
            ground = null;
            roof = null;

            walls = new List<GameObject>();
            torchs = new List<GameObject>();
            doors = new List<GameObject>();
            spawns = new List<GameObject>();

            InitIndexes();
        }

        public void InitIndexes()
        {
            wallIndex = 0;
            torchIndex = 0;
            doorIndex = 0;
            spawnIndex = 0;
        }

        private GameObject ResetObject(GameObject obj, GameObject prefab)
        {
            obj.SetActive(true);
            obj.transform.position = prefab.transform.position;
            obj.transform.rotation = prefab.transform.rotation;
            Door door = obj.GetComponent<Door>();
            if (door)
                door.ResetPivot();
            return obj;
        }

        public GameObject GetRoof(GameObject roofPrefab)
        {
            if (!roof)
                roof = Instantiate(roofPrefab);
            return roof;
        }

        public GameObject GetGround(GameObject groundPrefab)
        {
            if (!ground)
                ground = Instantiate(groundPrefab);
            return ground;
        }

        public GameObject GetWall(GameObject wallPrefab)
        {
            if (wallIndex == walls.Count)
                walls.Add(Instantiate(wallPrefab));
            return ResetObject(walls[wallIndex++], wallPrefab);
        }

        public GameObject GetTorch(GameObject torchPrefab)
        {
            if (torchIndex == torchs.Count)
                torchs.Add(Instantiate(torchPrefab));
            return ResetObject(torchs[torchIndex++], torchPrefab);
        }

        public GameObject GetDoor(GameObject doorPrefab)
        {
            if (doorIndex == doors.Count)
                doors.Add(Instantiate(doorPrefab));
            return ResetObject(doors[doorIndex++], doorPrefab);
        }

        public GameObject GetSpawn(GameObject spawnPrefab)
        {
            if (spawnIndex == spawns.Count)
                spawns.Add(Instantiate(spawnPrefab));
            return ResetObject(spawns[spawnIndex++], spawnPrefab);
        }

        public bool IsInPool(GameObject obj)
        {
            if (roof == obj)
                return true;
            if (ground == obj)
                return true;
            if (walls.Contains(obj))
                return true;
            if (torchs.Contains(obj))
                return true;
            if (doors.Contains(obj))
                return true;
            if (spawns.Contains(obj))
                return true;
            return false;
        }

        public void Finish()
        {
            for (int i = wallIndex; i < walls.Count; i++)
                walls[i].SetActive(false);
            for (int i = torchIndex; i < torchs.Count; i++)
                torchs[i].SetActive(false);
            for (int i = doorIndex; i < doors.Count; i++)
                doors[i].SetActive(false);
            for (int i = spawnIndex; i < spawns.Count; i++)
                spawns[i].SetActive(false);
        }
    }

    #region defintions

    private class Cell
    {
        public enum CASE_OBJECT
        {
            Door,
            Tresor,
            SpawnEnemy,
            Decor,
            Spawn,
            Stairs,
            Count,
            None = -1
        }

        public enum CASE_TYPE
        {
            Plain,
            Border,
            Corridor,
            Trap,
            Closed
        }

        public readonly bool[] walls;

        public int decoIndex = -1;

        public CASE_OBJECT obj = CASE_OBJECT.None;
        public DIRECTION objDirection = DIRECTION.North;

        public Cell(int x, int y)
        {
            walls = new bool[(int) DIRECTION.Count];
            Close();
            Pos = new CellPosition(x, y);
        }

        public CellPosition Pos { get; private set; }

        public CASE_TYPE GetCaseType()
        {
            int wallsCount = 0;
            for (int i = 0; i < (int) DIRECTION.Count; i++)
                if (walls[i])
                    wallsCount++;
            switch (wallsCount)
            {
                case 0:
                    return CASE_TYPE.Plain;
                case 1:
                    return CASE_TYPE.Border;
                case 2:
                    if (walls[(int) DIRECTION.East] && walls[(int) DIRECTION.West])
                        return CASE_TYPE.Corridor;
                    if (walls[(int) DIRECTION.North] && walls[(int) DIRECTION.South])
                        return CASE_TYPE.Corridor;
                    return CASE_TYPE.Border;
                case 3:
                    return CASE_TYPE.Trap;
                case 4:
                    return CASE_TYPE.Closed;
                default:
                    return CASE_TYPE.Closed;
            }
        }

        public void Close()
        {
            for (int i = 0; i < (int) DIRECTION.Count; i++)
                walls[i] = true;
        }

        public void Open()
        {
            for (int i = 0; i < (int) DIRECTION.Count; i++)
                walls[i] = false;
        }
    }

    private struct CellPosition
    {
        public readonly int x;
        public readonly int y;

        public CellPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(CellPosition a, CellPosition b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(CellPosition x, CellPosition y)
        {
            return !(x == y);
        }
    }

    #endregion
}