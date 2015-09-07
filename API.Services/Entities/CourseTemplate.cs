using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Entities
{
    /// <summary>
    /// This class represents the CourseTemplates table in the database
    /// </summary>
    [Table("CourseTemplates")]
    class CourseTemplate
    {
        /// <summary>
        /// The ID of the course
        /// Example: 1
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The template id for the course. This is always unique
        /// Example: "T-514-VEFT"
        /// </summary>
        public string TemplateID { get; set; }

        /// <summary>
        /// The name of the course
        /// Example: "Web services"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description for the course
        /// </summary>
        public string Description { get; set; }
    }
}
