using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class LinkBarToHealthFlexible : MonoBehaviour
{
    public Slider slider;              // arrastra aquí este mismo Slider
    public MonoBehaviour healthSource; // arrastra aquí el Player (o su componente de vida)

    // Intenta leer estos nombres de campos/propiedades:
    // maxHealth / MaxHealth   y   currentHealth / CurrentHealth
    FieldInfo fMax, fCur; PropertyInfo pMax, pCur;

    void Awake()
    {
        if (!slider) slider = GetComponent<Slider>();
        CacheMembers();
    }

    void Start()
    {
        if (!slider) return;
        slider.minValue = 0f;
        var (cur, max) = ReadHealth();
        if (max > 0) slider.maxValue = max;
        slider.value = Mathf.Clamp(cur, 0, slider.maxValue);
    }

    void Update()
    {
        if (!slider) return;
        var (cur, max) = ReadHealth();
        if (max > 0) slider.maxValue = max;
        slider.value = Mathf.Clamp(cur, 0, slider.maxValue);
    }

    void CacheMembers()
    {
        if (!healthSource) return;
        var t = healthSource.GetType();
        // Campos
        fMax = t.GetField("maxHealth") ?? t.GetField("MaxHealth");
        fCur = t.GetField("currentHealth") ?? t.GetField("CurrentHealth");
        // Propiedades
        pMax = t.GetProperty("maxHealth") ?? t.GetProperty("MaxHealth");
        pCur = t.GetProperty("currentHealth") ?? t.GetProperty("CurrentHealth");
    }

    (float cur, float max) ReadHealth()
    {
        if (!healthSource) return (0f, 0f);
        object maxObj = null, curObj = null;

        if (fMax != null) maxObj = fMax.GetValue(healthSource);
        if (pMax != null) maxObj = pMax.GetValue(healthSource);

        if (fCur != null) curObj = fCur.GetValue(healthSource);
        if (pCur != null) curObj = pCur.GetValue(healthSource);

        float max = maxObj is float mf ? mf : (maxObj is int mi ? mi : 0f);
        float cur = curObj is float cf ? cf : (curObj is int ci ? ci : 0f);
        return (cur, max);
    }
}
