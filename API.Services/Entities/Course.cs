using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Entities
{
    /// <summary>
    /// This class represents the course table in the database
    /// </summary>
    [Table("Courses")]
    class Course
    {
        /// <summary>
        /// A unique identifier for the course
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// The template id for a course. Foreign key to the CourseTemplate table
        /// Note : not the actual TemplateID, just a key joining the tables.
        /// </summary>
        public int TemplateID { get; set; }
        /// <summary>
        /// The start date for a course
        /// Example "2015-08-17"
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// The end date for a course
        /// Example "2015-11-08"
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// The semester and year the course is taught in
        /// Example "201501"
        /// </summary>
        public string Semester { get; set; }
    }
}
