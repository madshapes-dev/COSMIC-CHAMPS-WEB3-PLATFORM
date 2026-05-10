using UnityEngine;
#if (ENABLE_INPUT_SYSTEM)
using UnityEngine.InputSystem;
#endif
using System.Collections;

[ExecuteInEditMode]
public class ScreenshotTaker : MonoBehaviour {

#if (ENABLE_INPUT_SYSTEM)
	public Key hotKey = Key.F12;
#else
	public KeyCode keyCode = KeyCode.F12;
#endif

	[SerializeField]
	GameObject[] disableDuringScreenshot;

	void Update ()
	{
#if (ENABLE_INPUT_SYSTEM)
		bool isPressingShift = (Keyboard.current[Key.LeftShift].isPressed || Keyboard.current[Key.RightShift].isPressed)
		if (Keyboard.current[hotKey].wasPressedThisFrame)
#else
		bool isPressingShift = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
		if (Input.GetKeyDown(keyCode))
#endif
		{
			SetObjects(false);
			if (isPressingShift)
				TakeScreenShot(true);
			else
				TakeScreenShot(false);
		}
	}

	public void TakeScreenShot(bool superSized)
	{
		StartCoroutine(TakeScreenshotNextFrame(superSized));
	}

	IEnumerator TakeScreenshotNextFrame(bool superSized)
	{
		yield return null;
		string filename = "s_" + System.DateTime.Now.ToString("yyyy-M-dd--HH-mm-ss") + ".png";
		ScreenCapture.CaptureScreenshot(filename, superSized ? 4 : 0);
		Debug.Log("Screenshot saved: " + filename + (superSized ? " (super sized)" : " (normal), use Shift+F8 for super-sized.") );
		yield return null;
		SetObjects(true);
	}

	void SetObjects(bool state) 
	{
		foreach (GameObject go in disableDuringScreenshot)
			go.SetActive(state);
	}

}
