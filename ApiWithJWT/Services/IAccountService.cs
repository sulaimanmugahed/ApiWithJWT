using ApiWithJWT.Dtos;
using ApiWithJWT.Wrappers;

namespace ApiWithJWT.Services;
public interface IAccountService
{
	Task<BaseResult<AuthenticationResponse>> Authenticate(AuthenticationRequest request);
	Task<BaseResult> RegisterUser(RegistrationRequest request);
}