using UnityEngine;

public class UIManager : MonoBehaviour
{
    public PooledObjectData damageFloaterPooledObject;

    public Floater SpawnDamageFloater(Vector2 pos, float damageValue)
    {
        GameObject go = damageFloaterPooledObject.Spawn(pos, Quaternion.identity);
        Floater df = go.GetComponent<Floater>();
        if (df != null)
        {
            df.SetText(Helpers.RoundToDecimal(damageValue,1).ToString());
        }

        return df;
    }

    public Floater SpawnGenericFloater(Vector2 pos, string text)
    {
        GameObject go = damageFloaterPooledObject.Spawn(pos, Quaternion.identity);
        Floater df = go.GetComponent<Floater>();
        if (df != null)
        {
            df.SetText(text);
        }

        return df;
    }
}
