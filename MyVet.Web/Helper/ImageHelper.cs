using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MyVet.Web.Helper
{
    public class ImageHelper : IImageHelper
    {
        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";

            /*El metodo "Path.Combine" es un concatenador. 
              Es especial para unir rutas de diferentes sistema operativos (Windows, Linux. etc)
              Puedes usar concat pero este te puede generara problemas en otros sistemas
            */

            var path = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot\\images\\Pets", file);

            using(var stream=new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"~/images/Pets/{file}";
        }
    }
}
