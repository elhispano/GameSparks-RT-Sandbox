using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GeneralMenuItems : MonoBehaviour
{
	#if UNITY_EDITOR
	[MenuItem("Tools/Delete PlayerPrefs")]
	public static void DeletePlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
	}
	#endif
}
