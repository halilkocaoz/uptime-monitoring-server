using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UpMo.Common.DTO.Request;
using UpMo.Services.Abstract;
using UpMo.WebAPI.Controllers.Base;

namespace UpMo.WebAPI.Controllers
{
    [Route("[controller]")]
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService) =>
            _userService = userService;

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request) =>
            ApiResponse(await _userService.SignInWithPasswordAsync(request));

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request) =>
            ApiResponse(await _userService.SignUpWithPasswordAsync(request));
    }
}