using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Account;
using System;
using System.Threading.Tasks;
using API.DTO;
using API.Helpers;
using API.Infrastructure.Authentication;
using Model;
using Model.Entities;

namespace API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DmoListController: ControllerBase {

        private readonly IDmoCollectionRepository _dmoCollectionRepository;
        private readonly CurrentUserService _currentUserService;
        private readonly ResponseBuilder _responseBuilder;

        public DmoListController(
            IDmoCollectionRepository dmoCollectionRepository,
            CurrentUserService currentUserService, 
            ResponseBuilder responseBuilder) {
            _dmoCollectionRepository = dmoCollectionRepository ?? throw new ArgumentNullException(nameof(dmoCollectionRepository));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAll() {
            var user = await _currentUserService.GetAsync();
            var dmoCollections = await _dmoCollectionRepository.GetAllAsync(user.Id);

            return Ok();//todo add later
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Add(DmoListDto dmoList) {
            var user = await _currentUserService.GetAsync();

            if (await _dmoCollectionRepository.IsExist(dmoList.Name, user.Id)) {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage($"List with name '{dmoList.Name}' is already exist"));
            }

            await _dmoCollectionRepository.AddAsync(new UserDmoCollection {
                NoNameUser = user,
                NoNameUserId = user.Id,
                CollectionName = dmoList.Name
            });

            return NoContent();
        }
    }
}
