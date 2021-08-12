using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UpMo.Common.DTO.Request;
using UpMo.Services.Abstract;
using UpMo.WebAPI.Extensions;

namespace UpMo.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]")]
    public class OrganizationsController : BaseController
    {
        private readonly IOrganizationService _organizationService;
        public OrganizationsController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(OrganizationCreateRequest request)
        {
            request.CreatorUserID = User.GetId();
            return ApiResponse(await _organizationService.CreateAsync(request));
        }
    }
}