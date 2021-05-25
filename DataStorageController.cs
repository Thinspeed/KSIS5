using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.StaticFiles;

namespace KSIS5
{
	[ApiController]
	[Route("[controller]")]
	public class DataStorageController : ControllerBase
	{
		DirectoryInfo root;

		public DataStorageController()
		{
			root = new DirectoryInfo("files");
			if (!root.Exists)
			{
				root.Create();
			}
		}

		[HttpGet]
		public JsonResult Get()
		{
			var result = new List<string>();
			foreach (var subdir in root.GetDirectories())
			{
				result.Add(subdir.Name);
			}

			foreach (var file in root.GetFiles())
			{
				result.Add(file.Name);
			}

			return new JsonResult(result);
		}

		[HttpGet("{path}")]
		public JsonResult Get(string path)
		{
			var dir = new DirectoryInfo(string.Format("{0}/{1}", root.Name, path));
			if (!dir.Exists)
			{
				return new JsonResult(new NotFoundResult().StatusCode + "Папка не найдена");
			}

			var result = new List<string>();
			foreach (var subdir in dir.GetDirectories())
			{
				result.Add(subdir.Name);
			}

			foreach (var file in dir.GetFiles())
			{
				result.Add(file.Name);
			}

			return new JsonResult(result);
		}

		[HttpGet("{path}.{format}")]
		public PhysicalFileResult Get(string path, string format)
		{
			var fullPath = string.Format("{0}/{1}.{2}", root.FullName, path, format);
			string contentType;
			new FileExtensionContentTypeProvider().TryGetContentType(fullPath, out contentType);
			var result = PhysicalFile(fullPath, contentType);
			return result;
		}

		[HttpPut("{path}")]
		public JsonResult Put([FromBody] IFormFile recivedFile, string path)
		{
			if (recivedFile == null)
			{
				return new JsonResult(new BadRequestResult().StatusCode + "файл не получен");
			}

			FileInfo file = new FileInfo(string.Format("{0}/{1}/{2}", root.Name, path, recivedFile.FileName));
			if (!file.Exists)
			{
				file.Create();
			}

			using (var streamToRead = recivedFile.OpenReadStream())
			{
				using (var streamToWrite = file.OpenWrite())
				{
					byte[] buff = new byte[1024];
					int allBytes = 0;
					int bytesReaded;
					while ((bytesReaded = streamToRead.Read(buff, allBytes, buff.Length)) != 0)
					{
						streamToWrite.Write(buff, allBytes, bytesReaded);
						allBytes += bytesReaded;
					}
				}
			}

			return new JsonResult(new OkResult().StatusCode);
		}

		[HttpDelete("{path}")]
		public JsonResult Delete(string path)
		{
			return path.Contains('.') ? DeleteFile(string.Format("{0}/{1}", root.Name, path)) : DeleteDir(string.Format("{0}/{1}", root.Name, path));
		}

		private JsonResult DeleteFile(string path)
		{
			var file = new FileInfo(path);
			if (!file.Exists)
			{
				return new JsonResult(new NotFoundResult().StatusCode + "Файл не найден");
			}

			file.Delete();
			return new JsonResult(new OkResult().StatusCode + "Файл удалён");
		}

		private JsonResult DeleteDir(string path)
		{
			var dir = new DirectoryInfo(path);
			if (!dir.Exists)
			{
				return new JsonResult(new NotFoundResult().StatusCode + "Папка не найдена");
			}

			dir.Delete();
			return new JsonResult(new OkResult().StatusCode + "Папка удалена");
		}


	}
}
