using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [Header("Daten")]
    [SerializeField] ArigamiBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    [Header("Referenzen")]
    [SerializeField] GameObject shadow;

    // Gecachte Komponenten für bessere Performance
    private RectTransform rectTransform;
    private Image unitImage;
    Vector3 originalPos;
    Color originalColor;
    private CanvasGroup canvasGroup;

    private RectTransform shadowRect;
    Vector3 originalShadowPos;

    public Arigami Arigami { get; set; }

    private void Awake()
    {
        // Komponenten einmalig beim Start suchen
        rectTransform = GetComponent<RectTransform>();
        unitImage = GetComponent<Image>();
        originalPos = unitImage.transform.localPosition;
        originalColor = unitImage.color;

        // Cache die CanvasGroup beim Start
        canvasGroup = GetComponent<CanvasGroup>();

        // Falls du vergessen hast, sie im Inspector hinzuzufügen, erstelle sie automatisch:
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();


        if (shadow != null)
        {
            shadowRect = shadow.GetComponent<RectTransform>();
            originalShadowPos = shadowRect.transform.localPosition;
        }
    }

    public void Setup()
    {
        Arigami = new Arigami(_base, level);

        // 1. Sprite zuweisen (Nutzt gecachtes unitImage)
        unitImage.sprite = isPlayerUnit ? Arigami.Base.BackSprite : Arigami.Base.FrontSprite;
        unitImage.preserveAspect = true;

        // 1. Die CanvasGroup wieder auf 100% (der Gruppe!) setzen
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        // 2. Das Hauptbild auf volle Deckkraft (innerhalb der Gruppe)
        var imgCol = unitImage.color;
        imgCol.a = 1f;
        unitImage.color = imgCol;

        // 3. Den Schatten auf seinen spezifischen Alpha-Wert (100/255) setzen
        if (shadowRect != null)
        {
            var sImg = shadowRect.GetComponent<Image>();
            if (sImg != null)
            {
                var sCol = sImg.color;
                // 100f / 255f entspricht dem Wert 100 im Unity Inspector
                sCol.a = 100f / 255f;
                sImg.color = sCol;
            }
        }

        // 3. GameObject wieder aktivieren (falls es in der Faint-Animation deaktiviert wurde)
        gameObject.SetActive(true);

        // 2. Skalierung berechnen
        float baseSize = isPlayerUnit ? 335f : 260f;
        float scaleFactor = Arigami.Base.BattleSpriteScale / 100f;
        float finalSize = baseSize * scaleFactor;

        rectTransform.sizeDelta = new Vector2(finalSize, finalSize);
        rectTransform.localScale = Vector3.one;

        // 3. Pivot-Wechsel ohne Springen (Nutzt gecachtes rectTransform)
        float targetPivotY = Arigami.Base.IsFlying ? 0.5f : 0f;
        float pivotDiff = targetPivotY - rectTransform.pivot.y;
        float yCorrection = pivotDiff * rectTransform.sizeDelta.y;

        rectTransform.pivot = new Vector2(0.5f, targetPivotY);
        rectTransform.anchoredPosition += new Vector2(0, yCorrection);

        // 4. Extra-Höhe nur für fliegende Gegner
        if (Arigami.Base.IsFlying && !isPlayerUnit)
        {
            rectTransform.anchoredPosition += new Vector2(0, 40f);
        }

        // 5. SCHATTEN-LOGIK (Nutzt gecachtes shadowRect)
        if (shadowRect != null)
        {
            float baseShadowWidth = isPlayerUnit ? 335f : 260f;
            float baseShadowHeight = isPlayerUnit ? 110f : 65f;

            if (Arigami.Base.IsFlying)
            {
                // Schatten etwas kleiner bei Fliegern
                float flyingShadowScale = 0.8f;
                shadowRect.sizeDelta = new Vector2(baseShadowWidth * scaleFactor * flyingShadowScale,
                                                   baseShadowHeight * scaleFactor * flyingShadowScale);
            }
            else
            {
                // Schatten normal proportional
                shadowRect.sizeDelta = new Vector2(baseShadowWidth * scaleFactor,
                                                   baseShadowHeight * scaleFactor);
            }
        }
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
        {
            unitImage.transform.localPosition = new Vector3(-500, originalPos.y);
            shadowRect.transform.localPosition = new Vector3(-500, originalShadowPos.y);
        }
        else
        {
            unitImage.transform.localPosition = new Vector3(350, originalPos.y);
            shadowRect.transform.localPosition = new Vector3(350, originalShadowPos.y);
        }

        unitImage.transform.DOLocalMoveX(originalPos.x, 1f);
        shadowRect.transform.DOLocalMoveX(originalShadowPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(unitImage.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
            sequence.Join(shadowRect.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        }
        else
        {
            sequence.Append(unitImage.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
            sequence.Join(shadowRect.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
        }

        sequence.Append(unitImage.transform.DOLocalMoveX(originalPos.x, 0.25f));
        sequence.Join(shadowRect.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void playHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(unitImage.DOColor(Color.gray, 0.1f));
        sequence.Append(unitImage.DOColor(originalColor, 0.1f));
    }

    public void playFaintAnimation()
    {
        var sequence = DOTween.Sequence();

        // BEIDE bewegen
        sequence.Append(unitImage.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(shadowRect.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));

        // BEIDE ausfaden (Voraussetzung: Beide haben eine Image-Komponente)
        sequence.Join(unitImage.DOFade(0f, 0.4f));
        sequence.Join(shadowRect.GetComponent<Image>().DOFade(0f, 0.4f));

        sequence.OnComplete(() => gameObject.SetActive(false));
    }

}