using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private Image m_AbilityImage;
    [SerializeField] private Text m_AbilityText;

    public void ShowAbility(Ability ability)
	{
        m_AbilityImage.sprite = ability.AbilityImage;
        m_AbilityText.text = ability.AbilityText;
    }
}
