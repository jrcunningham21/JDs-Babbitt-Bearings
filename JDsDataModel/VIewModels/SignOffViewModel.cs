using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsDataModel.ViewModels
{
    public class SignOffViewModel
    {
        [Display(Name ="PIN #")]
        public string Pin { get; set; }
        public string SkillName { get; set; }
        public int SignOffId { get; set; }
    }
}
