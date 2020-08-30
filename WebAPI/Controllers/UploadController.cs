using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAPI.Model;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private ApplicationSettings _appSettings;
        private ProductContext _context;

        public UploadController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<ApplicationSettings> appSettings, ProductContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _context = context;

        }
        [HttpPost,DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
           

            string folderName;
            string pathToSave;
            string fileName;
            string fullPath;
            string dbPath="";
            
                var file = Request.Form.Files;
                if (file.Count == 0)
                {
                 string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
               
                    dbPath = user.Image;
                    return Ok(new { dbPath});
                
                


            }
                else
                {
                var filex = Request.Form.Files[0];
                folderName = Path.Combine("Resources", "Upload");
               
                pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    if (filex.Length > 0)
                    {
                    var randomFilename = Path.GetRandomFileName();
                    fileName = Path.ChangeExtension(randomFilename, ".jpg");
                    //fileName = ContentDispositionHeaderValue.Parse(filex.ContentDisposition).FileName.Trim('"');
                    fullPath = Path.Combine(pathToSave, fileName);
                        dbPath = Path.Combine(folderName, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                        filex.CopyTo(stream);
                        }
                        return Ok(new { dbPath });

                    }
                    else
                    {
                        return BadRequest();
                    }
                }
               
            } 
           
        }
    }
