using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    private CharacterMover mover;
    private Animator animator;

    [Header("Patrouillen Einstellungen")]
    public List<Vector2> patrolPath;
    public float waitTime = 2f;

    private int currentPathIndex = 0;
    private bool isReversing = false; // Steuert, ob wir vorwärts oder rückwärts laufen

    private void Awake()
    {
        mover = GetComponent<CharacterMover>();
        animator = GetComponent<Animator>();
        mover.SnapToGrid();
    }

    private void Start()
    {
        if (patrolPath != null && patrolPath.Count > 0)
        {
            StartCoroutine(PatrolRoutine());
        }
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            // Den Schalter für die Lauf-Animation aktualisieren
            if (animator != null)
            {
                animator.SetBool("isMoving", mover.IsMoving);
            }

            yield return new WaitForSeconds(waitTime);

            Vector2 nextDir = patrolPath[currentPathIndex];
            if (isReversing) nextDir *= -1;

            if (animator != null)
            {
                animator.SetFloat("moveX", nextDir.x);
                animator.SetFloat("moveY", nextDir.y);
                // Hier den Schalter auf TRUE setzen, da er gleich losläuft
                animator.SetBool("isMoving", true);
            }

            if (mover.TryMove(nextDir, out _))
            {
                UpdatePathIndex();
            }

            // Warten, bis der Schritt fertig ist
            while (mover.IsMoving)
            {
                yield return null;
            }

            // Stehen bleiben Animation
            if (animator != null)
            {
                animator.SetBool("isMoving", false);
            }
        }
    }


    private void UpdatePathIndex()
    {
        if (!isReversing)
        {
            currentPathIndex++;
            // Wenn wir am Ende der Liste angekommen sind...
            if (currentPathIndex >= patrolPath.Count)
            {
                isReversing = true; // ...drehen wir um
                currentPathIndex = patrolPath.Count - 1; // Starten beim letzten Element
            }
        }
        else
        {
            currentPathIndex--;
            // Wenn wir wieder am Anfang angekommen sind...
            if (currentPathIndex < 0)
            {
                isReversing = false; // ...laufen wir wieder vorwärts
                currentPathIndex = 0;
            }
        }
    }
}
