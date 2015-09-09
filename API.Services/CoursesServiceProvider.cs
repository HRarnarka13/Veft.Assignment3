using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using API.Models;
using API.Services.Repositories;
using API.Services.Exceptions;

namespace API.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CoursesServiceProvider
    {

        private readonly AppDataContext _db;

        public CoursesServiceProvider()
        {
            _db = new AppDataContext();
        }

        #region Course only related methods
        /// <summary>
        /// This method gets all courses
        /// </summary>
        /// <returns>A list of all courses</returns>
        public List<CourseDTO> GetCourses()
        {
            var courses = (from course in _db.Courses
                           join courseTemplate in _db.CourseTemplates on course.TemplateID equals courseTemplate.ID
                           select new CourseDTO
                           {
                               ID           = course.ID,
                               TemplateID   = courseTemplate.TemplateID,
                               Name         = courseTemplate.Name,
                               Semester     = course.Semester,
                               StartDate    = course.StartDate,
                               EndDate      = course.EndDate,
                               StudentCount = _db.StudentEnrollment.Count(x => x.CourseID == course.ID 
                                                                            && x.IsActive == true 
                                                                            && x.IsOnWaitingList == false)
                           }).ToList();
            return courses;
        }


        /// <summary>
        /// This method gets a single student with the provided id
        /// </summary>
        /// <param name="id">The id of the student</param>
        /// <returns>A course with the provided id</returns>
        public CourseDetailsDTO GetCourseByID(int id)
        {
            var course = (from c in _db.Courses
                          join ct in _db.CourseTemplates on c.TemplateID equals ct.ID
                          where c.ID == id
                          select new CourseDetailsDTO
                          {
                              ID           = c.ID,
                              TemplateID   = ct.TemplateID,
                              Name         = ct.Name,
                              Description  = ct.Description,
                              Semester     = c.Semester,
                              StartDate    = c.StartDate,
                              EndDate      = c.EndDate,
                              StudentCount = _db.StudentEnrollment.Where(x => x.CourseID == c.ID
                                                                            && x.IsOnWaitingList == false
                                                                            && x.IsActive == true).Count(),
                              Students     = (from sr in _db.StudentEnrollment
                                              join s in _db.Students on sr.StudentID equals s.ID
                                              where sr.CourseID == c.ID
                                              &&    sr.IsActive == true
                                              &&    sr.IsOnWaitingList == false
                                              select new StudentDTO
                                              {
                                                SSN  = s.SSN,
                                                Name = s.Name
                                              }).ToList()
                          }).SingleOrDefault();
            if (course == null)
            {
                throw new CourseNotFoundException();
            }
            return course;
        }
        /// <summary>
        /// This method adds a Course with the information from the CourseViewModel
        /// </summary>
        /// <param name="newCourse">Course containing all the information needed to add a new course</param>
        /// <returns></returns>
        public CourseDetailsDTO AddCourse(CourseViewModel newCourse)
        {
            // Check if the course exists
            var courseTemplate = _db.CourseTemplates.SingleOrDefault(x => x.TemplateID == newCourse.TemplateID);
            if (courseTemplate == null)
            {
                throw new TemplateCourseNotFoundException();
            }

            Entities.Course course = new Entities.Course
            {
                ID          = _db.Courses.Any() ? _db.Courses.Max(x => x.ID) + 1 : 1,
                TemplateID  = courseTemplate.ID,
                Semester    = newCourse.Semester,
                StartDate   = newCourse.StartDate,
                EndDate     = newCourse.EndDate,
                MaxStudents = newCourse.MaxStudents
            };
            _db.Courses.Add(course);

            _db.SaveChanges();

            return new CourseDetailsDTO
            {
                ID = course.ID,
                TemplateID = courseTemplate.TemplateID,
                Name = courseTemplate.Name,
                Description = courseTemplate.Description,
                Semester = course.Semester,
                StartDate = newCourse.StartDate,
                EndDate = newCourse.EndDate,
                StudentCount = _db.StudentEnrollment.Count(x => x.CourseID == course.ID
                                                             && x.IsActive == true
                                                             && x.IsOnWaitingList == false),
                Students = (from sr in _db.StudentEnrollment
                            join s in _db.Students on sr.StudentID equals s.ID
                            where sr.CourseID == course.ID
                            && sr.IsActive == true
                            && sr.IsOnWaitingList == false
                            select new StudentDTO
                            {
                                SSN = s.SSN,
                                Name = s.Name
                            }).ToList()
            };
        }
        /// <summary>
        /// Updates a course that already exists
        /// Note that this edits a Course, not a CourseTemplate.
        /// Only start and end date are editable
        /// </summary>
        /// <param name="courseID">The ID of the course to edit</param>
        /// <param name="updateCourse">a course with the information to edit</param>
        /// <returns></returns>
        public CourseDetailsDTO UpdateCourse(int courseID, UpdateCourseViewModel updateCourse)
        {
            // Get the course, throw exception if the course is not found
            Entities.Course course = _db.Courses.SingleOrDefault(x => x.ID == courseID);
            if (course == null)
            {
                throw new CourseNotFoundException();
            }

            // Check if the course tamplate exists
            var courseTemplate = _db.CourseTemplates.SingleOrDefault(x => x.ID == course.TemplateID);
            if (courseTemplate == null)
            {
                throw new TemplateCourseNotFoundException();
            }

            // Update the start and end date based on the view model
            course.StartDate = updateCourse.StartDate;
            course.EndDate = updateCourse.EndDate;
            course.MaxStudents = updateCourse.MaxStudents;

            // If all is successfull, we save our changes
            _db.SaveChanges();

            return new CourseDetailsDTO
            {
                ID           = course.ID,
                TemplateID   = courseTemplate.TemplateID,
                Name         = courseTemplate.Name,
                Description  = courseTemplate.Description,
                Semester     = course.Semester,
                StartDate    = updateCourse.StartDate,
                EndDate      = updateCourse.EndDate,
                StudentCount = _db.StudentEnrollment.Count(x => x.CourseID        == course.ID
                                                             && x.IsActive        == true
                                                             && x.IsOnWaitingList == false),
                Students = (from sr in _db.StudentEnrollment
                                join s in _db.Students on sr.StudentID equals s.ID
                                where sr.CourseID == course.ID
                                &&    sr.IsActive == true
                                &&    sr.IsOnWaitingList == false
                                select new StudentDTO
                                {
                                SSN = s.SSN,
                                    Name = s.Name
                                }).ToList()
            };
        }
        /// <summary>
        /// Deletes a course
        /// Note : This is a course not a course template.
        /// </summary>
        /// <param name="id">The ID of the course to delete</param>
        public void DeleteCourse(int id)
        {
            // Get the course
            Entities.Course course = _db.Courses.SingleOrDefault(x => x.ID == id);
            if (course == null)
            {
                throw new CourseNotFoundException();
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    // Remove all the students from the course
                    foreach (Entities.StudentEnrollment enrollment in _db.StudentEnrollment.Where(x => x.CourseID == course.ID))
                    {
                        _db.StudentEnrollment.Remove(enrollment);
                        _db.SaveChanges();
                    }
                    _db.Courses.Remove(course); // Remove the course
                    _db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new DeleteFromDatabaseException();
                }
            }
        }

        /// <summary>
        /// This method gets all the courses that are taught during the given semster.
        /// If no semester is given, then the default is the current semester.
        /// </summary>
        /// <param name="semester">The semester for the filter</param>
        /// <returns>A list of courses</returns>
        public List<CourseDTO> GetCoursesBySemester(string semester = null)
        {
            // Check if the semester is specified
            if (String.IsNullOrWhiteSpace(semester))
            {
                semester = "20153";
            }

            // Get all courses thought in the semester
            var courses = (from c in _db.Courses
                           join ct in _db.CourseTemplates on c.TemplateID equals ct.ID
                           where c.Semester == semester
                           select new CourseDTO
                           {
                               ID = c.ID,
                               TemplateID = ct.TemplateID,
                               Name = ct.Name,
                               Semester = c.Semester,
                               StartDate = c.StartDate,
                               EndDate = c.EndDate,
                               StudentCount = _db.StudentEnrollment.Count(x => x.CourseID == c.ID 
                                                                            && x.IsActive == true 
                                                                            && x.IsOnWaitingList == false)
                           }).ToList();

            return courses;
        }
        #endregion
        #region Course and Student related functions
        /// <summary>
        /// This method gets a list of students in the given course
        /// </summary>
        /// <param name="courseID">The id of the course</param>
        /// <returns>A list of students registered in the course</returns>
        public List<StudentDTO> GetStudentsInCourse(int courseID)
        {
            // Check if the course exists
            if (_db.Courses.SingleOrDefault(x => x.ID == courseID) == null)
            {
                throw new CourseNotFoundException();
            }

            // Get the students in the course
            List<StudentDTO> students = (from se in _db.StudentEnrollment
                                         join s in _db.Students on se.StudentID equals s.ID
                                         where se.CourseID == courseID
                                         &&    se.IsOnWaitingList == false 
                                         &&    se.IsActive == true
                                         select new StudentDTO
                                         {
                                             SSN = s.SSN,
                                             Name = s.Name
                                         }).ToList();
            return students;
        }

        /// <summary>
        /// This method adds a student to a given course
        /// </summary>
        /// <param name="courseID">The id of the course</param>
        /// <param name="newStudent">The student to be added</param>
        /// <returns>Details about the course, including a list of all students registerd in the course</returns>
        public StudentDTO AddStudentToCourse(int courseID, StudentViewModel newStudent)
        {
            // Check if the course exists
            var course = _db.Courses.SingleOrDefault(x => x.ID == courseID);
            var courseDetails = _db.CourseTemplates.SingleOrDefault(x => x.ID == course.TemplateID);
            if (course == null)
            {
                throw new CourseNotFoundException();
            }

            // Check if the student exists
            var student = _db.Students.SingleOrDefault(x => x.SSN == newStudent.SSN);
            if (student == null)
            {
                throw new StudentNotFoundException();
            }

            // Check if the course is full
            if (course.MaxStudents >= _db.StudentEnrollment.Count(x => x.CourseID == course.ID))
            {
                throw new CourseIsFullException();
            }

            
            // Check if the student is already registered in the course
            var studentEnrollment = _db.StudentEnrollment.SingleOrDefault(x => x.StudentID == student.ID && x.CourseID == course.ID);
            if (studentEnrollment == null) // Student has not registered for the course
            {
                _db.StudentEnrollment.Add(new Entities.StudentEnrollment
                {
                    StudentID       = student.ID,
                    CourseID        = course.ID,
                    IsOnWaitingList = false,
                    IsActive        = true
                });
            }
            else if (studentEnrollment.IsOnWaitingList == true) // Student is on the waitinglist
            {
                studentEnrollment.IsOnWaitingList = false;
                studentEnrollment.IsActive        = true;
            }
            else if (studentEnrollment.IsActive == false) // If the student enrollment has been deleted
            {
                studentEnrollment.IsActive = true;
            }
            else // Student is already in the course
            {
                throw new StudentAlreadyRegisteredInCourseException();
            }

            _db.SaveChanges();

            return new StudentDTO
            {
                SSN  = student.SSN,
                Name = student.Name
            };
        }
        
        /// <summary>
        /// Gets a student with the given SSN and that is registered in the given course
        /// </summary>
        /// <param name="courseID">The id of the course</param>
        /// <param name="newStudent">The student</param>
        /// <returns>The student</returns>
        public StudentDTO GetStudentInCourse(int courseID, string studentSSN)
        {
            Entities.Course course = _db.Courses.SingleOrDefault(x => x.ID == courseID);
            if (course == null) { throw new CourseNotFoundException(); }

            StudentDTO student = (from s in _db.Students
                                  join sr in _db.StudentEnrollment on s.ID equals sr.StudentID
                                  where sr.CourseID == course.ID 
                                  && s.SSN == studentSSN
                                  && sr.IsActive == true
                                  && sr.IsOnWaitingList == false   
                                  select new StudentDTO
                                  {
                                      SSN = s.SSN,
                                      Name = s.Name
                                  }).SingleOrDefault();

            if (student == null) { throw new StudentNotFoundException(); }
            return student;
        }

        #endregion
        /// <summary>
        /// This method returns a list of students registered on the waiting list for a give course
        /// </summary>
        /// <param name="id">The course id</param>
        /// <returns>A list of students of the waitinglist</returns>
        public List<StudentDTO> GetWaitinglistForACourse(int courseID)
        {
            var course = _db.Courses.SingleOrDefault(x => x.ID == courseID);
            if (course == null)
            {
                throw new CourseNotFoundException();
            }

            // Get list of students on the waiting list for the course
            List<StudentDTO> students = (from s in _db.Students
                                         join sr in _db.StudentEnrollment on s.ID equals sr.StudentID
                                         where sr.CourseID == course.ID
                                         && sr.IsOnWaitingList == true
                                         && sr.IsActive == true
                                         select new StudentDTO
                                         {
                                             Name = s.Name,
                                             SSN = s.SSN
                                         }).ToList();
            return students;
        }

        /// <summary>
        /// This method adds a given student to the waitinglist for a give course
        /// </summary>
        /// <param name="courseID">The course id</param>
        /// <param name="newStudent">The new student</param>
        /// <returns>The new student</returns>
        public StudentDTO AddStudentToWaitinglist(int courseID, StudentViewModel newStudent)
        {
            var course = _db.Courses.SingleOrDefault(x => x.ID == courseID);
            if (course == null)
            {
                throw new CourseNotFoundException();
            }

            var student = _db.Students.SingleOrDefault(x => x.SSN == newStudent.SSN);
            if (student == null)
            {
                throw new StudentNotFoundException();
            }

            var studentEnrollment = _db.StudentEnrollment.SingleOrDefault(x => x.StudentID == student.ID
                                                                            && x.CourseID == course.ID);
            if (studentEnrollment == null)
            {
                _db.StudentEnrollment.Add(new Entities.StudentEnrollment
                {
                    CourseID = course.ID,
                    StudentID = student.ID,
                    IsActive = true,
                    IsOnWaitingList = true
                });
            }
            else if (studentEnrollment.IsOnWaitingList == true || studentEnrollment.IsActive == true)
            {   
                // Check if the student is already on the waitinglist or registered in the course
                throw new StudentAlreadyRegisteredInCourseException();
            }
            else if (studentEnrollment.IsActive == false) // Check if the student as been removed
            {
                studentEnrollment.IsOnWaitingList = true;
                studentEnrollment.IsActive        = true;
            }

            _db.SaveChanges();

            return new StudentDTO
            {
                Name = student.Name,
                SSN  = student.SSN
            };
        }

        /// <summary>
        /// This method gets a given student on a waitinglist for a give course
        /// </summary>
        /// <param name="courseID">The course id</param>
        /// <param name="ssn">The ssn of the student</param>
        /// <returns></returns>
        public StudentDTO GetStudentOnWaitinglist(int courseID, string ssn)
        {
            var course = _db.Courses.SingleOrDefault(x => x.ID == courseID);
            if (course == null)
            {
                throw new CourseNotFoundException();
            }

            var student = _db.Students.SingleOrDefault(x => x.SSN == ssn);
            if (student == null)
            {
                throw new StudentNotFoundException();
            }

            var studentEnrollment = _db.StudentEnrollment.SingleOrDefault(x => x.CourseID == course.ID
                                                                            && x.StudentID == student.ID
                                                                            && x.IsOnWaitingList == true);
            if (studentEnrollment == null)
            {
                throw new StudentNotFoundException();
            }

            return new StudentDTO
            {
                Name = student.Name,
                SSN = student.SSN
            };

        }
    }
}
