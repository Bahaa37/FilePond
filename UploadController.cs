using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;

namespace FilesHandler.Controllers
{
    public class UploadController : BaseController
    {
      private List<string> UserUploadedFiles
      {
        get => HttpContext.Session["UserUploadedFiles"] as List<string> ?? new List<string>();
        set
        {
          List<string> list = HttpContext.Session["UserUploadedFiles"] as List<string> ?? new List<string>();
          list.AddRange(value);
          HttpContext.Session["UserUploadedFiles"] = list;
        }
      }

      public ActionResult Index() => View();

[HttpPost]
public string Upload() => CheckAndSaveFile(Request.Files);

[HttpDelete]
public void Delete() => DeleteFile(Request.Form.GetValues("uniqueFileId")?[0]);

private string CheckAndSaveFile(HttpFileCollectionBase files)
{
    var validExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".webp", ".pdf", ".doc", ".docx", ".txt", ".rtf" };
    var validContentTypes = new HashSet<string> { "image/jpg", "image/jpeg", "image/png", "image/webp", "application/pdf", "application/msword", "text/plain" };
    string uploadedFileName = null;
    if (files.Count > 0)
    {
        for (int i = 0; i < files.Count; i++)
        {
            HttpPostedFileBase file = Request.Files[i];
            string extension = Path.GetExtension(file.FileName);
            if (file != null && file.ContentLength > 0 && validExtensions.Contains(extension) && validContentTypes.Contains(file.ContentType))
            {
                string randomFileName = Path.GetRandomFileName();
                file.SaveAs(Server.MapPath(Path.Combine("~/FileRepository/", randomFileName + extension)));
                uploadedFileName = randomFileName;
                List<string> fileNameWrapper = new List<string> { uploadedFileName };
                UserUploadedFiles = fileNameWrapper;
            }
            else
                throw new Exception("File is not valid");
        }
    }
    return uploadedFileName;
}

private void DeleteFile(string uniqueFileId)
{
    if (uniqueFileId != null)
    {
        string fileToDelete = Directory.EnumerateFiles(Server.MapPath("~/FileRepository/"), $"{uniqueFileId}.*")?.FirstOrDefault();
        if (fileToDelete != null && System.IO.File.Exists(fileToDelete))
        {
            System.IO.File.Delete(fileToDelete);
            UserUploadedFiles?.Remove(uniqueFileId);
        }
    }
    else
        throw new Exception("File not found or unable to delete.");
}
    }
}