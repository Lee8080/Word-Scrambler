using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class hint : MonoBehaviour
{
    public static string hints;
    public int counter = 0;
    public void usehint()
    {   
        if(counter < 2)
        {
            #if UNITY_EDITOR
            EditorUtility.DisplayDialog("HINT", hints, "continue");
            #endif
            counter++;
        }
        else
        {
            #if UNITY_EDITOR
            EditorUtility.DisplayDialog("Oops", "Out of Hints" , "continue");
            #endif
        }
    }
}
