using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServerMongo.Models;
using IdentityServerMongo.Repository;
using IdentityServerMongo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoCoreDbRepository.Interfaces;
using MongoDB.Driver;
using PasswordHasher = ServiceStack.Auth.PasswordHasher;

namespace IdentityServerMongo.Controllers
{
    [Route("identity_server")]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository; 
        private PasswordHasher hash = new PasswordHasher();
        private readonly IUnitOfWork _uow;
        

        public UserController(IConfiguration configuration, IUserRepository userRepository, IUnitOfWork unitOfWork, IRoleRepository roleRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _uow = unitOfWork;
            _roleRepository = roleRepository;
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Login([FromBody]LoginEntity user)
        {
            var builder = Builders<ApplicationUser>.Filter;
            
            bool needRehash;
            FilterDefinition<ApplicationUser> filter = new ExpressionFilterDefinition<ApplicationUser>(x => x.UserName == user.Email);

            var userDbSearch = await _userRepository.GetByFilter(filter,0, 10, "UserName");

            var userDb = userDbSearch.FirstOrDefault();
            bool passwordIsCorrect = hash.VerifyPassword(userDb.Password, user.Password, out needRehash);
            
            if (!userDbSearch.Any() || !passwordIsCorrect)
                return NotFound(new { message = "Usuário ou senha inválidos" });
            
            var token = TokenService.GenerateToken(userDb);
            var expirationTime = DateTime.Now.AddHours(HashingOptions.ExpirationInHours);
            user.Password = null;
            return Ok(new LoginResponse(userDb.Id, userDb.Email, userDb.UserName, userDb.Name, userDb.LastName,
                userDb.Roles, token, expirationTime.ToString()));
        }
        
        [HttpGet]
        [Authorize]
        [Route("user_data")]
        public async Task<LoginResponse> UserData()
        {
            var userId = User.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid").FirstOrDefault().Value;
            var expirationTime = User.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/expiration").FirstOrDefault().Value;
            //var expirationDate = new DateTime(Convert.ToInt64(exp)*1000); //unix timestamp is in seconds, javascript in milliseconds
            
            var userDb = await _userRepository.GetById(Guid.Parse(userId));
            return new LoginResponse(userDb.Id, userDb.Email, userDb.UserName, userDb.Name, userDb.LastName,
                userDb.Roles, "", expirationTime);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("role")]
        public async  Task<IActionResult> AddRoles([FromBody] RoleEntity role)
        {
            var newRole = new ApplicationRole(role.Name);
            
            var roleExists =
                await _roleRepository.GetByFilter(
                    new ExpressionFilterDefinition<ApplicationRole>(x => x.Name == role.Name), 0, 1, "UserName");
            
            if (roleExists.Count() > 0)
                return BadRequest("Role " + role.Name + " já registrado.");
            
            _roleRepository.Add(newRole);
            
            bool result = await _uow.Commit();
            if (result)
                return Ok(newRole);
            
            return BadRequest("Ocorreu um erro ao gravar no banco de dados.");
        }

        [HttpPut]
        [Authorize(Roles="Admin")]
        [Route("role_to_user")]
        public async  Task<IActionResult> AddRolesToUser([FromBody] RoleToUserEntity roleToUser)
        {
            var role = await _roleRepository.GetById(roleToUser.RoleId);
            var user = await _userRepository.GetById(roleToUser.UserId);
            
            if (role == null || user == null)
                return BadRequest("Ids informados incorretos.");
            
            if (user.Roles == null)
                user.Roles = new List<ApplicationRole>();
            
            user.Roles.Add(role);
            
            _userRepository.Update(user);

            bool result = await _uow.Commit();
            if (result)
                return Ok("Role "+ role.Name + " adicionada para o usuário " + user.UserName);
            
            return BadRequest("Ocorreu um erro ao gravar no banco de dados.");
        }
        
        
        
        [HttpGet]
        [Route("is_authenticated")]
        [Authorize]
        public string Authenticated() => String.Format("Autenticado - {0}", User.Identity.Name);
        
        // POST api/user/register
        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterEntity model)
        {
            if (ModelState.IsValid)
            {
                var userDb = new ApplicationUser {Name = model.Name, LastName = model.LastName, UserName = model.Email, Email = model.Email, Password = hash.HashPassword(model.Password)};

                var userExists =
                   await _userRepository.GetByFilter(
                        new ExpressionFilterDefinition<ApplicationUser>(x => x.UserName == model.Email), 0, 1,"UserName");

                if (userExists != null)
                    return BadRequest("E-mail " + model.Email + " já registrado.");
                
                _userRepository.Add(userDb);
               bool result = await _uow.Commit();
                if (result)
                {
                    var token = TokenService.GenerateToken(userDb);
                    var expirationTime = DateTime.Now.AddHours(HashingOptions.ExpirationInHours);
   
                    return Ok(new LoginResponse(userDb.Id, userDb.Email, userDb.UserName, userDb.Name, userDb.LastName,
                        userDb.Roles, token, expirationTime.ToString()));
                }
                return BadRequest("Ocorreu um erro ao gravar no banco de dados.");
            }
            string errorMessage = string.Join(", ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            return BadRequest(errorMessage ?? "Bad Request");
        }
    }
}