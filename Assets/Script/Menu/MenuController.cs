using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SwitchShaderKeyword(string[] keywords, int index)
    {
        for (int i = 0; i < keywords.Length; i++)
        {
            if (i == index)
            {
                Shader.EnableKeyword(keywords[i]);
            }
            else
            {
                Shader.DisableKeyword(keywords[i]);
            }
        }
    }

    private static readonly string[] outlineKeywords = { "_OUTLINE_USE_NORMAL", "_OUTLINE_USE_BAKENORMAL" };
    const int kButtonWidth = 300;
    const int kButtonHeight = 40;
    const int kButtonSpacing = 10;

    void OnGUI()
    {
        int y = 10;
        int x = 10;
    
        if (GUI.Button(new Rect(x, y, kButtonWidth, kButtonHeight), outlineKeywords[0]))
        {
            SwitchShaderKeyword(outlineKeywords, 0);
        }

        y += kButtonHeight + kButtonSpacing;
        if (GUI.Button(new Rect(x, y, kButtonWidth, kButtonHeight), outlineKeywords[1]))
        {
            SwitchShaderKeyword(outlineKeywords, 1);
        }

        y += kButtonHeight + kButtonSpacing;
        if (GUI.Button(new Rect(x, y, kButtonWidth, kButtonHeight), "Empty"))
        {
            SwitchShaderKeyword(outlineKeywords, -1);
        }
    }
}
