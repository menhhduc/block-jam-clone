using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

[System.Serializable]
public class CharacterSpawnData
{
    public GameObject characterPrefab;
    public List<Vector2Int> spawnPositions = new List<Vector2Int>();
}

public class LevelManager : MonoBehaviour
{
    [Header("Grid Reference")] 
    public TestGrid testGrid;

    [Header("Character Setup")] 
    public List<CharacterSpawnData> characterSpawnData = new List<CharacterSpawnData>();
    private List<GameObject> spawnedCharacters = new List<GameObject>();

    [Header("Pathfinding Target")] 
    public List<Vector2Int> endPos = new List<Vector2Int>();

    [Header("Finish Slots Setup")] 
    public List<Transform> finishSlots = new List<Transform>();
    private List<Character> charactersInFinish = new List<Character>();
    private int finishSlotIndex = 0;

    public float delay = 1f;

    public static LevelManager Instance { get; private set; }

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
        if (testGrid != null)
        {
            testGrid.CreateGrid();
            SpawnAllCharacters();
        }
    }

    #region Public Getters

    public List<Vector2Int> GetEndPos() => endPos;

    public Transform GetSortedFinishSlot(Character character)
    {
        ColorType characterColor = character.GetColorType();
        int insertIndex = FindInsertIndex(characterColor);
        ShiftCharactersDown(insertIndex);
        charactersInFinish.Insert(insertIndex, character);
        if (insertIndex < finishSlots.Count)
        {
            return finishSlots[insertIndex];
        }

        return null;
    }

    #endregion

    private void SpawnAllCharacters()
    {
        if (testGrid == null)
        {
            return;
        }

        foreach (var characterData in characterSpawnData)
        {
            foreach (var spawnPos in characterData.spawnPositions)
            {
                testGrid.SpawnCharacter(characterData.characterPrefab, spawnPos);
            }
        }
    }

    public void UpdateCharacterPosition(Vector2Int oldPos, Vector2Int newPos)
    {
        if (testGrid != null)
        {
            testGrid.UpdatePos(oldPos, newPos);
        }
    }

    public void AddCharacterToFinish(Character character)
    {
        CheckForThreeMatches();
        CheckGameOver();
    }

    private void CheckForThreeMatches()
    {
        for (int i = 0; i <= charactersInFinish.Count - 3; i++)
        {
            ColorType color = charactersInFinish[i].GetColorType();
            if (charactersInFinish[i + 1].GetColorType() == color && charactersInFinish[i + 2].GetColorType() == color)
            {
                StartCoroutine(RemoveCharacters(i));
                return;
            }
        }
    }
    
    private void CheckGameOver()
    {
        if (charactersInFinish.Count >= 7)
        {
            bool hasMatch = false;
            for (int i = 0; i <= charactersInFinish.Count - 3; i++)
            {
                ColorType color = charactersInFinish[i].GetColorType();
                if (charactersInFinish[i + 1].GetColorType() == color && charactersInFinish[i + 2].GetColorType() == color)
                {
                    hasMatch = true;
                    break;
                }
            }

            if (!hasMatch)
            {
                Debug.Log("GAME OVER! No more slot available.");
            }
        }
    }

    private IEnumerator RemoveCharacters(int startIndex)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < 3; i++)
        {
            Character character = charactersInFinish[startIndex];
            charactersInFinish.RemoveAt(startIndex);
            character.transform.DOScale(0f, 0.2f).OnComplete(() => Destroy(character.gameObject));
        }

        finishSlotIndex = charactersInFinish.Count;

        for (int i = 0; i < charactersInFinish.Count; i++)
        {
            MoveCharacterToSlot(charactersInFinish[i], finishSlots[i]);
        }
    }

    private int FindInsertIndex(ColorType targetColor)
    {
        int lastSameColorIndex = -1;

        for (int i = 0; i < charactersInFinish.Count; i++)
        {
            if (charactersInFinish[i].GetColorType() == targetColor)
            {
                lastSameColorIndex = i;
            }
        }

        if (lastSameColorIndex >= 0)
        {
            return lastSameColorIndex + 1;
        }

        return charactersInFinish.Count;
    }

    private void ShiftCharactersDown(int fromIndex)
    {
        for (int i = fromIndex; i < charactersInFinish.Count; i++)
        {
            MoveCharacterToSlot(charactersInFinish[i], finishSlots[i + 1]);
        }
    }

    private void MoveCharacterToSlot(Character character, Transform targetSlot)
    {
        character.transform.DOMove(targetSlot.position, 0.3f)
            .SetEase(Ease.OutQuad);
    }
}