using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    /// <summary>
    /// This class represents a student when the client is sends a student to the server
    /// </summary>
    public class StudentViewModel
    {
        /// <summary>
        /// The SSN of a student
        /// Example: "2104923489"
        /// </summary>
        public string SSN { get; set; }
    }
}
