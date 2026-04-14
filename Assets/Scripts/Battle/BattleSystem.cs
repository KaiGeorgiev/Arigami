using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

enum BattleState { Start, PLayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

    public void StartBattle()
    {
        StartCoroutine(SetupBattle());

    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.setData(playerUnit.Arigami);
        enemyHud.setData(enemyUnit.Arigami);

        dialogBox.SetMovesNames(playerUnit.Arigami.Moves);

        yield return dialogBox.TypeDialog($"Ein wildes {enemyUnit.Arigami.Base.ArigamiName} erscheint.");

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PLayerAction;
        StartCoroutine(dialogBox.TypeDialog("Wähle eine Aktion"));
        dialogBox.EnablActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnablActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnablMoveSelector(true);
    }

    public void HandleUpdate()
    {
        switch (state)
        {
            case BattleState.PLayerAction: HandleActionSelection(); break;
            case BattleState.PlayerMove: HandleMoveSelection(); break;
        }
    }

    void HandleActionSelection()
    {
        currentAction = HandleGridSelection(currentAction, 4);
        dialogBox.UpdateActionSelection(currentAction);


        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (currentAction == 0) PlayerMove();
            else if (currentAction == 1) { /* Beutel */ }
            else if (currentAction == 2) { /* Arigami */ }
            else if (currentAction == 3) { /* Flucht */ }
        }
    }

    void HandleMoveSelection()
    {
        currentMove = HandleGridSelection(currentMove, playerUnit.Arigami.Moves.Count);
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Arigami.Moves[currentMove]);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            dialogBox.EnablMoveSelector(false);
            dialogBox.EnableDialogText(true);

            StartCoroutine(PerformPlayerMove());
        }
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;
        var move = playerUnit.Arigami.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Arigami.Base.ArigamiName} setzt {move.Base.MoveName} ein!");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.playHitAnimation();

        var damageDetails = enemyUnit.Arigami.TakeDamage(move, playerUnit.Arigami);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Arigami.Base.ArigamiName} wurde besiegt!");
            enemyUnit.playFaintAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }
    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Arigami.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Arigami.Base.ArigamiName} setzt {move.Base.MoveName} ein!");
        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        playerUnit.playHitAnimation();

        var damageDetails = playerUnit.Arigami.TakeDamage(move, enemyUnit.Arigami);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Arigami.Base.ArigamiName} wurde besiegt!");
            playerUnit.playFaintAnimation();
            
            yield return new WaitForSeconds(2f);
            OnBattleOver(false);
        }
        else
        {
            PlayerAction();
        }

    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        Debug.Log("test");
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("Ein Volltreffer!");
        }
        if (damageDetails.TypeEffectiveness > 1)
        {
            yield return dialogBox.TypeDialog("Das ist sehr effektiv!");
        }
        else if (damageDetails.TypeEffectiveness < 1)
        {
            yield return dialogBox.TypeDialog("Das ist nicht effektiv...");
        }
    }

    // Diese Methode gibt den neuen Index zurück
    int HandleGridSelection(int selectedItem, int maxItems)
    {
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            if (selectedItem <= 1 && selectedItem + 2 < maxItems)
                selectedItem += 2;
        }
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            if (selectedItem >= 2)
                selectedItem -= 2;
        }

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            if ((selectedItem == 0 || selectedItem == 2) && selectedItem + 1 < maxItems)
                selectedItem += 1;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            if (selectedItem == 1 || selectedItem == 3)
                selectedItem -= 1;
        }

        return Mathf.Clamp(selectedItem, 0, maxItems - 1);
    }
}
