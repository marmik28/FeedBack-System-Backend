using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FeedBack_System.Models
{
    public class Feedback
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("feedbackId")]
        public string? FeedbackId { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("category")]
        public string? Category { get; set; }

        [BsonElement("message")]
        public string? Message { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "In Progress";

        [BsonElement("comments")]
        public string? Comments { get; set; }
    }
}
