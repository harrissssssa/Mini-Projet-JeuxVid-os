using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnAnimationEnd : StateMachineBehaviour
{
    [Tooltip("Nom de la scène à charger")]
    public string sceneToLoad;

    // Cette fonction s'exécute automatiquement quand l'animation se termine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SceneManager.LoadScene("SceneFinal");
    }
}