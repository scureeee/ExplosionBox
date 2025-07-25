#if RELEASE
public static class Debug
{
    public static void Log(object message) { }
    public static void Log(object message, object context) { }
    public static void LogWarning(object message) { }
    public static void LogWarning(object message, object context) { }
    public static void LogError(object message) { }
    public static void LogError(object message, object context) { }
}
#endif
