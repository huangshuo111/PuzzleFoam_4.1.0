using System;
using System.Text;
using UnityEngine;
using tk2dRuntime;

[AddComponentMenu("2D Toolkit/Text/tk2dTextMesh")]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class tk2dTextMesh : MonoBehaviour, ISpriteCollectionForceBuild
{
	[Flags]
	private enum UpdateFlags
	{
		UpdateNone = 0,
		UpdateText = 1,
		UpdateColors = 2,
		UpdateBuffers = 4
	}

	[SerializeField]
	private tk2dFontData _font;

	private tk2dFontData _fontInst;

	[SerializeField]
	private string _text = string.Empty;

	private string _formattedText = string.Empty;

	[SerializeField]
	private Color _color = Color.white;

	[SerializeField]
	private Color _color2 = Color.white;

	[SerializeField]
	private bool _useGradient;

	[SerializeField]
	private int _textureGradient;

	[SerializeField]
	private TextAnchor _anchor = TextAnchor.LowerLeft;

	[SerializeField]
	private Vector3 _scale = new Vector3(1f, 1f, 1f);

	[SerializeField]
	private bool _kerning;

	[SerializeField]
	private int _maxChars = 16;

	[SerializeField]
	private bool _inlineStyling;

	[SerializeField]
	private bool _formatting;

	[SerializeField]
	private int _wordWrapWidth;

	public bool pixelPerfect;

	public float spacing;

	public float lineSpacing;

	private Vector3[] vertices;

	private Vector2[] uvs;

	private Vector2[] uv2;

	private Color[] colors;

	private UpdateFlags updateFlags = UpdateFlags.UpdateBuffers;

	private Mesh mesh;

	private MeshFilter meshFilter;

	private static readonly Color[] channelSelectColors = new Color[4]
	{
		new Color(0f, 0f, 1f, 0f),
		new Color(0f, 1f, 0f, 0f),
		new Color(1f, 0f, 0f, 0f),
		new Color(0f, 0f, 0f, 1f)
	};

	public tk2dFontData font
	{
		get
		{
			return _font;
		}
		set
		{
			_font = value;
			_fontInst = _font.inst;
			updateFlags |= UpdateFlags.UpdateText;
			UpdateMaterial();
		}
	}

	public bool formatting
	{
		get
		{
			return _formatting;
		}
		set
		{
			if (_formatting != value)
			{
				_formatting = value;
				updateFlags |= UpdateFlags.UpdateText;
			}
		}
	}

	public int wordWrapWidth
	{
		get
		{
			return _wordWrapWidth;
		}
		set
		{
			if (_wordWrapWidth != value)
			{
				_wordWrapWidth = value;
				updateFlags |= UpdateFlags.UpdateText;
			}
		}
	}

	public string text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public Color color
	{
		get
		{
			return _color;
		}
		set
		{
			_color = value;
			updateFlags |= UpdateFlags.UpdateColors;
		}
	}

	public Color color2
	{
		get
		{
			return _color2;
		}
		set
		{
			_color2 = value;
			updateFlags |= UpdateFlags.UpdateColors;
		}
	}

	public bool useGradient
	{
		get
		{
			return _useGradient;
		}
		set
		{
			_useGradient = value;
			updateFlags |= UpdateFlags.UpdateColors;
		}
	}

	public TextAnchor anchor
	{
		get
		{
			return _anchor;
		}
		set
		{
			_anchor = value;
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public Vector3 scale
	{
		get
		{
			return _scale;
		}
		set
		{
			_scale = value;
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public bool kerning
	{
		get
		{
			return _kerning;
		}
		set
		{
			_kerning = value;
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public int maxChars
	{
		get
		{
			return _maxChars;
		}
		set
		{
			_maxChars = value;
			updateFlags |= UpdateFlags.UpdateBuffers;
		}
	}

	public int textureGradient
	{
		get
		{
			return _textureGradient;
		}
		set
		{
			_textureGradient = value % font.gradientCount;
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public bool inlineStyling
	{
		get
		{
			return _inlineStyling;
		}
		set
		{
			_inlineStyling = value;
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public float Spacing
	{
		get
		{
			return spacing;
		}
		set
		{
			if (spacing != value)
			{
				spacing = value;
				updateFlags |= UpdateFlags.UpdateText;
			}
		}
	}

	public float LineSpacing
	{
		get
		{
			return lineSpacing;
		}
		set
		{
			if (lineSpacing != value)
			{
				lineSpacing = value;
				updateFlags |= UpdateFlags.UpdateText;
			}
		}
	}

	private bool useInlineStyling
	{
		get
		{
			return inlineStyling && _fontInst.textureGradients;
		}
	}

	public string FormatText(string unformattedString)
	{
		string _targetString = string.Empty;
		FormatText(ref _targetString, unformattedString);
		return _targetString;
	}

	private void FormatText()
	{
		FormatText(ref _formattedText, _text);
	}

	private void FormatText(ref string _targetString, string _source)
	{
		if (!formatting || wordWrapWidth == 0 || _fontInst.texelSize == Vector2.zero)
		{
			_targetString = _source;
			return;
		}
		float num = _fontInst.texelSize.x * (float)wordWrapWidth;
		StringBuilder stringBuilder = new StringBuilder(_source.Length);
		float num2 = 0f;
		float num3 = 0f;
		int num4 = -1;
		int num5 = -1;
		for (int i = 0; i < _source.Length; i++)
		{
			char c = _source[i];
			tk2dFontChar tk2dFontChar2;
			if (_fontInst.useDictionary)
			{
				if (!_fontInst.charDict.ContainsKey(c))
				{
					c = '\0';
				}
				tk2dFontChar2 = _fontInst.charDict[c];
			}
			else
			{
				if (c >= _fontInst.chars.Length)
				{
					c = '\0';
				}
				tk2dFontChar2 = _fontInst.chars[(uint)c];
			}
			switch (c)
			{
			case '\n':
				num2 = 0f;
				num3 = 0f;
				num4 = stringBuilder.Length;
				num5 = i;
				break;
			case ' ':
			case '!':
			case ',':
			case '.':
			case ':':
			case ';':
				if (num2 + tk2dFontChar2.p1.x * _scale.x > num)
				{
					stringBuilder.Append('\n');
					num2 = tk2dFontChar2.advance * _scale.x;
				}
				else
				{
					num2 += tk2dFontChar2.advance * _scale.x;
				}
				num3 = num2;
				num4 = stringBuilder.Length;
				num5 = i;
				break;
			default:
				if (num2 + tk2dFontChar2.p1.x * _scale.x > num)
				{
					if (num3 > 0f)
					{
						num3 = 0f;
						num2 = 0f;
						stringBuilder.Remove(num4 + 1, stringBuilder.Length - num4 - 1);
						stringBuilder.Append('\n');
						i = num5;
						continue;
					}
					stringBuilder.Append('\n');
					num2 = tk2dFontChar2.advance * _scale.x;
				}
				else
				{
					num2 += tk2dFontChar2.advance * _scale.x;
				}
				break;
			}
			stringBuilder.Append(c);
		}
		_targetString = stringBuilder.ToString();
	}

	private void InitInstance()
	{
		if (_fontInst == null && _font != null)
		{
			_fontInst = _font.inst;
		}
	}

	private void Awake()
	{
		if (_font != null)
		{
			_fontInst = _font.inst;
		}
		if (pixelPerfect)
		{
			MakePixelPerfect();
		}
		updateFlags = UpdateFlags.UpdateBuffers;
		if (_font != null)
		{
			Init();
			UpdateMaterial();
		}
	}

	protected void OnDestroy()
	{
		if (meshFilter == null)
		{
			meshFilter = GetComponent<MeshFilter>();
		}
		if (meshFilter != null)
		{
			mesh = meshFilter.sharedMesh;
		}
		if ((bool)mesh)
		{
			UnityEngine.Object.DestroyImmediate(mesh, true);
			meshFilter.mesh = null;
		}
	}

	public int NumDrawnCharacters()
	{
		InitInstance();
		if ((updateFlags & (UpdateFlags.UpdateText | UpdateFlags.UpdateBuffers)) != 0)
		{
			FormatText();
		}
		bool flag = useInlineStyling;
		int num = 0;
		for (int i = 0; i < _formattedText.Length && num < _maxChars; i++)
		{
			int num2 = _formattedText[i];
			if (_fontInst.useDictionary)
			{
				if (!_fontInst.charDict.ContainsKey(num2))
				{
					num2 = 0;
				}
			}
			else if (num2 >= _fontInst.chars.Length)
			{
				num2 = 0;
			}
			if (num2 == 10)
			{
				continue;
			}
			if (flag && num2 == 94 && i + 1 < _formattedText.Length)
			{
				i++;
				if (_formattedText[i] != '^')
				{
					continue;
				}
			}
			num++;
		}
		return num;
	}

	public int NumTotalCharacters()
	{
		InitInstance();
		bool flag = useInlineStyling;
		int num = 0;
		for (int i = 0; i < _formattedText.Length; i++)
		{
			int num2 = _formattedText[i];
			if (_fontInst.useDictionary)
			{
				if (!_fontInst.charDict.ContainsKey(num2))
				{
					num2 = 0;
				}
			}
			else if (num2 >= _fontInst.chars.Length)
			{
				num2 = 0;
			}
			if (num2 == 10)
			{
				continue;
			}
			if (flag && num2 == 94 && i + 1 < _formattedText.Length)
			{
				i++;
				if (_formattedText[i] != '^')
				{
					continue;
				}
			}
			num++;
		}
		return num;
	}

	private void PostAlignTextData(int targetStart, int targetEnd, float offsetX)
	{
		for (int i = targetStart * 4; i < targetEnd * 4; i++)
		{
			Vector3 vector = vertices[i];
			vector.x += offsetX;
			vertices[i] = vector;
		}
	}

	private int FillTextData()
	{
		Vector2 vector = new Vector2((float)_textureGradient / (float)font.gradientCount, 0f);
		float yAnchorForHeight = GetYAnchorForHeight(GetMeshDimensionsForString(_formattedText).y);
		bool flag = useInlineStyling;
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < _formattedText.Length && num3 < _maxChars; i++)
		{
			int num5 = _formattedText[i];
			tk2dFontChar tk2dFontChar2;
			if (_fontInst.useDictionary)
			{
				if (!_fontInst.charDict.ContainsKey(num5))
				{
					num5 = 0;
				}
				tk2dFontChar2 = _fontInst.charDict[num5];
			}
			else
			{
				if (num5 >= _fontInst.chars.Length)
				{
					num5 = 0;
				}
				tk2dFontChar2 = _fontInst.chars[num5];
			}
			if (num5 == 10)
			{
				float lineWidth = num;
				int targetEnd = num3;
				if (num4 != num3)
				{
					float xAnchorForWidth = GetXAnchorForWidth(lineWidth);
					PostAlignTextData(num4, targetEnd, xAnchorForWidth);
				}
				num4 = num3;
				num = 0f;
				num2 -= (_fontInst.lineHeight + lineSpacing) * _scale.y;
				continue;
			}
			if (flag && num5 == 94 && i + 1 < _formattedText.Length)
			{
				i++;
				if (_formattedText[i] != '^')
				{
					int num6 = _formattedText[i] - 48;
					vector = new Vector2((float)num6 / (float)font.gradientCount, 0f);
					continue;
				}
			}
			vertices[num3 * 4] = new Vector3(num + tk2dFontChar2.p0.x * _scale.x, yAnchorForHeight + num2 + tk2dFontChar2.p0.y * _scale.y, 0f);
			vertices[num3 * 4 + 1] = new Vector3(num + tk2dFontChar2.p1.x * _scale.x, yAnchorForHeight + num2 + tk2dFontChar2.p0.y * _scale.y, 0f);
			vertices[num3 * 4 + 2] = new Vector3(num + tk2dFontChar2.p0.x * _scale.x, yAnchorForHeight + num2 + tk2dFontChar2.p1.y * _scale.y, 0f);
			vertices[num3 * 4 + 3] = new Vector3(num + tk2dFontChar2.p1.x * _scale.x, yAnchorForHeight + num2 + tk2dFontChar2.p1.y * _scale.y, 0f);
			if (tk2dFontChar2.flipped)
			{
				uvs[num3 * 4] = new Vector2(tk2dFontChar2.uv1.x, tk2dFontChar2.uv1.y);
				uvs[num3 * 4 + 1] = new Vector2(tk2dFontChar2.uv1.x, tk2dFontChar2.uv0.y);
				uvs[num3 * 4 + 2] = new Vector2(tk2dFontChar2.uv0.x, tk2dFontChar2.uv1.y);
				uvs[num3 * 4 + 3] = new Vector2(tk2dFontChar2.uv0.x, tk2dFontChar2.uv0.y);
			}
			else
			{
				uvs[num3 * 4] = new Vector2(tk2dFontChar2.uv0.x, tk2dFontChar2.uv0.y);
				uvs[num3 * 4 + 1] = new Vector2(tk2dFontChar2.uv1.x, tk2dFontChar2.uv0.y);
				uvs[num3 * 4 + 2] = new Vector2(tk2dFontChar2.uv0.x, tk2dFontChar2.uv1.y);
				uvs[num3 * 4 + 3] = new Vector2(tk2dFontChar2.uv1.x, tk2dFontChar2.uv1.y);
			}
			if (_fontInst.textureGradients)
			{
				uv2[num3 * 4] = vector + tk2dFontChar2.gradientUv[0];
				uv2[num3 * 4 + 1] = vector + tk2dFontChar2.gradientUv[1];
				uv2[num3 * 4 + 2] = vector + tk2dFontChar2.gradientUv[2];
				uv2[num3 * 4 + 3] = vector + tk2dFontChar2.gradientUv[3];
			}
			if (_fontInst.isPacked)
			{
				Color color = channelSelectColors[tk2dFontChar2.channel];
				colors[num3 * 4] = color;
				colors[num3 * 4 + 1] = color;
				colors[num3 * 4 + 2] = color;
				colors[num3 * 4 + 3] = color;
			}
			num += (tk2dFontChar2.advance + spacing) * _scale.x;
			if (_kerning && i < _formattedText.Length - 1)
			{
				tk2dFontKerning[] array = _fontInst.kerning;
				foreach (tk2dFontKerning tk2dFontKerning2 in array)
				{
					if (tk2dFontKerning2.c0 == _formattedText[i] && tk2dFontKerning2.c1 == _formattedText[i + 1])
					{
						num += tk2dFontKerning2.amount * _scale.x;
						break;
					}
				}
			}
			num3++;
		}
		if (num4 != num3)
		{
			float lineWidth2 = num;
			int targetEnd2 = num3;
			float xAnchorForWidth2 = GetXAnchorForWidth(lineWidth2);
			PostAlignTextData(num4, targetEnd2, xAnchorForWidth2);
		}
		return num3;
	}

	public void Init(bool force)
	{
		if (force)
		{
			updateFlags |= UpdateFlags.UpdateBuffers;
		}
		Init();
	}

	public void Init()
	{
		if (!_fontInst || ((updateFlags & UpdateFlags.UpdateBuffers) == 0 && !(mesh == null)))
		{
			return;
		}
		_fontInst.InitDictionary();
		FormatText();
		Color color = _color;
		Color color2 = ((!_useGradient) ? _color : _color2);
		vertices = new Vector3[_maxChars * 4];
		uvs = new Vector2[_maxChars * 4];
		colors = new Color[_maxChars * 4];
		if (_fontInst.textureGradients)
		{
			uv2 = new Vector2[_maxChars * 4];
		}
		int[] array = new int[_maxChars * 6];
		int num = FillTextData();
		for (int i = 0; i < num; i++)
		{
			if (!_fontInst.isPacked)
			{
				colors[i * 4] = (colors[i * 4 + 1] = color);
				colors[i * 4 + 2] = (colors[i * 4 + 3] = color2);
			}
			array[i * 6] = i * 4;
			array[i * 6 + 1] = i * 4 + 1;
			array[i * 6 + 2] = i * 4 + 3;
			array[i * 6 + 3] = i * 4 + 2;
			array[i * 6 + 4] = i * 4;
			array[i * 6 + 5] = i * 4 + 3;
		}
		for (int j = num; j < _maxChars; j++)
		{
			vertices[j * 4] = (vertices[j * 4 + 1] = (vertices[j * 4 + 2] = (vertices[j * 4 + 3] = Vector3.zero)));
			uvs[j * 4] = (uvs[j * 4 + 1] = (uvs[j * 4 + 2] = (uvs[j * 4 + 3] = Vector2.zero)));
			if (_fontInst.textureGradients)
			{
				uv2[j * 4] = (uv2[j * 4 + 1] = (uv2[j * 4 + 2] = (uv2[j * 4 + 3] = Vector2.zero)));
			}
			if (!_fontInst.isPacked)
			{
				colors[j * 4] = (colors[j * 4 + 1] = color);
				colors[j * 4 + 2] = (colors[j * 4 + 3] = color2);
			}
			else
			{
				colors[j * 4] = (colors[j * 4 + 1] = (colors[j * 4 + 2] = (colors[j * 4 + 3] = Color.clear)));
			}
			array[j * 6] = j * 4;
			array[j * 6 + 1] = j * 4 + 1;
			array[j * 6 + 2] = j * 4 + 3;
			array[j * 6 + 3] = j * 4 + 2;
			array[j * 6 + 4] = j * 4;
			array[j * 6 + 5] = j * 4 + 3;
		}
		if (mesh == null)
		{
			if (meshFilter == null)
			{
				meshFilter = GetComponent<MeshFilter>();
			}
			mesh = new Mesh();
			mesh.hideFlags = HideFlags.DontSave;
			meshFilter.mesh = mesh;
		}
		else
		{
			mesh.Clear();
		}
		mesh.vertices = vertices;
		mesh.uv = uvs;
		if (font.textureGradients)
		{
			mesh.uv2 = uv2;
		}
		mesh.triangles = array;
		mesh.colors = colors;
		mesh.RecalculateBounds();
		updateFlags = UpdateFlags.UpdateNone;
	}

	public void Commit()
	{
		InitInstance();
		_fontInst.InitDictionary();
		if ((updateFlags & UpdateFlags.UpdateBuffers) != 0 || mesh == null)
		{
			Init();
		}
		else
		{
			if ((updateFlags & UpdateFlags.UpdateText) != 0)
			{
				FormatText();
				int num = FillTextData();
				for (int i = num; i < _maxChars; i++)
				{
					vertices[i * 4] = (vertices[i * 4 + 1] = (vertices[i * 4 + 2] = (vertices[i * 4 + 3] = Vector3.zero)));
				}
				mesh.vertices = vertices;
				mesh.uv = uvs;
				if (_fontInst.textureGradients)
				{
					mesh.uv2 = uv2;
				}
				mesh.RecalculateBounds();
			}
			if (!font.isPacked && (updateFlags & UpdateFlags.UpdateColors) != 0)
			{
				Color color = _color;
				Color color2 = ((!_useGradient) ? _color : _color2);
				for (int j = 0; j < colors.Length; j += 4)
				{
					colors[j] = (colors[j + 1] = color);
					colors[j + 2] = (colors[j + 3] = color2);
				}
				mesh.colors = colors;
			}
		}
		updateFlags = UpdateFlags.UpdateNone;
	}

	public Vector2 GetMeshDimensionsForString(string str)
	{
		bool flag = useInlineStyling;
		float b = 0f;
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		for (int i = 0; i < str.Length && num3 < _maxChars; i++)
		{
			int num4 = str[i];
			if (num4 == 10)
			{
				b = Mathf.Max(num, b);
				num = 0f;
				num2 -= (_fontInst.lineHeight + lineSpacing) * _scale.y;
				continue;
			}
			if (flag && num4 == 94 && i + 1 < str.Length)
			{
				i++;
				if (str[i] != '^')
				{
					continue;
				}
			}
			tk2dFontChar tk2dFontChar2;
			if (_fontInst.useDictionary)
			{
				if (!_fontInst.charDict.ContainsKey(num4))
				{
					num4 = 0;
				}
				tk2dFontChar2 = _fontInst.charDict[num4];
			}
			else
			{
				if (num4 >= _fontInst.chars.Length)
				{
					num4 = 0;
				}
				tk2dFontChar2 = _fontInst.chars[num4];
			}
			num += (tk2dFontChar2.advance + spacing) * _scale.x;
			if (_kerning && i < str.Length - 1)
			{
				tk2dFontKerning[] array = _fontInst.kerning;
				foreach (tk2dFontKerning tk2dFontKerning2 in array)
				{
					if (tk2dFontKerning2.c0 == str[i] && tk2dFontKerning2.c1 == str[i + 1])
					{
						num += tk2dFontKerning2.amount * _scale.x;
						break;
					}
				}
			}
			num3++;
		}
		b = Mathf.Max(num, b);
		num2 -= (_fontInst.lineHeight + lineSpacing) * _scale.y;
		return new Vector2(b, num2);
	}

	private float GetYAnchorForHeight(float textHeight)
	{
		int num = (int)_anchor / 3;
		float num2 = (_fontInst.lineHeight + lineSpacing) * _scale.y;
		switch (num)
		{
		case 0:
			return 0f - num2;
		case 1:
		{
			float num3 = (0f - textHeight) / 2f - num2;
			if (_fontInst.version >= 2)
			{
				float num4 = _fontInst.texelSize.y * _scale.y;
				return Mathf.Floor(num3 / num4) * num4;
			}
			return num3;
		}
		case 2:
			return 0f - textHeight - num2;
		default:
			return 0f - num2;
		}
	}

	private float GetXAnchorForWidth(float lineWidth)
	{
		switch ((int)_anchor % 3)
		{
		case 0:
			return 0f;
		case 1:
		{
			float num = (0f - lineWidth) / 2f;
			if (_fontInst.version >= 2)
			{
				float num2 = _fontInst.texelSize.x * _scale.x;
				return Mathf.Floor(num / num2) * num2;
			}
			return num;
		}
		case 2:
			return 0f - lineWidth;
		default:
			return 0f;
		}
	}

	public void MakePixelPerfect()
	{
		float num = 1f;
		tk2dPixelPerfectHelper inst = tk2dPixelPerfectHelper.inst;
		if ((bool)inst)
		{
			num = ((!inst.CameraIsOrtho) ? (inst.scaleK + inst.scaleD * base.transform.position.z) : inst.scaleK);
		}
		else if (tk2dCamera.inst != null)
		{
			if (_fontInst.version < 1)
			{
				Debug.LogError("Need to rebuild font.");
			}
			num = _fontInst.invOrthoSize * _fontInst.halfTargetHeight;
		}
		else if ((bool)Camera.main)
		{
			if (Camera.main.orthographic)
			{
				num = Camera.main.orthographicSize;
			}
			else
			{
				float zdist = base.transform.position.z - Camera.main.transform.position.z;
				num = tk2dPixelPerfectHelper.CalculateScaleForPerspectiveCamera(Camera.main.fov, zdist);
			}
		}
		scale = new Vector3(Mathf.Sign(scale.x) * num, Mathf.Sign(scale.y) * num, Mathf.Sign(scale.z) * num);
	}

	public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
	{
		if (_font != null && _font.spriteCollection != null)
		{
			return _font.spriteCollection == spriteCollection;
		}
		return true;
	}

	private void UpdateMaterial()
	{
		if (base.GetComponent<Renderer>().sharedMaterial != _fontInst.materialInst)
		{
			base.GetComponent<Renderer>().material = _fontInst.materialInst;
		}
	}

	public void ForceBuild()
	{
		if (_font != null)
		{
			_fontInst = _font.inst;
			UpdateMaterial();
		}
		Init(true);
	}
}
