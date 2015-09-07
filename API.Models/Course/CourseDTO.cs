using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{

    /// <summary>
    /// This class represents an item in a list of courses
    /// </summary>
    public class CourseDTO
    {
        /// <summary>
        /// Repersents the name of a single course
        /// Examble: "Introduction to machine learning"
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Repersents the template id of a single course
        /// Examble: T-504-ITML
        /// </summary>
        public string TemplateID { get; set; }

        /// <summary>
        /// Repersents nothing, just an id for a single course
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The semester of the course
        /// Example: 20151
        /// </summary>
        public string Semester { get; set; }

        /// <summary>
        /// Repersents the start date for a single course
        /// Example "2015-11-17"
        /// </summary>
        public DateTime StartDate { get; set; }


        /// <summary>
        /// Repersents the end date for a single course
        /// Example "2015-08-17"
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The number of students in the course.
        /// </summary>
        public int StudentCount { get; set; }
    }
}
