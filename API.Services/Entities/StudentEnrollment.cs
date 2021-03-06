﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Entities
{
    /// <summary>
    /// This class represents a student and course relationship in the database
    /// </summary>
    [Table("StudentEnrollments")]
    class StudentEnrollment
    {
        /// <summary>
        /// The Id of the enrollment
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The ID of a student in the course
        /// </summary>
        public int StudentID { get; set; }
        /// <summary>
        /// The ID of the course the student is enrolled in
        /// </summary>
        public int CourseID { get; set; }

        /// <summary>
        /// Flag to indicate if a student is on the waiting list for that course
        /// Note: This is not a scalible and eligant solution but is fine for this assignment
        /// </summary>
        public Boolean IsOnWaitingList { get; set; }

        /// <summary>
        /// Flag to indicate if a student is active in a course
        /// Note: This is not a scalible and eligant solution but is fine for this assignment
        /// </summary>
        public Boolean IsActive { get; set; }
    }
}
