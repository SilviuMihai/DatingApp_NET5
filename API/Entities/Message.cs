using System;

namespace API.Entities
{
    public class Message
    {
        //Sender /expeditor
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public AppUser Sender { get; set; }


        //Recipient /destinatar
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public AppUser Recipient { get; set; }
        

        //About the messages
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;
        public bool SenderDeleted { get; set; } //to delete the message - sender side
        public bool RecipientDeleted { get; set; } //to delete the message - recipient side 
    }
}