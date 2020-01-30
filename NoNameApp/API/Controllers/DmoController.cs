using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Account;
using System;
using System.Threading.Tasks;
using API.DTO;
using API.Infrastructure.Authentication;
using Model;
using Model.Entities;

namespace API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DmoController: ControllerBase {

        private readonly IDmoRepository _dmoRepository;
        private readonly CurrentUserService _currentUserService;

        public DmoController(
            IDmoRepository dmoRepository, 
            IUserRepository userRepository, 
            CurrentUserService currentUserService) {
            _dmoRepository = dmoRepository ?? throw new ArgumentNullException(nameof(dmoRepository));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }


        //temporary solution
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> AddDmo(DmoDto dmo) {
            var user = await _currentUserService.GetAsync();

            await _dmoRepository.Add(new Dmo {
                NoNameUser = user,
                NoNameUserId = user.Id,
                Name = dmo.Name,
                MovieTitle = dmo.MovieTitle
            });

            //if (user.UserName == "test3") {
            //    throw new Exception();
            //}

            return NoContent();
        }
    }
}
