using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Paraphernalia.Extensions {
public static class UIExtensions {

	public static float TextWidth(this Text text) {
        float width = 0;
        CharacterInfo charInfo;
        foreach (char c in text.text) {
            text.font.GetCharacterInfo(c, out charInfo, text.cachedTextGenerator.fontSizeUsedForBestFit);
            width += charInfo.advance; 
        }
        return width;
	}
}
}