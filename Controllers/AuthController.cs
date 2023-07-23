using System.Buffers.Text;
using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AnnotationsAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AnnotationsAPI.Models;
using System.Net.Mail;
using System.Net;
using System.ComponentModel;
using AnnotationsAPI.Context;

namespace AnnotationsAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<UserAplication> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationContext _applicationContext;

        public AuthController(
            IConfiguration configuration,
            UserManager<UserAplication> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationContext applicationContext
            )
        {
            _configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
            _applicationContext = applicationContext;

        }

        /* TODO: Confirm user email
        When the user registers via the /api/auth/register endpoint, 
        a confirmation code should be sent to the user's email, 
        and the user should use the code in the /api/auth/confirm-email endpoint
        */

        /// <summary> User register </summary>
        /// <returns> Returns the user created </returns>
        /// <response code="400"> If any field is missing, invalid or there is already a user with the registered email </response>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<UserAplication>> CreateUserAsync([FromBody] UserDto userDto)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(userDto.Email);
                if (userExists is not null)
                {
                    return StatusCode(
                        StatusCodes.Status400BadRequest,
                        new { Success = false, Message = "User already exists!" }
                    );
                }

                UserAplication user = new()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Email = userDto.Email,
                    UserName = userDto.Email,
                    Name = userDto.Name
                };

                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (!result.Succeeded)
                {
                    Console.WriteLine(result.Errors);
                    string errorMessage = "Error creating user.";
                    foreach (var erro in result.Errors)
                    {
                        errorMessage += " " + erro.Description;
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = errorMessage });
                }

                if (!await _roleManager.RoleExistsAsync("USER"))
                {
                    await _roleManager.CreateAsync(new("USER"));
                }

                await _userManager.AddToRoleAsync(user, "USER");

                user.PasswordHash = "";
                user.SecurityStamp = "";
                user.ConcurrencyStamp = "";

                return Created(nameof(LoginAsync), user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error registering user!", Description = ex.Message });
            }
        }

        /// <summary> User login </summary>
        /// <returns> Return a token </returns>
        /// <response code="401"> If the credentials are wrong </response>
        /// <response code="400"> If any field is missing or invalid </response>
        /// <response code="404"> If user not exists </response>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto userDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email);

                if (user is not null && await _userManager.CheckPasswordAsync(user, userDto.Password))
                {
                    string token = await GetTokenAsync(user);
                    return Ok(new { Message = $"Bearer {token}" });
                }
                else if (user == null)
                {
                    return NotFound(new { Message = $"There is not even an account with email: {userDto.Email}" });
                }

                return Unauthorized(new { Message = "Incorrect password" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Login error!" });
            }
        }

        /// <summary> Get account data that is authenticated </summary>
        /// <returns> Return account data </returns>
        /// <response code="401"> If not authenticated </response>
        [Authorize]
        [HttpGet("account-data")]
        public async Task<ActionResult<UserDtoResponse>> GetAccountData()
        {
            try
            {
                var id = User?.Identity?.Name;
                var user = await _userManager.FindByIdAsync(id);
                var userRoles = await _userManager.GetRolesAsync(user);
                UserDtoResponse userDtoResponse = new UserDtoResponse();

                userDtoResponse.Email = user.Email;
                userDtoResponse.Name = user.Name;
                userDtoResponse.Id = user.Id;
                userDtoResponse.Roles = userRoles;

                return Ok(userDtoResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error geting account data!" });
            }
        }

        /// <summary> Fully update user account data </summary>
        /// <returns> Returns updated account data </returns>
        /// <response code="401"> If not authenticated </response>
        /// <response code="400"> If any fields are missing or invalid </response>
        [Authorize]
        [HttpPut("account-update")]
        public async Task<ActionResult<UserAplication>> PutUpdateAccount([FromBody] UserDto userDto)
        {
            try
            {
                UserDtoNotValidate userDtoNotValidate = new()
                {
                    Email = userDto.Email,
                    Name = userDto.Name,
                    Password = userDto.Password
                };
                var id = User?.Identity?.Name;
                var user = await UpdateUser(userDtoNotValidate, id);

                return Ok(user);
            }
            catch (BadHttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error updating account!" });
            }
        }

        /// <summary> Partially update user account data </summary>
        /// <returns> Returns updated account data </returns>
        /// <response code="401"> If not authenticated </response>
        /// <response code="400"> If any fields are missing or invalid </response>
        [Authorize]
        [HttpPatch("account-update")]
        public async Task<IActionResult> PatchUpdateAccount([FromBody] UserDtoNotValidate userDtoNotValidate)
        {
            try
            {
                var id = User?.Identity?.Name;
                await UpdateUser(userDtoNotValidate, id);
                return Ok(new { message = "Update success" });
            }
            catch (BadHttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error updating account!" });
            }
        }

        /// <summary> Authenticated user delete own account </summary>
        /// <response code="204"> If account deleted success </response>
        /// <response code="401"> if unauthenticated </response>
        [Authorize]
        [HttpDelete("delete-account")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var id = User?.Identity?.Name;
                var user = await _userManager.FindByIdAsync(id);

                await _userManager.DeleteAsync(user);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error deleting account!", Description = ex.Message });
            }
        }

        /// <summary> Send forgotten password reset code to email </summary>
        /// <returns> Returns message notifying that the email was sent </returns>
        /// <response code="404"> If there is no user with the entered email </response>
        /// <response code="400"> If any field is missing or invalid </response>
        [HttpPost("forgotten-password/send-email-code")]
        public async Task<IActionResult> SendEmailCode(EmailToDto emailToDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(emailToDto.Email);
                if (user == null)
                {
                    return NotFound(new { Message = $"Email user {emailToDto.Email} not found!" });
                }

                Random random = new Random();
                long code = random.Next(1000000, 2000000);

                AuthorizationCode authorizationCode = new()
                {
                    Code = code,
                    UserAplicationId = user.Id
                };
                _applicationContext.AuthorizationCodes.Add(authorizationCode);
                _applicationContext.SaveChanges();

                bool isSent = await SendEmailAsync(
                    emailToDto.Email,
                    "Contact System - Your code reset forgotten password",
                    $"Your password reset code is {code}, valid for 15 minutes."
                );

                if (isSent)
                {
                    string uri = $"{Request.Scheme}://{Request.Host}/api/auth/forgotten-password/change-password";
                    return Ok(new
                    {
                        Message = $"Code send to {emailToDto.Email}",
                        Description = $"Use code in {uri}"
                    });
                }

                _applicationContext.AuthorizationCodes.Remove(authorizationCode);
                _applicationContext.SaveChanges();

                return StatusCode(
                   StatusCodes.Status400BadRequest,
                   new
                   {
                       Message = "Error sending email",
                   }
                );

            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "Error sending email!",
                        Description = ex.Message
                    });

            }
        }

        /// <summary> Reset password using code sent to email </summary>
        /// <returns> Returns message notifying that the password was successfully reset </returns>
        /// <response code="404"> If the entered code is incorrect or does not exist, or if there is no user with the entered email </response>
        /// <response code="400"> If any fields are missing, invalid, or the code is leaked </response>
        [HttpPut("forgotten-password/change-password")]
        public async Task<IActionResult> ChangeFogottenPassword(ChangeForgottenPasswordDto changeForgottenPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(changeForgottenPasswordDto.Email);
                if (user == null)
                {
                    return NotFound(new { Message = $"Email user {changeForgottenPasswordDto.Email} not found!" });
                }

                var codeForChangeForgottenPassword = _applicationContext.AuthorizationCodes
                                                    .Where(x => x.Code == changeForgottenPasswordDto.Code
                                                     && x.UserAplicationId == user.Id).FirstOrDefault();
                if (codeForChangeForgottenPassword == null)
                {
                    return NotFound(new { Message = "Code entered does not exist!" });
                }

                if (DateTime.UtcNow > codeForChangeForgottenPassword.CodeExpires)
                {
                    _applicationContext.AuthorizationCodes.Remove(codeForChangeForgottenPassword);
                    _applicationContext.SaveChanges();

                    return StatusCode(
                        StatusCodes.Status400BadRequest,
                        new { Message = "The maximum time in the code has expired" }
                    );
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resultUpdatePassword = await _userManager.ResetPasswordAsync(user, token, changeForgottenPasswordDto.NewPassword);

                if (!resultUpdatePassword.Succeeded)
                {
                    return StatusCode(
                         StatusCodes.Status400BadRequest,
                         new
                         {
                             Message = "The maximum time in the code has expired",
                             Erros = resultUpdatePassword.Errors
                         }
                     );
                }

                return Ok(new { Message = "Password successfully updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "Error updating password!",
                        Description = ex.Message
                    });

            }
        }

        /// <summary> Admin: Get all users </summary>
        /// <returns> Return a page of users </returns>
        /// <response code="401"> if unauthenticated </response>
        /// <response code="403"> if non-admin  </response>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("list-all-users")]
        public async Task<ActionResult<PageResponse<List<UserDtoResponse>>>> GetAllUsers([FromQuery] Pagination pagination)
        {
            try
            {
                var validPagination = new Pagination(pagination.Page, pagination.Size);

                var users = _userManager.Users
                    .Skip((validPagination.Page - 1) * validPagination.Size)
                    .Take(validPagination.Size)
                    .ToList();

                List<UserDtoResponse> usersResponse = new List<UserDtoResponse>();

                foreach (var user in users)
                {
                    UserDtoResponse userDtoResponse = new UserDtoResponse();
                    var userRoles = await _userManager.GetRolesAsync(user);
                    userDtoResponse.Email = user.Email;
                    userDtoResponse.Name = user.Name;
                    userDtoResponse.Id = user.Id;
                    userDtoResponse.Roles = userRoles;
                    usersResponse.Add(userDtoResponse);
                }

                var totalRecords = _userManager.Users.Count();

                string baseUri = $"{Request.Scheme}://{Request.Host}/api/auth/list-all-users";

                PageResponse<List<UserDtoResponse>> pagedResponse = new(
                    data: usersResponse,
                    page: validPagination.Page,
                    size: validPagination.Size,
                    totalRecords: totalRecords,
                    uri: baseUri
                );

                return Ok(pagedResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error geting users!", Description = ex.Message });
            }
        }

        /// <summary> Admin: Find user by email </summary>
        /// <returns> Return a page of users </returns>
        /// <response code="401"> if unauthenticated </response>
        /// <response code="403"> if non-admin  </response>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("find-user-by-email/{email}")]
        public async Task<ActionResult<PageResponse<List<UserDtoResponse>>>> FindUserByEmail([FromQuery] Pagination pagination, string email)
        {
            try
            {
                var validPagination = new Pagination(pagination.Page, pagination.Size);

                var users = _userManager.Users
                    .Where(user => user.Email.Contains(email))
                    .Skip((validPagination.Page - 1) * validPagination.Size)
                    .Take(validPagination.Size)
                    .ToList();

                List<UserDtoResponse> usersResponse = new List<UserDtoResponse>();

                foreach (var user in users)
                {
                    UserDtoResponse userDtoResponse = new UserDtoResponse();
                    var userRoles = await _userManager.GetRolesAsync(user);
                    userDtoResponse.Email = user.Email;
                    userDtoResponse.Name = user.Name;
                    userDtoResponse.Id = user.Id;
                    userDtoResponse.Roles = userRoles;
                    usersResponse.Add(userDtoResponse);
                }

                var totalRecords = _userManager.Users.Where(user => user.Email.Contains(email)).Count();

                string baseUri = $"{Request.Scheme}://{Request.Host}/api/auth/find-user-by-email/{email}";

                PageResponse<List<UserDtoResponse>> pagedResponse = new(
                    data: usersResponse,
                    page: validPagination.Page,
                    size: validPagination.Size,
                    totalRecords: totalRecords,
                    uri: baseUri
                );

                return Ok(pagedResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error geting users!" });
            }
        }

        /// <summary> Admin delete a user </summary>
        /// <response code="204"> If user deleted success </response>
        /// <response code="401"> if unauthenticated </response>
        /// <response code="403"> if non-admin  </response>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete-a-user/{userEmail}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAUser(string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return NotFound(new { Message = $"User {userEmail} not found" });
                }

                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any(role => role == "ADMIN"))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "An admin cannot delete another admin." });
                }

                await _userManager.DeleteAsync(user);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error deleting user!", Description = ex.Message });
            }
        }

        /// <summary> Admin fully updates a user's data </summary>
        /// <returns> Returns updated user data </returns>
        /// <response code="401"> if unauthenticated </response>
        /// <response code="403"> if non-admin  </response>
        /// <response code="400"> If any fields are missing or invalid </response>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("update-a-user/{userEmail}")]
        public async Task<ActionResult<UserAplication>> PutUpdateAUser(string userEmail, [FromBody] UserDto userDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return NotFound(new { Message = $"User {userEmail} not found" });
                }

                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any(role => role == "Admin"))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "An admin cannot update another admin." });
                }

                UserDtoNotValidate userDtoNotValidate = new()
                {
                    Email = userDto.Email,
                    Name = userDto.Name,
                    Password = userDto.Password
                };

                var userUpdated = await UpdateUser(userDtoNotValidate, user.Id);

                return Ok(userUpdated);
            }
            catch (BadHttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error updating account!" });
            }
        }

        /// <summary> Admin partially updates a user's data </summary>
        /// <returns> Returns updated user data </returns>
        /// <response code="401"> if unauthenticated </response>
        /// <response code="403"> if non-admin  </response>
        /// <response code="400"> If any fields are missing or invalid </response>
        [Authorize(Roles = "ADMIN")]
        [HttpPatch("update-a-user/{userEmail}")]
        public async Task<IActionResult> PatchUpdateAUser(string userEmail, [FromBody] UserDtoNotValidate userDtoNotValidate)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return NotFound(new { Message = $"User {userEmail} not found" });
                }

                var userUpdated = await UpdateUser(userDtoNotValidate, user.Id);

                return Ok(userUpdated);
            }
            catch (BadHttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error updating account!" });
            }
        }

        /* Function to send email */
        private async Task<bool> SendEmailAsync(string email, string subject, string body)
        {
            try
            {
                var client = new SmtpClient(_configuration["Email:Host"], Convert.ToInt32(_configuration["Email:Port"]))
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_configuration["Email:EmailFrom"], _configuration["Email:PasswordFrom"])
                };

                await client.SendMailAsync(
                     new MailMessage(
                         from: _configuration["Email:EmailFrom"],
                         to: email,
                         subject: subject,
                         body: body
                     ));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /* Generate token jwt */
        private async Task<string> GetTokenAsync(UserAplication user)
        {

            var authClaims = new List<Claim>
            {
                new (ClaimTypes.Name, user.Id),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddDays(30),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        /* Function to update the user, used for PutUpdateAccount() and PatchUpdateAccount() */
        private async Task<UserAplication> UpdateUser(UserDtoNotValidate userDtoNotValidate, string userId)
        {
            var userLogged = await _userManager.FindByIdAsync(userId);

            if (!userDtoNotValidate.Name.IsNullOrEmpty())
            {
                userLogged.Name = userDtoNotValidate.Name;
            }

            if (!userDtoNotValidate.Email.IsNullOrEmpty())
            {
                string strModel = "^([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(userDtoNotValidate.Email, strModel))
                {
                    throw new BadHttpRequestException("Email must be well-formed");
                }

                var userExists = await _userManager.FindByEmailAsync(userDtoNotValidate.Email);
                if (userExists is not null && userExists.Email.ToString() != userLogged.Email)
                {
                    throw new BadHttpRequestException($"User with {userDtoNotValidate.Email} already exists!");
                }

                userLogged.Email = userDtoNotValidate.Email;
                userLogged.UserName = userDtoNotValidate.Email;
            }

            var resultUpdateData = await _userManager.UpdateAsync(userLogged);

            if (!resultUpdateData.Succeeded)
            {
                string errorMessage = "Error updating data account! ";
                foreach (var erro in resultUpdateData.Errors)
                {
                    errorMessage += erro.Description;
                }
                throw new Exception(errorMessage);
            }

            if (!userDtoNotValidate.Password.IsNullOrEmpty())
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(userLogged);
                var resultUpdatePassword = await _userManager.ResetPasswordAsync(userLogged, token, userDtoNotValidate.Password);

                if (!resultUpdatePassword.Succeeded)
                {
                    Console.WriteLine(resultUpdatePassword.Errors);
                    throw new Exception("Error update password!");
                }
            }

            userLogged.PasswordHash = "";
            userLogged.SecurityStamp = "";
            userLogged.ConcurrencyStamp = "";

            return userLogged;

        }

    }
}