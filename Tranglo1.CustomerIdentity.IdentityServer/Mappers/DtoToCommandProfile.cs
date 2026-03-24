using AutoMapper;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Tranglo1.CustomerIdentity.IdentityServer.Command;
using Tranglo1.CustomerIdentity.IdentityServer.CustomerUserList.Commands;
using Tranglo1.CustomerIdentity.IdentityServer.DTO;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.CustomerUser;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.KYCAdminManagement.AdminManagement;
using Tranglo1.CustomerIdentity.IdentityServer.Models;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;

namespace Tranglo1.CustomerIdentity.IdentityServer.Mappers
{
	internal class DtoToCommandProfile : Profile
	{
		public DtoToCommandProfile()
		{
			CreateMap<ForgotPasswordModel, RequestResetPasswordCommand>();
			CreateMap<CreatePasswordModel, VerifyCustomerUserResetPasswordCommand>();
			CreateMap<LoginInputModel, ValidateUserPasswordCommand>();
			CreateMap<RegisterInputModel, RegisterCustomerUserCommand>();
			CreateMap<RegisterWithRegistryCodeInputModel, RegisterCustomerUserWithCodeCommand>();

			CreateMap<InviteUserInputDTO, InviteUserCommand>();
			CreateMap<InviteePasswordVerificationViewModel, InviteePasswordVerificationCommand>();
			CreateMap<UnlockUserInputDTO, UnlockUserCommand>();
			CreateMap<LockUserInputDTO, LockUserCommand>();
			CreateMap<TrangloEntityBlockStatusInputDTO, UpdateTrangloStaffBlockStatusCommand>();
			CreateMap<TrangloStaffUserUpdateInputDTO, UpdateTrangloStaffAssignmentCommand>();
			CreateMap<VerifyCustomerUserEmailModel, VerifyCustomerUserEmailCommand>()
				.ForMember(
				command => command.Token,
				mapping => mapping.MapFrom(
					dto => Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token))));

			CreateMap<ResendInvitationInputDTO, ResendInvitationCommand>();
		}
	}
}
