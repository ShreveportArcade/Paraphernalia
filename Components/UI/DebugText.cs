using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class DebugText : MonoBehaviour, ILogHandler {

	private Text text;
	private ILogHandler baseHandler = Debug.unityLogger.logHandler;

	void Awake () {
		text = GetComponent<Text>();
		baseHandler = Debug.unityLogger.logHandler;
		Debug.unityLogger.logHandler = this;
	}

	public void LogFormat (LogType logType, UnityEngine.Object context, string format, params object[] args) {
		text.text += "\n" + string.Format(format, args);
        baseHandler.LogFormat (logType, context, format, args);
    }

    public void LogException (System.Exception exception, UnityEngine.Object context) {
		text.text += "\n" + exception.ToString() + " - " + context.ToString();
        baseHandler.LogException (exception, context);
    }
}
