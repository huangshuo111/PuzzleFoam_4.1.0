using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public class Xml
{
	public static string UTF8ByteArrayToString(byte[] characters)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		return uTF8Encoding.GetString(characters);
	}

	public static byte[] StringToUTF8ByteArray(string pXmlString)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		return uTF8Encoding.GetBytes(pXmlString);
	}

	public static string SerializeObject<Type>(object pObject)
	{
		string text = null;
		MemoryStream stream = new MemoryStream();
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(Type));
		XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, Encoding.UTF8);
		xmlSerializer.Serialize(xmlTextWriter, pObject);
		stream = (MemoryStream)xmlTextWriter.BaseStream;
		text = UTF8ByteArrayToString(stream.ToArray());
		xmlTextWriter.Close();
		stream.Dispose();
		return text;
	}

	public static object DeserializeObject<Type>(string pXmlizedString)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(Type));
		MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
		XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
		object result = xmlSerializer.Deserialize(memoryStream);
		xmlTextWriter.Close();
		memoryStream.Dispose();
		return result;
	}
}
