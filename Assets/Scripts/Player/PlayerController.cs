using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    private CharacterMover mover;
    private Animator animator;
    private Vector2 input;

    public event Action OnEncountered;

    private void Awake()
    {
        mover = GetComponent<CharacterMover>();
        animator = GetComponent<Animator>();
        mover.SnapToGrid();
    }
    private void OnEnable()
    {
        // Sicherstellen, dass mover existiert (falls OnEnable vor Awake kommt)
        if (mover == null) mover = GetComponent<CharacterMover>();

        // Das Event vom Mover abfangen und an den GameController weiterleiten
        mover.OnEncountered += HandleMoverEncounter;
    }

    private void OnDisable()
    {
        if (mover != null)
            mover.OnEncountered -= HandleMoverEncounter;
    }

    private void HandleMoverEncounter()
    {
        // Leitet das Event an den GameController weiter
        OnEncountered?.Invoke();
    }

    public void HandleUpdate()
    {
        if (mover.IsMoving) return;

        UpdateInput();

        if (input != Vector2.zero)
        {
            animator.SetFloat("moveX", input.x);
            animator.SetFloat("moveY", input.y);

            // Der Befehl an die "Muskeln"
            mover.TryMove(input, out _);
        }

        animator.SetBool("isMoving", mover.IsMoving);
    }

    private void UpdateInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float x = 0, y = 0;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) y = 1;
        else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) y = -1;

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x = 1;
        else if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x = -1;

        // Diagonal-Sperre mit Priorität auf der aktuellen Achse
        if (x != 0 && y != 0)
        {
            if (Mathf.Abs(input.x) > 0) y = 0;
            else x = 0;
        }
        input = new Vector2(x, y);
    }
}
