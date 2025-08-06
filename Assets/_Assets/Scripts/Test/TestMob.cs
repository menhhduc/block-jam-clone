using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

public class TestMob : MonoBehaviour
{
    [SerializeField] private CircleCollider2D _collider;
    [SerializeField] private Vector2Int pos;
    [SerializeField] ColorType colorType;
    [SerializeField] private bool activate = false;
    [SerializeField, ReadOnly] List<Slot> path;

    protected Material mobColor;
    public ColorType ColorType => colorType;
    Tween moveTween;
    Coroutine corMove;
    Coroutine corMoveToEdge;

    public Vector2Int Pos
    {
        get => pos;
        set => pos = value;
    }

    public List<Slot> Path
    {
        set
        {
            if (value != null)
            {
                Activate();
                path = value;
            }
        }
    }

    protected virtual void Start()
    {
        SetUp();
    }

    public void SetUpAfterSpawn(ColorType mobType)
    {
        colorType = mobType;
    }

    protected virtual void SetUp()
    {
        mobColor = Instantiate(LevelController.Instance.ColorConfig.ColorDict[colorType]);
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = mobColor;
        }

        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, 0);
        _collider.enabled = false;
    }

    public void Activate()
    {
        if (activate)
        {
            return;
        }
        activate = true;
        _collider.enabled = true;
    }

    public void MoveToEdge(float edgeX)
    {
        if (corMoveToEdge != null)
            StopCoroutine(corMoveToEdge);
        corMoveToEdge = StartCoroutine(CorMoveToEdge(edgeX));
    }

    IEnumerator CorMoveToEdge(float edgeX)
    {
        float durationPerSlot = 0.5f / (path.Count + 1);
        bool down = true;

        foreach (Slot slot in path)
        {
            Vector3 destination = slot.transform.position;
            if (Mathf.Abs(destination.x - transform.position.x) > 0.5f && down)
            {
                down = false;
                yield return new WaitForSeconds(durationPerSlot * 0.2f);
                transform.DOMove(destination, durationPerSlot * 0.8f).SetEase(Ease.Linear);
                yield return new WaitForSeconds(durationPerSlot * 0.8f);
                continue;
            }
            if (Mathf.Abs(destination.y - transform.position.y) > 0.5f && !down)
            {
                down = true;
                yield return new WaitForSeconds(durationPerSlot * 0.2f);
                transform.DOMove(destination, durationPerSlot * 0.8f).SetEase(Ease.Linear);
                yield return new WaitForSeconds(durationPerSlot * 0.8f);
                continue;
            }
            transform.DOMove(destination, durationPerSlot).SetEase(Ease.Linear);
            yield return new WaitForSeconds(durationPerSlot);
        }
        transform.DOMoveZ(edgeX, durationPerSlot).SetEase(Ease.Linear);
    }

    public void Move(Vector3 destination)
    {
        if (corMove != null)
            StopCoroutine(corMove);
        corMove = StartCoroutine(CorMove(destination));
    }

    IEnumerator CorMove(Vector3 destination)
    {
        if (this == null || gameObject == null)
            yield break;

        Vector2 direction = new Vector2(destination.x - transform.position.x, destination.y - transform.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        moveTween?.Kill();
        moveTween = transform.DORotate(new Vector3(0, 0, angle), 0.1f).SetEase(Ease.OutFlash);
        yield return moveTween.WaitForCompletion();

        if (this == null || gameObject == null)
            yield break;

        moveTween = transform.DOMove(destination, 0.5f);
        yield return moveTween.WaitForCompletion();

        if (this == null || gameObject == null)
            yield break;

        moveTween = transform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.OutFlash);
    }

    public void Disappear()
    {
        transform.DOKill();
        transform.DOMoveY(-0.5f, 0.1f);

        transform.DOScale(0.25f, 0.25f).OnComplete(() =>
        {
            if (this != null && gameObject != null)
            {
                transform.DOScale(0, 0.25f).SetEase(Ease.OutFlash).OnComplete(() =>
                {
                    if (mobColor != null)
                        Destroy(mobColor);
                    if (gameObject != null)
                        Destroy(gameObject);
                });
            }
        });
    }

    private void OnDestroy()
    {
        if (transform != null)
        {
            transform.DOKill();
        }

        if (corMove != null)
            StopCoroutine(corMove);
        if (corMoveToEdge != null)
            StopCoroutine(corMoveToEdge);

        if (LevelController.Instance != null && LevelController.Instance.MobContainer.childCount == 1)
            LevelController.Instance.Victory();
    }
}