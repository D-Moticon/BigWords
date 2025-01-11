using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RelicLibrary", menuName = "Scriptable Objects/RelicLibrary")]
public class RelicLibrary : ScriptableObject
{
    [System.Serializable]
    public class RelicInfo
    {
        public RelicSO relic;
    }

    public List<RelicInfo> relicInfos;

    public List<RelicSO> GetRelicSOs()
    {
        List<RelicSO> returnList = new List<RelicSO>();
        for (int i = 0; i < relicInfos.Count; i++)
        {
            returnList.Add(relicInfos[i].relic);
        }

        return returnList;
    }
}
