using System;
using System.Text.Json.Serialization;

namespace API.DTOs
{
    public class MessageDto
    {
         //Sender /expeditor
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderPhotoUrl { get; set; }
       [JsonIgnore]
        public bool SenderDeleted { get; set; }


        //Recipient /destinatar
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string RecipientPhotoUrl { get; set; }
        [JsonIgnore]
        public bool RecipientDeleted { get; set; }
        

        //About the messages
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.Now; 
    }
}