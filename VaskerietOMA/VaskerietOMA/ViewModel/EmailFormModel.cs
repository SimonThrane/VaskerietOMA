using System.ComponentModel.DataAnnotations;
using System.Web;

namespace VaskerietOMA.ViewModel
{
    public class EmailFormModel
    {
        [Required, Display(Name = "Navn")]
        public string FromName { get; set; }
        [Required, Display(Name = "Email"), EmailAddress]
        public string FromEmail { get; set; }
        [Required, Display(Name = "Besked")]
        public string Message { get; set; }
        [Display(Name = "Maskine")]
        public Machine Machine { get; set; }
        public HttpPostedFileBase Upload { get; set; }
    }
}