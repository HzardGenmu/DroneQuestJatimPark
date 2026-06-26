using UnityEngine;

public class DroneDisplayManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] drones;

    private int currentIndex;
    public GameObject CurrentDrone => drones[currentIndex];

    void Start()
    {
        ShowDrone(0);
    }

    public void NextDrone()
    {
        currentIndex++;

        if (currentIndex >= drones.Length)
            currentIndex = 0;

        ShowDrone(currentIndex);
    }

    public void PreviousDrone()
    {
        currentIndex--;

        if (currentIndex < 0)
            currentIndex = drones.Length - 1;

        ShowDrone(currentIndex);
    }

    void ShowDrone(int index)
    {
        for (int i = 0; i < drones.Length; i++)
        {
            drones[i].SetActive(i == index);
        }
    }
}