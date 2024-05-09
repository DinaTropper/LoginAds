namespace simpleAdsAuth.Web.Models
{
    public class AdsViewModel
    {
        public List<Ad> Ads { get; set; }
        public List<User> Users { get; set; }
        public User User { get; set; }
        public string Message { get; set; }
        public bool showDeletes { get; set; }
    }
}
