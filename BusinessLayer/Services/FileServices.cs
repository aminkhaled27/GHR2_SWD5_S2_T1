using BusinessLayer.Services.IServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class FileServices : IFileServices
    {
        public string UploadFile(IFormFile file, string destinationFolder)
        {
            var uniqueFileName = string.Empty;

            if (file != null && file.Length > 0)
            {
               

                var uploadsFolder = Path.Combine(@"./wwwroot/Images/", destinationFolder);
                uniqueFileName = Guid.NewGuid().ToString().Substring(0,10) + "_" + file.FileName;  // ==> map to  uniqueidentifier  in sql server
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                
                using (var fileStream = new FileStream(filePath, FileMode.Create)) // Resources written inside using()  must implement IDisposable interface
                {
                    file.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

    }
}
