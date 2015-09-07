using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Entities
{
    /// <summary>
    /// This class represents a student template in the database
    /// </summary>
    [Table("Students")]
    class Student
    {
        /// <summary>
        /// A unique identifier for the student
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// The name of the student
        /// Example "Daniel Freysteinn Sverrirsson"
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The SSN of the student
        /// Example "2104923489"
        /// </summary>
        public string SSN { get; set; }
    }
}
