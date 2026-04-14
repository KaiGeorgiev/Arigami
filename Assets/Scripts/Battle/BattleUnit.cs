using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [Header("Einstellungen")]
    [SerializeField] bool isPlayerUnit;

    [Header("Referenzen")]
    [SerializeField] GameObject shadow;

    private RectTransform rectTransform;
    private Image unitImage;
    private RectTransform shadowRect;
    private Image shadowImage;

    private Vector3 basisPos;
    private Vector3 shadowBasisPos;
    private Vector3 originalPos;
    private Vector3 originalShadowPos;
    private Color originalColor;

    public Arigami Arigami { get; set; }

    private void Awake()
    {
        InitializeComponents();
    }

    public void Setup(Arigami arigami)
    {
        Arigami = arigami;

        ResetUnitState();
        ApplyArigamiData();
        CalculateLayout();

        PlayEnterAnimation();
    }

    #region Private Kapselung (Setup-Logik)

    private void InitializeComponents()
    {
        rectTransform = GetComponent<RectTransform>();
        unitImage = GetComponent<Image>();
        basisPos = rectTransform.localPosition;
        originalColor = unitImage.color;

        if (shadow != null)
        {
            shadowRect = shadow.GetComponent<RectTransform>();
            shadowImage = shadow.GetComponent<Image>();
            shadowBasisPos = shadowRect.localPosition;
            originalShadowPos = shadowRect.localPosition;
        }
    }

    private void ResetUnitState()
    {
        // Tweens stoppen
        unitImage.DOKill();
        rectTransform.DOKill();
        if (shadowImage != null) shadowImage.DOKill();

        // Transformationen zurücksetzen
        rectTransform.localPosition = basisPos;
        if (shadowRect != null) shadowRect.localPosition = shadowBasisPos;

        // Visuelles Reset
        gameObject.SetActive(true);
        unitImage.color = Color.white;

        if (shadowImage != null)
        {
            Color sCol = shadowImage.color;
            sCol.a = 100f / 255f;
            shadowImage.color = sCol;
        }
    }

    private void ApplyArigamiData()
    {
        unitImage.sprite = isPlayerUnit ? Arigami.Base.BackSprite : Arigami.Base.FrontSprite;
        unitImage.preserveAspect = true;
    }

    private void CalculateLayout()
    {
        // 1. Skalierung
        float baseSize = isPlayerUnit ? 335f : 260f;
        float scaleFactor = Arigami.Base.BattleSpriteScale / 100f;
        float finalSize = baseSize * scaleFactor;

        rectTransform.sizeDelta = new Vector2(finalSize, finalSize);
        rectTransform.localScale = Vector3.one;

        // 2. Position (Flieger-Offset)
        if (Arigami.Base.IsFlying && !isPlayerUnit)
        {
            rectTransform.anchoredPosition += new Vector2(0, 70f);
        }
        originalPos = rectTransform.localPosition;

        // 3. Schatten-Größe
        if (shadowRect != null)
        {
            float baseShadowWidth = isPlayerUnit ? 335f : 260f;
            float baseShadowHeight = isPlayerUnit ? 110f : 65f;
            float flyingMultiplier = Arigami.Base.IsFlying ? 0.8f : 1.0f;

            shadowRect.sizeDelta = new Vector2(
                baseShadowWidth * scaleFactor * flyingMultiplier,
                baseShadowHeight * scaleFactor * flyingMultiplier
            );
        }
    }

    #endregion

    #region Animationen (Public API)

    public void PlayEnterAnimation()
    {
        float startX = isPlayerUnit ? -500f : 350f;

        rectTransform.anchoredPosition = new Vector2(startX, rectTransform.anchoredPosition.y);
        if (shadowRect != null) shadowRect.anchoredPosition = new Vector2(startX, shadowRect.anchoredPosition.y);

        rectTransform.DOAnchorPosX(originalPos.x, 1f).SetEase(Ease.OutCubic);
        if (shadowRect != null) shadowRect.DOAnchorPosX(originalShadowPos.x, 1f).SetEase(Ease.OutCubic);
    }

    public void PlayAttackAnimation()
    {
        float moveDistance = isPlayerUnit ? 50f : -50f;
        var seq = DOTween.Sequence();
        seq.Append(rectTransform.DOAnchorPosX(originalPos.x + moveDistance, 0.2f));
        seq.Append(rectTransform.DOAnchorPosX(originalPos.x, 0.2f));
    }

    public void playHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(unitImage.DOColor(Color.gray, 0.1f));
        sequence.Append(unitImage.DOColor(Color.white, 0.1f));
    }

    public void playFaintAnimation()
    {
        unitImage.DOKill();
        var sequence = DOTween.Sequence();

        sequence.Append(unitImage.DOColor(Color.white, 0.05f));
        sequence.Join(rectTransform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        if (shadowRect != null) sequence.Join(shadowRect.DOLocalMoveY(shadowRect.localPosition.y - 150f, 0.5f));

        sequence.Join(unitImage.DOFade(0f, 0.4f));
        if (shadowImage != null) sequence.Join(shadowImage.DOFade(0f, 0.4f));

        sequence.OnComplete(() => gameObject.SetActive(false));
    }

    #endregion
}