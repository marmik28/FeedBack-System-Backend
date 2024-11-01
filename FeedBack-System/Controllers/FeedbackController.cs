using FeedBack_System.Models;
using FeedBack_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FeedBack_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;

        public FeedbackController(FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [Authorize(Roles = "Employee,Admin")]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserFeedbacks(string userId)
        {
            var feedbacks = await _feedbackService.GetFeedbacksByUserAsync(userId);
            return Ok(feedbacks);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
            return Ok(feedbacks);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateFeedback([FromBody] Feedback feedback)
        {
            await _feedbackService.CreateFeedbackAsync(feedback);
            return CreatedAtAction(nameof(CreateFeedback), new { id = feedback.Id }, feedback);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("updateCommentById")]
        public async Task<IActionResult> UpdateCommentByFeedbackId([FromBody] UpdateCommentByIdRequest request)
        {
            var result = await _feedbackService.UpdateFeedbackCommentByIdAsync(request.FeedbackId, request.Comment);

            if (result.ModifiedCount > 0)
            {
                return NoContent();
            }
            return NotFound("Feedback with this ID not found");
        }

        public class UpdateCommentByIdRequest
        {
            public string? FeedbackId { get; set; }
            public string? Comment { get; set; }
        }


    }
}
