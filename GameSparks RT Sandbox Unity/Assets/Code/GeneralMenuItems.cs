using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GeneralMenuItems : MonoBehaviour
{

	[MenuItem("Tools/Delete PlayerPrefs")]
	public static void DeletePlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
	}
}
