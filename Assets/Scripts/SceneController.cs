using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// TitleSceneからloadSceneする為だけのcrass
/// </summary>
public class SceneController : MonoBehaviour
{
    public void SceneJump()
    {
        SceneManager.LoadScene("MainScene");
    }
}
