using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] ArigamiBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    [SerializeField] GameObject shadow; 

    public Arigami Arigami { get; set; }
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Setup()
    {
        Arigami = new Arigami(_base, level);
        RectTransform rect = GetComponent<RectTransform>();
        Image img = GetComponent<Image>();

        // 1. Sprite zuweisen
        img.sprite = isPlayerUnit ? Arigami.Base.BackSprite : Arigami.Base.FrontSprite;

        // 2. Skalierung berechnen
        float baseSize = (isPlayerUnit) ? 335f : 260f;
        float scaleFactor = Arigami.Base.BattleSpriteScale / 100f;
        float finalSize = baseSize * scaleFactor;
       
        rect.sizeDelta = new Vector2(finalSize, finalSize);
       


        // 3. Pivot-Wechsel ohne Springen
        float targetPivotY = (Arigami.Base.IsFlying) ? 0.5f : 0f;
        float pivotDiff = targetPivotY - rect.pivot.y;
        float yCorrection = pivotDiff * rect.sizeDelta.y;

        rect.pivot = new Vector2(0.5f, targetPivotY);
        rect.anchoredPosition += new Vector2(0, yCorrection);

        // 4. Extra-Höhe nur für Flieger
        if (Arigami.Base.IsFlying && !isPlayerUnit)
        {
            rect.anchoredPosition += new Vector2(0, 40f);
        }

        // 5. SCHATTEN-LOGIK
        if (shadow != null)
        {
            RectTransform shadowRect = shadow.GetComponent<RectTransform>();

            // Wir definieren eine Grundgröße für den Schatten (z.B. 120 Pixel breit)
            float baseShadowWidth = (isPlayerUnit) ? 335f : 260f; ;
            float baseShadowHeight = (isPlayerUnit) ? 110f : 65f; ;

            if (Arigami.Base.IsFlying)
            {
                // Schatten unter den fliegenden Fisch (Unterkante minus kleiner Puffer)
                float distanceFromCenterToBottom = -(finalSize / 2f);
                //shadowRect.anchoredPosition = new Vector2(0, distanceFromCenterToBottom - 20f);

                // Schatten etwas kleiner skalieren für den Flug-Effekt (z.B. 80% der Normalgröße)
                float flyingShadowScale = 0.8f;
                shadowRect.sizeDelta = new Vector2(baseShadowWidth * scaleFactor * flyingShadowScale,
                                                   baseShadowHeight * scaleFactor * flyingShadowScale);
            }
            else
            {
                // Schatten direkt unter die Pfoten
                //shadowRect.anchoredPosition = Vector2.zero;

                // Schatten wächst proportional mit dem Pokémon
                shadowRect.sizeDelta = new Vector2(baseShadowWidth * scaleFactor,
                                                   baseShadowHeight * scaleFactor);
            }
        }

        rect.localScale = Vector3.one;
        img.preserveAspect = true;
    }
}