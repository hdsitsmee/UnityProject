using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager instance;

    [Header("All Guest Data")]
    public GuestData[] allGuests;

    [Header("All Drink Data")]
    public DrinkData[] allDrinks;

    void Start()
    {
        LoadAllProgress();
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void LoadAllProgress()
    {
        foreach (var guest in allGuests)
        {
            guest.currentSatisfaction = PrefsManager.GetFloat($"GUEST_SAT_{guest.guestId}", 0f);
            guest.isAscended = PrefsManager.GetBool($"GUEST_ASC_{guest.guestId}", false);
            guest.hasMet = PrefsManager.GetBool($"GUEST_MET_{guest.guestId}", false);
        }

        foreach (var drink in allDrinks)
        {
            drink.hasMade = PrefsManager.GetBool($"DRINK_MADE_{drink.drinkId}", false);
        }
    }

    public void SaveGuestProgress(GuestData guest)
    {
        PrefsManager.SetFloat($"GUEST_SAT_{guest.guestId}", guest.currentSatisfaction);
        PrefsManager.SetBool($"GUEST_ASC_{guest.guestId}", guest.isAscended);
        PrefsManager.SetBool($"GUEST_MET_{guest.guestId}", guest.hasMet);
    }

    public void SaveDrinkProgress(DrinkData drink)
    {
        PrefsManager.SetBool($"DRINK_MADE_{drink.drinkId}", drink.hasMade);
    }

    public void ResetAllProgress()
    {
        foreach (var guest in allGuests)
        {
            PrefsManager.DeleteKey($"GUEST_SAT_{guest.guestId}");
            PrefsManager.DeleteKey($"GUEST_ASC_{guest.guestId}");
            PrefsManager.DeleteKey($"GUEST_MET_{guest.guestId}");

            guest.currentSatisfaction = 0f;
            guest.isAscended = false;
            guest.hasMet = false;
        }

        foreach (var drink in allDrinks)
        {
            PrefsManager.DeleteKey($"DRINK_MADE_{drink.drinkId}");
            drink.hasMade = false;
        }
        Debug.Log("손님,음료 데이터 초기화 완료!");
    }
    // 무기 레벨 초기화
    public void ResetWeaponLevel()
    {
        PlayerPrefs.DeleteKey("WeaponLevel");
        Debug.Log("Weapon Level 데이터 초기화 완료!");
    }
    /*
    public void ClearAllPlayerPrefs()
    {
        PrefsManager.DeleteAll();

        foreach (var guest in allGuests)
        {
            guest.currentSatisfaction = 0f;
            guest.isAscended = false;
            guest.hasMet = false;
        }

        foreach (var drink in allDrinks)
        {
            drink.hasMade = false;
        }
    }*/
}
