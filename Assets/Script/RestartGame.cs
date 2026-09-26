using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
   
    public void RecommencerLeJeu()
    {
        
        SceneManager.LoadScene("abandonedTown");
    }
}