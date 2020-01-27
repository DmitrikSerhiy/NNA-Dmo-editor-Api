using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Account;
using System;
using System.Threading.Tasks;
using API.DTO;
using Model;
using Model.Entities;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DmoController: ControllerBase {

        private readonly IDmoRepository _dmoRepository;
        private readonly IUserRepository _userRepository;

        public DmoController(
            IDmoRepository dmoRepository, 
            IUserRepository userRepository) {
            _dmoRepository = dmoRepository ?? throw new ArgumentNullException(nameof(dmoRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }


        //temporary solution
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> AddDmo(DmoDto dmo, Guid userId) {
            var user = await _userRepository.WithId(userId);

            await _dmoRepository.Add(new Dmo {
                NoNameUser = user,
                NoNameUserId = userId,
                Name = dmo.Name,
                MovieTitle = dmo.MovieTitle
            });

            return NoContent();
        }
    }
}
