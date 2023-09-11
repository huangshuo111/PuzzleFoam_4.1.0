using System.Globalization;
using UnityEngine;

public class NGUIUtility
{
	public static void disable(UIButton[] buttons, bool bOnlySelf)
	{
		foreach (UIButton button in buttons)
		{
			disable(button, bOnlySelf);
		}
	}

	public static void disable(UIButton button, bool bOnlySelf)
	{
		if (button != null)
		{
			button.setEnable(false, bOnlySelf);
		}
	}

	public static void enable(UIButton[] buttons, bool bOnlySelf)
	{
		foreach (UIButton button in buttons)
		{
			enable(button, bOnlySelf);
		}
	}

	public static void enable(UIButton button, bool bOnlySelf)
	{
		if (button != null)
		{
			button.setEnable(true, bOnlySelf);
		}
	}

	public static void setupButton(GameObject target, GameObject receiver)
	{
		setupButton(target, receiver, false);
	}

	public static void setupButton(GameObject target, GameObject receiver, bool includeInactive)
	{
		UIButtonMessage[] componentsInChildren = target.GetComponentsInChildren<UIButtonMessage>(includeInactive);
		UIButtonMessage[] array = componentsInChildren;
		foreach (UIButtonMessage uIButtonMessage in array)
		{
			uIButtonMessage.target = receiver;
			uIButtonMessage.functionName = "OnButton";
		}
	}

	public static int GetTextWidthMax(UILabel label)
	{
		BMFont bMFont = null;
		UIFont font = null;
		Font dynamicFont = null;
		if (label.font.UseDynamicFont)
		{
			GetFontInfo(label, ref font, ref dynamicFont);
		}
		else
		{
			font = label.font;
			bMFont = font.bmFont;
		}
		string processedText = label.processedText;
		int num = 0;
		int num2 = 0;
		int horizontalSpacing = font.horizontalSpacing;
		string[] array = processedText.Split('\n');
		string[] array2 = array;
		foreach (string text in array2)
		{
			foreach (char c in text)
			{
				if (label.font.UseDynamicFont)
				{
					CharacterInfo info;
					if (dynamicFont.GetCharacterInfo(c, out info, font.dynamicFontSize, font.dynamicFontStyle))
					{
						num += (int)((float)horizontalSpacing + info.width);
					}
				}
				else
				{
					BMGlyph glyph = bMFont.GetGlyph(c);
					num = ((glyph == null) ? (num + (horizontalSpacing + 45)) : (num + (horizontalSpacing + glyph.advance)));
				}
			}
			num2 = Mathf.Max(num2, num);
			num = 0;
		}
		return num2;
	}

	public static string LeftWidth(UILabel label, int width)
	{
		BMFont bMFont = null;
		UIFont font = null;
		Font dynamicFont = null;
		if (label.font.UseDynamicFont)
		{
			GetFontInfo(label, ref font, ref dynamicFont);
		}
		else
		{
			font = label.font;
			bMFont = font.bmFont;
		}
		string processedText = label.processedText;
		int horizontalSpacing = font.horizontalSpacing;
		int num = 0;
		string text = string.Empty;
		string[] array = processedText.Split('\n');
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			num = 0;
			if (text.Length > 0)
			{
				text += "\n";
			}
			for (int j = 0; j < text2.Length; j++)
			{
				char c = text2[j];
				UnicodeCategory unicodeCategory = char.GetUnicodeCategory(c);
				if (unicodeCategory == UnicodeCategory.Surrogate)
				{
					c = ' ';
				}
				if (label.font.UseDynamicFont)
				{
					CharacterInfo info;
					if (dynamicFont.GetCharacterInfo(c, out info, font.dynamicFontSize, font.dynamicFontStyle))
					{
						num += (int)((float)horizontalSpacing + info.width);
					}
				}
				else
				{
					BMGlyph glyph = bMFont.GetGlyph(c);
					num = ((glyph == null) ? (num + (horizontalSpacing + 45)) : (num + (horizontalSpacing + glyph.advance)));
				}
				if (num > width)
				{
					text += text2.Substring(0, j);
					break;
				}
			}
			text = Utility.StripInvalidUnicodeCharacters(text);
		}
		return text;
	}

	private static void GetFontInfo(UILabel label, ref UIFont font, ref Font dynamicFont)
	{
		if (label == null)
		{
			return;
		}
		font = label.font;
		if (!(font == null))
		{
			dynamicFont = font.dynamicFont;
			if (!(dynamicFont == null))
			{
			}
		}
	}
}
