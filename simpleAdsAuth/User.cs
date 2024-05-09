namespace simpleAdsAuth
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        //public List<Ad> Ads { get; set; }
    }
    public class Ad
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public string Details { get; set; }
        public int UserId { get; set; }

    }
}
