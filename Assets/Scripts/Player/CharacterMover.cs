using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class CharacterMover : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpSpeed = 3f;
    public Tilemap tilemap;
    public Tilemap encounterTilemap; // Hier die Encounter-Map im Inspektor zuweisen!

    public bool IsMoving { get; private set; }
    public bool isPlayer; // Im Inspektor beim Spieler auf TRUE, bei NPCs auf FALSE setzen

    public event Action OnEncountered;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }
    public void SnapToGrid()
    {
        transform.position = new Vector3(
            Mathf.Floor(transform.position.x) + 0.5f,
            Mathf.Floor(transform.position.y - 0.2f) + 0.7f,
            0);
    }

    public bool TryMove(Vector2 direction, out Vector3 targetPos)
    {
        targetPos = CalculateLogicalTarget(direction);
        if (IsMoving) return false;

        WallType wallResult = CanMove(direction);

        if (wallResult == WallType.Open)
        {
            StartCoroutine(MoveRoutine(targetPos, moveSpeed));
            return true;
        }
        else if (wallResult == WallType.Jumpable)
        {
            targetPos = CalculateLogicalTarget(direction * 2);
            StartCoroutine(JumpRoutine(targetPos));
            return true;
        }

        return false;
    }

    private WallType CanMove(Vector2 moveDir)
    {
        Vector3 targetPos = CalculateLogicalTarget(moveDir);
        Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.2f);

        if (hit != null)
        {
            bool isCamera = hit.gameObject.layer == LayerMask.NameToLayer("CameraBouncs");
            bool isMe = hit.transform == this.transform;

            if (!isCamera && !isMe) return WallType.Solid;
        }

        Vector3 currentLogicalPos = new Vector3(Mathf.Floor(transform.position.x) + 0.5f, Mathf.Floor(transform.position.y - 0.2f) + 0.5f, 0);
        Vector3Int currentCell = tilemap.WorldToCell(currentLogicalPos);
        Vector3Int targetCell = tilemap.WorldToCell(currentLogicalPos + (Vector3)moveDir);

        RPGTile currentTile = tilemap.GetTile(currentCell) as RPGTile;
        RPGTile targetTile = tilemap.GetTile(targetCell) as RPGTile;

        if (currentTile != null)
        {
            if (moveDir.y > 0 && currentTile.wallTop != WallType.Open) return currentTile.wallTop;
            if (moveDir.y < 0 && currentTile.wallBottom != WallType.Open) return currentTile.wallBottom;
            if (moveDir.x > 0 && currentTile.wallRight != WallType.Open) return currentTile.wallRight;
            if (moveDir.x < 0 && currentTile.wallLeft != WallType.Open) return currentTile.wallLeft;
        }

        if (targetTile != null)
        {
            if (targetTile.isFullBlock) return WallType.Solid;
            if (moveDir.y > 0 && targetTile.wallBottom != WallType.Open) return targetTile.wallBottom;
            if (moveDir.y < 0 && targetTile.wallTop != WallType.Open) return targetTile.wallTop;
            if (moveDir.x > 0 && targetTile.wallLeft != WallType.Open) return targetTile.wallLeft;
            if (moveDir.x < 0 && targetTile.wallRight != WallType.Open) return targetTile.wallRight;
        }

        return WallType.Open;
    }

    private Vector3 CalculateLogicalTarget(Vector2 dir)
    {
        return new Vector3(
            Mathf.Floor(transform.position.x) + 0.5f + dir.x,
            Mathf.Floor(transform.position.y - 0.2f) + 0.5f + dir.y,
            0);
    }

    private IEnumerator MoveRoutine(Vector3 targetPos, float speed)
    {
        IsMoving = true;
        Vector3 visualTargetPos = new Vector3(targetPos.x, targetPos.y + 0.2f, targetPos.z);

        while (Vector3.Distance(transform.position, visualTargetPos) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, visualTargetPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = visualTargetPos;
        IsMoving = false;

        // --- ENCOUNTER CHECK ---
        CheckForEncounter(targetPos);
    }

    private IEnumerator JumpRoutine(Vector3 targetPos)
    {
        IsMoving = true;
        Vector3 startPos = transform.position;
        Vector3 visualTargetPos = new Vector3(targetPos.x, targetPos.y + 0.2f, targetPos.z);

        float progress = 0;
        while (progress < 1f)
        {
            progress += Time.deltaTime * jumpSpeed;
            Vector3 currentPos = Vector3.Lerp(startPos, visualTargetPos, progress);
            currentPos.y += Mathf.Sin(progress * Mathf.PI) * 0.5f;

            transform.position = currentPos;
            yield return null;
        }

        transform.position = visualTargetPos;
        IsMoving = false;

        // --- ENCOUNTER CHECK (AUCH NACH SPRUNG) ---
        CheckForEncounter(targetPos);
    }

    private void CheckForEncounter(Vector3 position)
    {
        if (!isPlayer) return;

        Vector3Int cell = encounterTilemap.WorldToCell(position);
        TileBase tile = encounterTilemap.GetTile(cell);

        if (tile is RPGTile rpgTile && rpgTile.canHaveEncounter)
        {
            if (UnityEngine.Random.Range(1,101) <= 10)
            {
                animator.SetBool("isMoving", IsMoving);
                OnEncountered?.Invoke();

            }

        }
    }
}
