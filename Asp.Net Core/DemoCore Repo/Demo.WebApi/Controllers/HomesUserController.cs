using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Abstractions;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.WebApi.Controllers
{
    public class HomesUserController : Controller
    {
        IHomesUserService _service;


        public HomesUserController(IHomesUserService service)
        {
            _service = service ?? throw new ArgumentNullException();
        }
        [Route("api/homes-users")]
        [HttpGet]
        public async Task<ICollection<HomesUser>> Get()
        {
            return await _service.ReadAllAsync();
        }

        [Route("api/homes-users/{UserId}")]
        [HttpGet]
        public async Task<HomesUser> Get(long userId)
        {
            var HomesUserList = await _service.ReadByIdAsync(userId);
            return HomesUserList.FirstOrDefault();
        }

        [Route("api/homes-users")]
        [HttpPost]
        public async Task<ICollection<HomesUser>> Post([FromBody]ICollection<HomesUser> homesUser)
        {
            return await _service.AddUsers(homesUser);
        }

        [Route("api/homes-users")]
        [HttpPatch]
        public async Task<HomesUser> Patch([FromBody]HomesUser homesUser)
        {
            return await _service.UpdateAsync(homesUser);
        }
    }
}