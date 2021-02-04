namespace API.DTOs
{
    //the users that are liked, are displayed as cards, to get all the information from this Dto
    public class LikeDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public string PhotoUrl { get; set; }
        public string City { get; set; }
    }
}