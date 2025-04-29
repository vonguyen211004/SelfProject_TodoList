using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace TodoList.Models
{
    public class Todo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("Title")]
        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("Description")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("IsDone")]
        [Display(Name = "Hoàn thành")]
        public bool IsDone { get; set; }

        [BsonElement("CreatedAt")]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("DueDate")]
        [Display(Name = "Hạn chót")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; } = string.Empty;
    }
}