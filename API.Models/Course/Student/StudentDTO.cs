using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    /// <summary>
    /// This class repersents a single student in Reykjavik University
    /// </summary>
    public class StudentDTO
    {
        /// <summary>
        /// The SSN for a single student
        /// Example : "0203932331"
        /// </summary>
        public string SSN { get; set; }

        /// <summary>
        /// The name of a single student
        /// Example : "John Jenson"
        /// </summary>
        public string Name { get; set; }
    }
}
