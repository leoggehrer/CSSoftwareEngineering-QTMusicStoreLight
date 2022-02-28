﻿using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QTMusicStoreLight.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumsController : GenericController<Logic.Entities.Album, Models.AlbumEdit, Models.Album>
    {
        public AlbumsController(Logic.Controllers.AlbumsController controller)
            : base(controller)
        {

        }
    }
}
