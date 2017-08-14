using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Paraphernalia.Extensions {
public static class UIExtensions {

	public static float TextWidth(this Text text) {
        float width = 0;
        int fontSize = text.fontSize;
        if (text.resizeTextForBestFit) fontSize = text.cachedTextGeneratorForLayout.fontSizeUsedForBestFit;
        text.font.RequestCharactersInTexture(text.text, fontSize);
        CharacterInfo charInfo;
        foreach (char c in text.text) {
            text.font.GetCharacterInfo(c, out charInfo, fontSize);
            width += charInfo.advance; 
        }
        return width;
	}
}
}