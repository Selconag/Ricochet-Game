using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    public static System.Action IncreasePortalMultiplier;

    [SerializeField] private List<Ability> levelAbilities;
    private List<Ability> SelectedAbilities;
    [SerializeField] private float enchanceBulletSpawnSpeed = 0.2f;
    [SerializeField] private GameObject skill1,skill2,skill3;
    private int bounceLevel = 0, pierceLevel = 0;
    private int bounceLeft = 0, pierceLeft = 0;


    private static AbilityManager _instance;

	#region Getter Setters
	public static AbilityManager Instance
	{
        get => _instance;
	}

    public bool IsPiercingBullet
    {
        set
        {
            if (value)
            {
                pierceLevel++;
                pierceLeft++;
            }
        }
    }

    public int PiercingLevel
    {
        get { return pierceLevel; }
        set
        {
            pierceLeft = pierceLeft + value;

        }
    }

    public bool IsBouncingBullet
    {
        set
        {
            if (value)
            {
                bounceLevel++;
                bounceLeft++;
            }
        }
    }

    public int BouncingLevel
    {
        get { return bounceLevel; }
        set
        {
            bounceLeft = bounceLeft + value;

        }
    }

    public float BulletSpawnSpeed
	{
        set { Player.Instance.BulletSpawnSpeed = value; }
    }
#endregion

	#region Mono
	private void Awake()
    {
        _instance = this;

    }

	public void Set3AbilitiesOnUI()
	{
        SelectedAbilities = new List<Ability>();
        while (SelectedAbilities.Count < 3)
		{
            Ability ability = levelAbilities[Random.Range(0, levelAbilities.Count)];
            if (SelectedAbilities.Contains(ability))
            {
                continue;
            }
            else
            {
                SelectedAbilities.Add(ability);
            }
        }
        skill1.GetComponent<AbilityUI>().ShowAbility(SelectedAbilities[0]);
        skill2.GetComponent<AbilityUI>().ShowAbility(SelectedAbilities[1]);
        skill3.GetComponent<AbilityUI>().ShowAbility(SelectedAbilities[2]);
    }

    public void ActivateAbility(int number)
	{
        //Modular Version Template
        //switch (number)
        switch ((int)SelectedAbilities[number].AbilityType)
		{
            case 0:
                IsBouncingBullet = true;
                break;
            case 1:
                IsPiercingBullet = true;
                break;
            case 2:
                BulletSpawnSpeed = enchanceBulletSpawnSpeed;
                break;
            case 3: //Rainbow Mode
                Player.Instance.StartRainbowMode();
                break;
            case 4: //Increase Bullet Life Time
                //Nothing Happens
                break;
            case 5: //Portal Skill
                //Increase in multiplying amounts
                IncreasePortalMultiplier.Invoke();
                break;
        }
    }


    #endregion

}
