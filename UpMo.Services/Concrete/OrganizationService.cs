using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UpMo.Common.DTO.Request;
using UpMo.Common.DTO.Response;
using UpMo.Common.Response;
using UpMo.Data;
using UpMo.Entities;
using UpMo.Services.Abstract;

namespace UpMo.Services.Concrete
{
    public class OrganizationService : BaseService, IOrganizationService
    {
        public OrganizationService(IMapper mapper,
                                   UpMoContext context) : base(mapper, context)
        {
        }

        public async Task<ApiResponse> CreateAsync(OrganizationCreateRequest request)
        {
            var organization = _mapper.Map<Organization>(request);

            var organizationManager = new OrganizationManager // todo mapper
            {
                ID = System.Guid.NewGuid(),
                OrganizationID = organization.ID,
                UserID = organization.CreatorUserID,
                Admin = true,
                Viewer = true
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                await _context.AddAsync(organization);
                await _context.AddAsync(organizationManager);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }

            return new ApiResponse(ResponseStatus.Created, _mapper.Map<OrganizationResponse>(organization));
        }

        /// <summary>
        /// Checks Admin, Creator or not by <see cref="authenticatedUserID"/> for Organization.
        /// </summary>
        /// <param name="authenticatedUserID"></param>
        /// <param name="toBeCheckedOrganization"></param>
        /// <returns>true if creator or admin otherwise, false.</returns>
        private async Task<bool> checkAdminOrCreatorForOrganizationByUserIdAsync(int authenticatedUserID, Organization toBeCheckedOrganization)
        {
            if (toBeCheckedOrganization.CreatorUserID.Equals(authenticatedUserID)) //before not pulling managers and using managers to check
                return true;

            if (toBeCheckedOrganization.Managers is null)
                toBeCheckedOrganization.Managers = await _context.OrganizationManagers.Where(x => x.OrganizationID == toBeCheckedOrganization.ID).ToListAsync();

            return toBeCheckedOrganization.Managers.Any(x => x.Admin && x.UserID == authenticatedUserID);
        }

        public async Task<ApiResponse> UpdateAsyncByID(Guid toBeUpdatedOrganizationID, OrganizationUpdateRequest request)
        {
            var toBeUpdatedOrganization = await _context.Organizations.SingleOrDefaultAsync(x => x.ID == toBeUpdatedOrganizationID);
            if (toBeUpdatedOrganization is null)
                return new ApiResponse(ResponseStatus.NotFound, ResponseMessage.NotFoundOrganization);

            bool authenticatedUserAuthorizedForUpdatingOrganization = await checkAdminOrCreatorForOrganizationByUserIdAsync(request.AuthenticatedUserID, toBeUpdatedOrganization);
            if (authenticatedUserAuthorizedForUpdatingOrganization)
            {
                toBeUpdatedOrganization.Name = request.Name;
                await _context.SaveChangesAsync();

                return new ApiResponse(ResponseStatus.OK, _mapper.Map<OrganizationResponse>(toBeUpdatedOrganization));
            }

            return new ApiResponse(ResponseStatus.Forbid, ResponseMessage.Forbid);
        }

        public async Task<ApiResponse> GetOrganizationsByAuthenticatedUserIDAsync(int authenticatedUserID)
        {
            var organizationsForAuthenticatedUser = await _context.Organizations
                                                    .Include(x => x.Managers)
                                                    .Include(x => x.Monitors)
                                                    .AsSplitQuery()
                                                    .Where(x => x.CreatorUserID == authenticatedUserID || x.Managers.Any(x => x.Viewer && x.UserID == authenticatedUserID))
                                                    .ToListAsync();

            object returnObject = new { organizations = _mapper.Map<List<OrganizationResponse>>(organizationsForAuthenticatedUser) };
            return new ApiResponse(ResponseStatus.OK, returnObject);
        }
    }
}