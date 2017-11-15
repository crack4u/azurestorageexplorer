﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using StorageLibrary;

namespace AngularWebStorageExplorer.Controllers
{
    [Produces("application/json")]
    [Route("api/Blobs")]
    public class BlobsController : Controller
    {
		[HttpGet("[action]")]
		public async Task<IEnumerable<string>> GetBlobs(string container)
		{
			if (string.IsNullOrEmpty(container))
				return new List<string>();

			List<IListBlobItem> blobs = await Container.ListBlobsAsync(Settings.Instance.Account, Settings.Instance.Key, container);

			return blobs.Select(b => b.Uri.ToString());
		}

		[HttpGet("[action]")]
		public async Task<FileResult> GetBlob(string blobUri)
		{
			if (string.IsNullOrEmpty(blobUri))
				return null;

			string blobPath = await Container.GetBlob(Settings.Instance.Account, Settings.Instance.Key, blobUri);

			FileInfo info = new FileInfo(blobPath);

			byte[] fileBytes = System.IO.File.ReadAllBytes(info.FullName);
			//return File(fileBytes, "application/x-msdownload", info.Name);
			return File(fileBytes, "application/octet-stream", info.Name);

			//Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
			//Response.AddHeader("Content-Length", info.Length.ToString());
			//Response.ContentType = "application/octet-stream";
			//Response.WriteFile(path);
			//Response.Flush();

			//return Ok();
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> DeleteBlob( string blobUri)
		{
			if (string.IsNullOrEmpty(blobUri))
				return BadRequest();

			await Container.DeleteBlobAsync(Settings.Instance.Account, Settings.Instance.Key, blobUri);

			return Ok();
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> UploadBlob(string container, List<IFormFile> files)
		{
			foreach (IFormFile file in files)
				await Container.CreateBlobAsync(Settings.Instance.Account, Settings.Instance.Key, container, file.FileName, file.OpenReadStream() );

			return Ok();
		}
	}


}