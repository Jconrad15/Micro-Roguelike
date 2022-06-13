using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FollowerPrefabUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    public void Setup(Entity entity)
    {
        nameText.SetText(entity.CharacterName);
    }

}
