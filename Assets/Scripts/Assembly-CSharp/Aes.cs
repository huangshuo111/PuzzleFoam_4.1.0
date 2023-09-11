using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class Aes
{
	public enum eEncodeType
	{
		URLSafe = 0,
		Percent = 1
	}

	public static bool bUseAes = true;

	private static string password = string.Empty;

	private static byte[] nullIV = new byte[16];

	public static void Init()
	{
		password = MessageResource.Instance.getMessage(200000);
	}

	public static string EncryptString(string str, eEncodeType encodeType)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(password);
		byte[] iv = nullIV;
		return EncryptString(str, encodeType, bytes, iv);
	}

	public static string EncryptString(string str, eEncodeType encodeType, byte[] key, byte[] iv)
	{
		if (bUseAes)
		{
			AesManaged aesManaged = createAes();
			ICryptoTransform cryptoTransform = aesManaged.CreateEncryptor(key, iv);
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.Close();
			cryptoStream.Dispose();
			cryptoTransform.Dispose();
			memoryStream.Close();
			byte[] inArray = memoryStream.ToArray();
			memoryStream.Dispose();
			if (encodeType == eEncodeType.URLSafe)
			{
				return URLSafeEncode(Convert.ToBase64String(inArray));
			}
			return PercentEncode(Convert.ToBase64String(inArray));
		}
		return str;
	}

	public static string DecryptString(string str, eEncodeType decodeType)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(password);
		byte[] iv = nullIV;
		return DecryptString(str, decodeType, bytes, iv);
	}

	public static string DecryptString(string str, eEncodeType decodeType, byte[] key, byte[] iv)
	{
		if (bUseAes)
		{
			AesManaged aesManaged = createAes();
			ICryptoTransform cryptoTransform = aesManaged.CreateDecryptor(key, iv);
			byte[] array = null;
			array = ((decodeType != 0) ? Convert.FromBase64String(PercentDecode(str)) : Convert.FromBase64String(URLSafeDecode(str)));
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
			cryptoStream.Write(array, 0, array.Length);
			cryptoStream.Close();
			cryptoStream.Dispose();
			cryptoTransform.Dispose();
			memoryStream.Close();
			byte[] bytes = memoryStream.ToArray();
			memoryStream.Dispose();
			return Encoding.UTF8.GetString(bytes);
		}
		return str;
	}

	public static void EncryptToFile(byte[] bytes, string filepath)
	{
		byte[] bytes2 = Encoding.UTF8.GetBytes(password);
		byte[] iv = nullIV;
		EncryptToFile(bytes, filepath, bytes2, iv);
	}

	public static void EncryptToFile(byte[] bytes, string filepath, byte[] key, byte[] iv)
	{
		if (bUseAes)
		{
			filepath = EncryptPath(filepath);
			AesManaged aesManaged = createAes();
			ICryptoTransform cryptoTransform = aesManaged.CreateEncryptor(key, iv);
			FileStream fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
			CryptoStream cryptoStream = new CryptoStream(fileStream, cryptoTransform, CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.Close();
			cryptoStream.Dispose();
			cryptoTransform.Dispose();
			fileStream.Close();
			fileStream.Dispose();
		}
		else
		{
			File.WriteAllBytes(filepath, bytes);
		}
	}

	public static byte[] DecryptFromFile(string filepath)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(password);
		byte[] iv = nullIV;
		return DecryptFromFile(filepath, bytes, iv);
	}

	public static byte[] DecryptFromFile(string filepath, byte[] key, byte[] iv)
	{
		if (bUseAes)
		{
			filepath = EncryptPath(filepath);
			AesManaged aesManaged = createAes();
			FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
			ICryptoTransform cryptoTransform = aesManaged.CreateDecryptor(key, iv);
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
			byte[] array = File.ReadAllBytes(filepath);
			cryptoStream.Write(array, 0, array.Length);
			fileStream.Close();
			fileStream.Dispose();
			cryptoStream.Close();
			cryptoStream.Dispose();
			cryptoTransform.Dispose();
			memoryStream.Close();
			byte[] result = memoryStream.ToArray();
			memoryStream.Dispose();
			return result;
		}
		return File.ReadAllBytes(filepath);
	}

	public static bool Exists(string path)
	{
		if (bUseAes)
		{
			return File.Exists(EncryptPath(path));
		}
		return File.Exists(path);
	}

	public static void Delete(string path)
	{
		if (bUseAes)
		{
			File.Delete(EncryptPath(path));
		}
		else
		{
			File.Delete(path);
		}
	}

	public static string EncryptPath(string path)
	{
		string fileName = Path.GetFileName(path);
		string newValue = EncryptString(fileName, eEncodeType.Percent);
		return path.Replace(fileName, newValue);
	}

	private static AesManaged createAes()
	{
		AesManaged aesManaged = new AesManaged();
		aesManaged.KeySize = 256;
		aesManaged.BlockSize = 128;
		aesManaged.FeedbackSize = 128;
		aesManaged.Mode = CipherMode.CBC;
		aesManaged.Padding = PaddingMode.PKCS7;
		return aesManaged;
	}

	private static string URLSafeEncode(string str)
	{
		return str.Replace("+", ",").Replace("/", "-").Replace("=", "*");
	}

	private static string URLSafeDecode(string str)
	{
		return str.Replace(".", "+").Replace("-", "/").Replace("*", "=");
	}

	private static string PercentEncode(string str)
	{
		return str.Replace("+", "%2B").Replace("/", "%2F").Replace("=", "%3D");
	}

	private static string PercentDecode(string str)
	{
		return str.Replace("%2B", "+").Replace("%2F", "/").Replace("%3D", "=");
	}
}
