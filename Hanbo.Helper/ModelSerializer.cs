using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Hanbo.Helper
{
	public class ModelSerializer
	{
		private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

		/// <summary>
		/// 序列化 to File
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="filepath"></param>
		public static void DoSerialize(object obj, string filepath)
		{
			using (FileStream fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
			{
				// 建立 BinaryFormatter 物件
				BinaryFormatter binaryFormatter = new BinaryFormatter();

				// 將物件進行二進位序列化，並且儲存檔案				
				binaryFormatter.Serialize(fileStream, obj);
			}
		}

		/// <summary>
		/// 序列化取得二進位資料
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static byte[] DoSerialize(object obj)
		{
			byte[] data = null;
			using (MemoryStream ms = new MemoryStream())
			{
				// 建立 BinaryFormatter 物件
				BinaryFormatter binaryFormatter = new BinaryFormatter();

				// 將物件進行二進位序列化
				binaryFormatter.Serialize(ms, obj);
				data = ms.GetBuffer();
			}
			return data;
		}

		/// <summary>
		/// 反序列化 From File
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static object DeSerialize(string fileName)
		{
			object deObject = null;
			try
			{
				using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
				{
					// 建立 BinaryFormatter 物件
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					// 將檔案內容還原序列化成 Object 物件，並且進一步轉型成正確的型別 ClsSerializable
					deObject = binaryFormatter.Deserialize(fileStream);
				}
			}
			catch (SerializationException ex)
			{
				logger.Error("DeSerialize Fail: " + ex.Message);
			}
			return deObject;
		}

		/// <summary>
		/// 反序列化 From Memory
		/// </summary>
		/// <param name="binaryData"></param>
		/// <returns></returns>
		public static object DeSerialize(byte[] binaryData)
		{
			object deObject = null;
			try
			{
				using (MemoryStream ms = new MemoryStream(binaryData))
				{
					// 建立 BinaryFormatter 物件
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					// 將檔案內容還原序列化成 Object 物件
					deObject = binaryFormatter.Deserialize(ms);
				}
			}
			catch (SerializationException ex)
			{
				logger.Error("DeSerialize Fail: " + ex.Message);
			}
			return deObject;
		}
	}
}
