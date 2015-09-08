using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    /// <summary>
    /// This class represents a instance of a Course
    /// </summary>
    public class CourseViewModel
    {
        /// <summary>
        /// The template ID of the course
        /// Example: T-514-VEFT
        /// </summary>
        [Required]
        public string CourseID { get; set; }

        /// <summary>
        /// The semester of the course
        /// Example: 20151
        /// </summary>
        [Required]
        public string Semester { get; set; }
        /// <summary>
        /// The start date of the course
        /// Example "2015-08-17"
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// The end date of the course
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
