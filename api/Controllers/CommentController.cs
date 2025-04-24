// using api.Data;
// using api.Dtos.Comment;
// using api.Interfaces;
// using api.Mappers;
// using api.Models;
// using api.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace api.Controllers
// {
//     [Route("api/comment")]
//     [ApiController]
//     public class CommentController : ControllerBase
//     {
//         private readonly IComment icomment;
//         private readonly ApplicationDBContext context;
//         private readonly iStock iStock;

//         public CommentController(IComment icomment, ApplicationDBContext context, iStock iStock)
//         {
//             this.icomment =icomment;
//             this.context = context;
//             this.iStock = iStock;

//         }

//         [HttpGet]
//         public async Task<IActionResult> GetAll(){

//             // this is responsible for the validation we added in the dtos (model state came from basecontroller)
//             if(!ModelState.IsValid)
//                 return BadRequest(ModelState);
            
//             var comments = await icomment.GetAllAsync();

//             var commentDto = comments.Select(s => s.ToCommentDto()); // using mappers
//             return Ok(commentDto);

//         }

//         [HttpGet("{id:int}")]
//         public async Task<IActionResult> GetById([FromRoute] int id){

//             if(!ModelState.IsValid)
//                 return BadRequest(ModelState);

//             var comment = await icomment.GetByIdAsync(id);

//             if(comment == null){
//                 return NotFound();
//             }
//             return Ok(comment.ToCommentDto());

//         }

//          [HttpPost("{stockId:int}")]
//          public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentDto commentDto)
//          {

//             if(!ModelState.IsValid)
//                 return BadRequest(ModelState);

//             if(!await iStock.StockExists(stockId))
//             {
//                 return BadRequest("No not Exist");
//             }

//             var commentModel = commentDto.ToCommentFromCreate(stockId);
//             await icomment.CreateAsync(commentModel);

//             return CreatedAtAction(nameof(GetById), new {id = commentModel.Id}, commentModel.ToCommentDto());

//             //return CreatedAtAction("target method", route parameters, response body);
//             //This tells ASP.NET Core:“Use the GetById method in this controller to generate the URL of the newly created resource.

//             // Part 2: new { id = commentModel.Id }
//             // This provides the parameter for the URL.
//             // GetById needs an id
//             // You're giving it commentModel.Id
//             // For example, if the new comment’s ID is 15, it builds this URL:
//             // /api/comment/15

//             // Part 3: commentModel.ToCommentDto()
//             // This is the actual content of the response body — what gets returned as JSON.
//             // You're converting the internal Comment model into a clean CommentDto (e.g., hiding fields you don’t want to expose).
//             // The client gets this back as the body of the 201 Created response.

//          }

//            [HttpPut]
//         [Route("{id:int}")]
//         public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto)
//         {

//             if(!ModelState.IsValid)
//                 return BadRequest(ModelState);

//             var comment = await icomment.UpdateAsync(id, updateDto.ToCommentFromUpdate(id));

//             if (comment == null)
//             {
//                 return NotFound();
//             }

//             return Ok(comment.ToCommentDto());
//         }


//          [HttpDelete]
//         [Route("{id:int}")]
//         public async Task<IActionResult> Delete([FromRoute] int id)
//         {

//             if(!ModelState.IsValid)
//                 return BadRequest(ModelState);

//             var commentModel = await icomment.DeleteAsync(id);

//             if (commentModel == null)
//             {
//                 return NotFound();
//             }

//             return NoContent();
//         }
//     }
// }


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IComment _commentRepo;
        private readonly iStock _stockRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFMPService _fmpService;
        public CommentController(IComment commentRepo,
        iStock stockRepo, UserManager<AppUser> userManager,
        IFMPService fmpService)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
            _userManager = userManager;
            _fmpService = fmpService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] CommentQueryObject queryObject)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comments = await _commentRepo.GetAllAsync(queryObject);

            var commentDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = await _commentRepo.GetByIdAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpPost]
        [Route("{symbol:alpha}")]
        public async Task<IActionResult> Create([FromRoute] string symbol, CreateCommentDto commentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stock = await _stockRepo.GetBySymbolAsync(symbol);

            if (stock == null)
            {
                stock = await _fmpService.FindStockBySymbolAsync(symbol);
                if (stock == null)
                {
                    return BadRequest("Stock does not exists");
                }
                else
                {
                    await _stockRepo.CreateAsync(stock);
                }
            }

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var commentModel = commentDto.ToCommentFromCreate(stock.Id);
            commentModel.AppUserId = appUser.Id;
            await _commentRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = await _commentRepo.UpdateAsync(id, updateDto.ToCommentFromUpdate(id));

            if (comment == null)
            {
                return NotFound("Comment not found");
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var commentModel = await _commentRepo.DeleteAsync(id);

            if (commentModel == null)
            {
                return NotFound("Comment does not exist");
            }

            return Ok(commentModel);
        }
    }
}