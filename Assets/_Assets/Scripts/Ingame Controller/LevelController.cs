using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using DG.Tweening;

#region Map Setup Properties
[System.Serializable]
public class Column
{
    public List<Slot> slots = new List<Slot>();
}

[System.Serializable]
public class Stage
{
    public Transform slotContainer;
    public Transform groundSlotContainer;
    public List<Slot> slots = new List<Slot>();
    public List<Column> groundSlots = new List<Column>();
    public Transform mobContainer, tunnelContainer;
    public float cameraPosX;
    public List<Vector2Int> outSlots = new List<Vector2Int>(); // Initialize here
    public List<TestMob> testMobs = new List<TestMob>();
}
#endregion

public class LevelController : MonoBehaviour
{
    [Header("Stage properties")]
    [SerializeField] private List<Stage> stages = new List<Stage>();
    [SerializeField] private int setupStage = 0;
    [SerializeField] private Transform edge;
    [SerializeField] private ColorConfig colorConfig;
    [SerializeField] private TestGridAStar grid;

    [Header("Spawn settings")]
    [SerializeField] private int column;
    [SerializeField] private int row;
    [SerializeField] private GameObject mobPrefab, tunnelPrefab;

    float edgeX;
    int currentSlot = 0;
    int currentStage = 0;

    public static LevelController Instance { get; private set; }
    public ColorConfig ColorConfig => colorConfig;
    public Transform MobContainer => stages[currentStage].mobContainer;
    bool interactable = false;

    #region Buttons
    [Button]
    void SpawnMob()
    {
        GameObject clone = Instantiate(mobPrefab, stages[setupStage].groundSlots[column - 1].slots[row - 1].transform.position, Quaternion.identity, stages[setupStage].mobContainer);
        TestMob scr = clone.GetComponent<TestMob>();
        scr.Pos = new Vector2Int(row - 1, column - 1);
        scr.SetUpAfterSpawn(ColorType.Blue);

        stages[setupStage].testMobs.Add(scr);
    }

    [Button]
    void SetUp()
    {
        stages[setupStage].slots.Clear();
        for (int i = 0; i < stages[setupStage].slotContainer.childCount; i++)
        {
            stages[setupStage].slots.Add(stages[setupStage].slotContainer.GetChild(i).GetComponent<Slot>());
        }

        stages[setupStage].groundSlots.Clear();
        for (int i = 0; i < stages[setupStage].groundSlotContainer.childCount; i++)
        {
            Column col = new Column();
            Transform column = stages[setupStage].groundSlotContainer.GetChild(i);
            for (int j = 0; j < column.childCount; j++)
            {
                col.slots.Add(column.GetChild(j).GetComponent<Slot>());
            }
            stages[setupStage].groundSlots.Add(col);
        }
    }
    #endregion

    public bool Interactable
    {
        set => interactable = value;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (edge == null)
        {
            return;
        }

        if (stages == null || stages.Count == 0)
        {
            return;
        }

        if (currentStage >= stages.Count)
        {
            return;
        }

        edgeX = edge.position.x;
        SetUpObject();
        GenerateGrid();
        interactable = true;
    }

    private void Update()
    {
        if (!interactable)
            return;
        Click();
    }

    private void SetUpObject()
    {
        foreach (Stage stage in stages)
        {
            if (stage.mobContainer == null || stage.groundSlots == null) continue;

            for (int i = 0; i < stage.mobContainer.childCount; i++)
            {
                TestMob temp = stage.mobContainer.GetChild(i).GetComponent<TestMob>();
                if (temp != null && temp.Pos.y < stage.groundSlots.Count && temp.Pos.x < stage.groundSlots[temp.Pos.y].slots.Count)
                {
                    stage.groundSlots[temp.Pos.y].slots[temp.Pos.x].Mob = temp;
                }
            }
        }
    }

    private void HandleInput(Vector3 position)
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(position);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Mob"))
            {
                TestMob component = hit.collider.GetComponent<TestMob>();
                if (component != null)
                {
                    MoveMob(component.Pos, component);
                    Destroy(hit.collider);
                }
            }
        }
    }

    private void Click()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 pos = Input.mousePosition;
            HandleInput(pos);
        }
    }

    private void GenerateGrid()
    {
        if (grid == null)
        {
            return;
        }

        if (stages == null || currentStage >= stages.Count)
        {
            return;
        }

        if (stages[currentStage].groundSlots == null || stages[currentStage].groundSlots.Count == 0)
        {
            return;
        }

        if (grid.CreateGrid(stages[currentStage].groundSlots))
        {
            FindPath();
        }
    }

    private void FindPath()
    {
        if (stages == null || currentStage >= stages.Count)
        {
            return;
        }

        Stage currentStageData = stages[currentStage];

        if (currentStageData.outSlots == null)
        {
            return;
        }

        if (currentStageData.groundSlots == null)
        {
            return;
        }

        if (currentStageData.testMobs == null)
        {
            return;
        }

        List<Vector2Int> endPoints = new List<Vector2Int>();

        foreach (Vector2Int outSlot in currentStageData.outSlots)
        {
            Slot currentSlot = currentStageData.groundSlots[outSlot.y].slots[outSlot.x];

            if (currentSlot != null && currentSlot.Mob == null)
            {
                endPoints.Add(outSlot);
            }
        }

        foreach (TestMob mob in currentStageData.testMobs)
        {
            if (mob == null) continue;

            List<Slot> shortestPath = null;

            foreach (Vector2Int endPoint in endPoints)
            {
                List<Slot> path = TestPathfinding.FindPath(mob.Pos, endPoint, grid);
                if (path == null)
                {
                    continue;
                }
                if (shortestPath == null || shortestPath.Count > path.Count)
                {
                    shortestPath = path;
                }
            }
            mob.Path = shortestPath;
        }
    }

    public void MoveMob(Vector2Int mobPos, TestMob mob)
    {
        stages[currentStage].groundSlots[mobPos.y].slots[mobPos.x].Mob = null;

        if (grid != null && grid.grid != null &&
            mobPos.x >= 0 && mobPos.x < grid.grid.GetLength(0) &&
            mobPos.y >= 0 && mobPos.y < grid.grid.GetLength(1))
        {
            grid.grid[mobPos.x, mobPos.y].UpdateWalkableState();
            FindPath();
        }

        stages[currentStage].testMobs.Remove(mob);
        mob.MoveToEdge(edgeX);
        StartCoroutine(CorMoveMob(mob));
    }

    IEnumerator CorMoveMob(TestMob mob)
    {
        yield return new WaitForSeconds(1.0f);
        if (mob == null || mob.gameObject == null)
            yield break;
        if (currentSlot >= stages[currentStage].slots.Count)
        {
            Debug.LogError($"Current slot index ({currentSlot}) is out of range! Available slots: {stages[currentStage].slots.Count}");
            Lost();
            yield break;
        }
        if (stages[currentStage].slots[currentSlot] == null)
        {
            Debug.LogError($"Slot at index {currentSlot} is null!");
            yield break;
        }

        Debug.Log($"Moving mob {mob.gameObject.name} to slot {currentSlot}");
        Vector3 slotPosition = stages[currentStage].slots[currentSlot].transform.position;
        mob.transform.position = new Vector3(slotPosition.x, slotPosition.y, slotPosition.z);

        stages[currentStage].slots[currentSlot].Mob = mob;
        SortMob(currentSlot);
        currentSlot++;
    }

    private void SortMob(int currentSlot)
    {
        for (int i = currentSlot - 1; i >= 0; i--)
        {
            if (stages[currentStage].slots[i].Mob.ColorType == stages[currentStage].slots[currentSlot].Mob.ColorType)
            {
                if (i == currentSlot - 1)
                    break;
                TestMob tempMob = stages[currentStage].slots[currentSlot].Mob;
                for (int j = currentSlot - 1; j > i; j--)
                {
                    stages[currentStage].slots[j].Mob.Move(new Vector3(stages[currentStage].slots[j + 1].transform.position.x, stages[currentStage].slots[j + 1].transform.position.y, 0));
                    stages[currentStage].slots[j + 1].Mob = stages[currentStage].slots[j].Mob;
                }
                tempMob.Move(new Vector3(stages[currentStage].slots[i + 1].transform.position.x, stages[currentStage].slots[i + 1].transform.position.y, 0));
                stages[currentStage].slots[i + 1].Mob = tempMob;
                StartCoroutine(CorConnect(i + 1));
                return;
            }
        }
        stages[currentStage].slots[currentSlot].Mob.Move(new Vector3(stages[currentStage].slots[currentSlot].transform.position.x, stages[currentStage].slots[currentSlot].transform.position.y, 0));
        StartCoroutine(CorConnect(currentSlot));
    }

    IEnumerator CorConnect(int currentSlot)
    {
        if (currentSlot >= 2 && stages[currentStage].slots[currentSlot - 1].Mob.ColorType == stages[currentStage].slots[currentSlot].Mob.ColorType && stages[currentStage].slots[currentSlot - 2].Mob.ColorType == stages[currentStage].slots[currentSlot].Mob.ColorType)
        {
            TestMob mob1 = stages[currentStage].slots[currentSlot - 2].Mob, mob2 = stages[currentStage].slots[currentSlot - 1].Mob, mob3 = stages[currentStage].slots[currentSlot].Mob;
            for (int i = currentSlot + 1; i <= this.currentSlot; i++)
            {
                if (stages[currentStage].slots[i].Mob == null)
                    break;

                stages[currentStage].slots[i].Mob.Move(new Vector3(stages[currentStage].slots[i - 3].transform.position.x, stages[currentStage].slots[i - 3].transform.position.y, 0));
                stages[currentStage].slots[i - 3].Mob = stages[currentStage].slots[i].Mob;
                stages[currentStage].slots[i].Mob = null;
            }
            this.currentSlot -= 3;
            mob1.Disappear();
            yield return new WaitForSeconds(0.1f);
            mob2.Disappear();
            yield return new WaitForSeconds(0.1f);
            mob3.Disappear();
        }
        if (this.currentSlot == 7)
        {
            Lost();
        }
    }

    public void Victory()
    {
        currentStage++;
        if (currentStage >= stages.Count)
        {
            Debug.Log("All stages completed!");
            return;
        }

        currentSlot = 0;
        GenerateGrid();
    }

    void Lost()
    {
        Debug.Log("Game Over - Lost!");
    }
}