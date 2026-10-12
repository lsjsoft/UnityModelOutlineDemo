using UnityEngine;

public class MenuController : MonoBehaviour
{
    private void SwitchShaderKeyword(string[] keywords, int index)
    {
        for (int i = 0; i < keywords.Length; i++)
        {
            if (string.IsNullOrEmpty(keywords[i]))
            {
                continue;
            }
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

    private static readonly string[] outlineKeywords = { "_OUTLINE_NORMAL", "_OUTLINE_BAKE2", "_OUTLINE_BAKE3", "" };
    private static readonly string[] outlineNames    = { "原始法线", "烘焙法线2", "烘焙法线3", "关闭" };
    const int kButtonWidth = 300;
    const int kButtonHeight = 40;
    const int kButtonSpacing = 10;

    void OnGUI()
    {
        int y = 10;
        int x = 10;
    
        for (int i = 0; i < outlineKeywords.Length; i++)
        {
            if (GUI.Button(new Rect(x, y, kButtonWidth, kButtonHeight), outlineNames[i]))
            {
                SwitchShaderKeyword(outlineKeywords, i);
            }
            y += kButtonHeight + kButtonSpacing;
        }
    }
}
