using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend_aspnet_crud.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using backend_aspnet_crud.Entities;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;

namespace backend_aspnet_crud.Controller
{
    // public class FileUPloadAPI {
    //     public IFormFile file { get; set; }
    // }

    [ApiController]
    [Route("v1/users")]
    [Authorize]
    public class ClientController: ControllerBase {
        [HttpGet]
        [Route("oauth")]
        public async Task<ActionResult<User>> findAll(
            [FromServices] IUserRepository userContext,
            [FromServices] IFileRepository fileContext
        ){
            var username = this.User.Identity.Name;
            var user = await userContext.findByUsername(username);
            
            if(user == null){
                return new ObjectResult(new { errors = new List<string> {"user not found"} });
            }

            var image = fileContext.findFileByUserId(user.id);
            if(image != null) {
                user.Image = image;
            }

            return user;
        }

        [HttpPost]
        [Route("uploads")]
        public async Task<ActionResult> UploadFileAsync(
            [FromServices] IFileRepository fileContext,
            [FromServices] IUserRepository userContext,
            [FromServices] IWebHostEnvironment environment
        ){
            var username = this.User.Identity.Name;
            var user = await userContext.findByUsername(username);
            if(user == null){
                return BadRequest("user not found");
            }
            
            var files = HttpContext.Request.Form.Files;
            if(files.Count != 1) {
                return BadRequest("problem 01");
            }

            var ImagePath = environment.WebRootPath+"\\Upload\\";
            var Extension = Path.GetExtension(files[0].FileName);
            var RelativeImagePath = ImagePath + user.id + Extension;

            try{
                using (FileStream fileStream = System.IO.File.Create(RelativeImagePath)) {
                    files[0].CopyTo(fileStream);
                    fileStream.Flush();

                    var hasFile = fileContext.findFileByUserId(user.id);
                    if(hasFile == null){
                        var file = new FileM() { 
                            filename = files[0].FileName,
                            path = RelativeImagePath,
                            UserId = user.id
                        };

                        fileContext.addFile(file);
                        return Ok("\\Upload\\"+RelativeImagePath);
                    }   
                    return BadRequest();
                }
            }catch (Exception ex){
                return BadRequest(ex.Message);
            }
        }
    }
}