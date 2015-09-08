using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    /// <summary>
    /// This class represents what is allowed to be changed by a user
    /// </summary>
    public class UpdateCourseViewModel
    {
        /// <summary>
        /// The start date of a course
        /// Example "2015=08=17"
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// The start date of a course
        /// Example "2015-11-15"
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The maximum students allowed to be registerd in the course
        /// </summary>
        [Required]
        public int MaxStudents { get; set; }
    }
}
