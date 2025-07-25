#if !DEBUG
public static class DebugSupporter
{
    [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        UnityEngine.Debug.unityLogger.logEnabled = false;
    }
}
#endif
