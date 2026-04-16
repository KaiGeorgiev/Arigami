using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

enum BattleStates { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    BattleStates state;
    int currentAction;
    int currentMove;
    int currentMember;

    ArigamiParty playerParty;
    Arigami wildArigami;

    public void StartBattle(ArigamiParty playerParty, Arigami wildArigami)
    {
        this.playerParty = playerParty;
        this.wildArigami = wildArigami;
        StartCoroutine(SetupBattle());

    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyArigami());
        enemyUnit.Setup(wildArigami);

        partyScreen.Init();

        dialogBox.SetMovesNames(playerUnit.Arigami.Moves);

        yield return dialogBox.TypeDialog($"Ein wildes {enemyUnit.Arigami.Base.ArigamiName} erscheint.");

        ChooseFirstTurn();
    }

    void ChooseFirstTurn()
    {
        if (playerUnit.Arigami.Speed >= enemyUnit.Arigami.Speed)
        {
            ActionSelection();
        }else
        {
            StartCoroutine(EnemyMove());
        }
    }

    void BattleOver(bool won)
    {
        state = BattleStates.BattleOver;
        playerParty.Arigamis.ForEach(a => a.OnBattleOver());
        OnBattleOver(won);
    }

    void ActionSelection()
    {
        state = BattleStates.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("Wähle eine Aktion"));
        dialogBox.EnablActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleStates.PartyScreen;
        partyScreen.SetPartyData(playerParty.Arigamis);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleStates.MoveSelection;
        dialogBox.EnablActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnablMoveSelector(true);
    }

    IEnumerator PlayerMove()
    {
        state = BattleStates.PerformMove;

        var move = playerUnit.Arigami.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        // Wenn state nicht durch RunMove geändert wurde gehe zum nächten schritt
        if (state == BattleStates.PerformMove)
        {
            StartCoroutine(EnemyMove());
        }

    }

    IEnumerator EnemyMove()
    {
        state = BattleStates.PerformMove;

        var move = enemyUnit.Arigami.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        // Wenn state nicht durch RunMove geändert wurde gehe zum nächten schritt
        if (state == BattleStates.PerformMove)
        {
            ActionSelection();
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move) 
    {
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Arigami.Base.ArigamiName} setzt {move.Base.MoveName} ein!");

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        targetUnit.playHitAnimation();

        if(move.Base.Category == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, sourceUnit.Arigami, targetUnit.Arigami);

        }
        else
        {
            var damageDetails = targetUnit.Arigami.TakeDamage(move, sourceUnit.Arigami);
            yield return targetUnit.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }

        if (targetUnit.Arigami.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Arigami.Base.ArigamiName} wurde besiegt!");
            targetUnit.playFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(targetUnit);
        }

        // Status like brn oder psn hurt arigami ater the tunr
        sourceUnit.Arigami.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Arigami);
        yield return sourceUnit .Hud.UpdateHP();
        if (sourceUnit.Arigami.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Arigami.Base.ArigamiName} wurde besiegt!");
            sourceUnit.playFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
        }
    }

    IEnumerator RunMoveEffects(Move move, Arigami source, Arigami target)
    {
        var effects = move.Base.Effects;

        //Start Boosting
        if (effects.Boosts != null)
        {
            if (move.Base.Target == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts);
            }
        }

        // Status Conditions
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator ShowStatusChanges(Arigami arigami)
    {
        while (arigami.StatusChanges.Count > 0)
        {
            var message = arigami.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextArigami = playerParty.GetHealthyArigami();
            if (nextArigami != null)
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            BattleOver(true);
        }
    }

    public void HandleUpdate()
    {
        switch (state)
        {
            case BattleStates.ActionSelection: HandleActionSelection(); break;
            case BattleStates.MoveSelection: HandleMoveSelection(); break;
            case BattleStates.PartyScreen: HandlePartyScreenSelection(); break;
        }
    }

    void HandleActionSelection()
    {
        currentAction = HandleGridSelection(currentAction, 4);
        dialogBox.UpdateActionSelection(currentAction);


        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (currentAction == 0)
            {
                /* Kampf */
                MoveSelection();
            }
            else if (currentAction == 1) 
            { 
                /* Beutel */ 
            }
            else if (currentAction == 2) 
            {
                OpenPartyScreen();
                /* Arigami */ 
            }
            else if (currentAction == 3) 
            { 
                /* Flucht */ 
            }
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

            StartCoroutine(PlayerMove());
        }
        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            dialogBox.EnablMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();

        }
    }

    void HandlePartyScreenSelection()
    {
        currentMember = HandleGridSelection(currentMember, playerParty.Arigamis.Count);
        partyScreen.UpdateMemberSelection(currentMember);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            var selectedMember = playerParty.Arigamis[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("Du kannst kein besigtes Arigami auswählen");
                return;
            }
            if (selectedMember == playerUnit.Arigami)
            {
                partyScreen.SetMessageText("Das Ausgewählte Arigami ist bereist im Kampf");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleStates.Busy;
            StartCoroutine(SwitchArigami(selectedMember));
        }

        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator SwitchArigami(Arigami newArigami)
    {
        bool currentArigamiFainted = true;
        if(playerUnit.Arigami.HP > 0)
        {
            currentArigamiFainted = false;
            yield return dialogBox.TypeDialog($"Komm zurück {playerUnit.Arigami.Base.ArigamiName}");
            // Hier muss eine ANimation eingebaut werden
            playerUnit.playFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
       

        playerUnit.Setup(newArigami);

        dialogBox.SetMovesNames(newArigami.Moves);

        yield return dialogBox.TypeDialog($"Los {newArigami.Base.ArigamiName}!");

        if (currentArigamiFainted)
        {
            ChooseFirstTurn();
        }
        StartCoroutine(EnemyMove()); 

    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
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
            if (selectedItem + 2 < maxItems)
                selectedItem += 2;
        }
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            if (selectedItem >= 2)
                selectedItem -= 2;
        }

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            if (selectedItem + 1 < maxItems && selectedItem % 2 == 0)
                selectedItem += 1;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame && selectedItem % 2 != 0)
        {
            if (selectedItem > 0 )
                selectedItem -= 1;
        }

        return Mathf.Clamp(selectedItem, 0, maxItems - 1);
    }
}
