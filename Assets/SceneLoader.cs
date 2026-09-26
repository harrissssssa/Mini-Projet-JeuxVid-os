using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void ChargerLaScene(string nomDeLaScene)
    {
        SceneManager.LoadScene(nomDeLaScene);
    }
}