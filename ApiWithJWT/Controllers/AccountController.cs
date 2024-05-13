

using ApiWithJWT.Dtos;
using ApiWithJWT.Services;
using ApiWithJWT.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiWithJWT.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountService accountService) : ControllerBase
{
	[HttpPost(nameof(Authenticate))]
	public async Task<BaseResult<AuthenticationResponse>> Authenticate(AuthenticationRequest request)
		   => await accountService.Authenticate(request);


	[HttpPost(nameof(Register))]
	public async Task<BaseResult> Register(RegistrationRequest request)
		   => await accountService.RegisterUser(request);


	[HttpGet,Authorize]
	public IActionResult GetStart()
	{
		return Ok("Welcome ^_^");
	}
}
