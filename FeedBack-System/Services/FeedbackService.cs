using FeedBack_System.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeedBack_System.Services
{
    public class FeedbackService
    {
        private readonly IMongoCollection<Feedback> _feedbackCollection;

        public FeedbackService(IMongoCollection<Feedback> feedbackCollection)
        {
            _feedbackCollection = feedbackCollection;
        }

        public async Task<List<Feedback>> GetFeedbacksByUserAsync(string userId) =>
           await _feedbackCollection.Find(feedback => feedback.UserId == userId).ToListAsync();


    public async Task<List<Feedback>> GetAllFeedbacksAsync() =>
            await _feedbackCollection.Find(_ => true).ToListAsync();


        public async Task CreateFeedbackAsync(Feedback feedback)
        {
            feedback.FeedbackId = GenerateUniqueFeedbackId();
            await _feedbackCollection.InsertOneAsync(feedback);
        }

        private string GenerateUniqueFeedbackId()
        {
            Random random = new Random();
            return random.Next(1000, 9999).ToString(); // Generates a 4-digit ID
        }

        public async Task<UpdateResult> UpdateFeedbackCommentByIdAsync(string feedbackId, string comment)
        {
            var filter = Builders<Feedback>.Filter.Eq(f => f.FeedbackId, feedbackId);
            var update = Builders<Feedback>.Update.Set(f => f.Comments, comment);
            return await _feedbackCollection.UpdateOneAsync(filter, update);
        }



    }
}
