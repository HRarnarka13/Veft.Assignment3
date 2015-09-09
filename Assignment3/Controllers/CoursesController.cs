using API.Models;
using API.Services;
using API.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Assignment3.Controllers
{
    [RoutePrefix("api/v1/courses")]
    public class CoursesController : ApiController
    {
        private static CoursesServiceProvider _service;

        public CoursesController()
        {
            _service = new CoursesServiceProvider();
        }

        #region Courses
        /// <summary>
        /// This method gets a list of avalible courses
        /// </summary>
        /// <returns>A list of all courses</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetCourses()
        {
            return Ok(_service.GetCourses());
        }

        /// <summary>
        /// This method addes a new course the list of courses
        /// </summary>
        /// <param name="newCourse">The new course</param>
        /// <returns>The location of the new course</returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult AddCourse(CourseViewModel newCourse)
        {
            if (!ModelState.IsValid) { throw new HttpResponseException(HttpStatusCode.PreconditionFailed); }

            try
            {
                CourseDetailsDTO course = _service.AddCourse(newCourse); // may throw exception
                var location = Url.Link("GetCourse", new { id = course.ID });
                return Created(location, course);
            }
            catch (TemplateCourseNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.PreconditionFailed);
            }

        }

        /// <summary>
        /// This method gets a list of courses taught during the given semster
        /// </summary>
        /// <param name="semester">The semester, example: "20153"</param>
        /// <returns>A list of courses that are taught on the semster given</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetCoursesBySemester(string semester)
        {
            return Ok(_service.GetCoursesBySemester(semester));
        }
        #endregion

        #region Courses/{id}
        /// <summary>
        /// This method gets the course with the given id.
        /// It whould be better to use filter to handle these exceptions but it is
        /// required in this assigment.
        /// </summary>
        /// <param name="id">id of the course</param>
        /// <returns>a single course</returns>
        [HttpGet]
        [Route("{id:int}", Name = "GetCourse")]
        public IHttpActionResult _GetCourseById(int id)
        {
            try
            {
                return Ok(_service.GetCourseByID(id));
            }
            catch (CourseNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// This method updates the given course properties for a single given course
        /// </summary>
        /// <param name="editedCourse">The edited course</param>
        /// <returns>200 if successful</returns>
        [HttpPut]
        [Route("{id:int}")]
        [ResponseType(typeof(UpdateCourseViewModel))]
        public IHttpActionResult UpdateCourse(int id, UpdateCourseViewModel editedCourse)
        {
            if (!ModelState.IsValid || editedCourse.StartDate.Equals(DateTime.MinValue) || editedCourse.EndDate.Equals(DateTime.MinValue))
            {
                throw new HttpResponseException(HttpStatusCode.PreconditionFailed);
            }

            try
            {
                CourseDetailsDTO course = _service.UpdateCourse(id, editedCourse);
                var location = Url.Link("GetCourse", new { id = course.ID });
                return Created(location, course);
            }
            catch (CourseNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// This method deletes the course with the given id   
        /// </summary>
        /// <param name="id">The id of the course</param>
        [HttpDelete]
        [Route("{id:int}", Name = "DeleteCourse")]
        public void DeleteCourse(int id)
        {
            try
            {
                _service.DeleteCourse(id); // this may throw a not found exception
            }
            catch (CourseNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            catch (DeleteFromDatabaseException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Courses/{id}/students
        /// <summary>
        /// Get a list of all the students in a given course
        /// </summary>
        /// <param name="id">id of the course</param>
        /// <returns>List of students in the course</returns>
        [HttpGet]
        [Route("{id:int}/students", Name = "GetStudentsInCourse")]
        public IHttpActionResult GetStudentsInCourse(int id)
        {
            try
            {
                List<StudentDTO> students = _service.GetStudentsInCourse(id); // this may throw a not found exception
                return Ok(students);
            }
            catch (CourseNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// Adds a student to a given course
        /// </summary>
        /// <param name="id">id of the course</param>
        /// <param name="newStudent">The student to be added</param>
        /// <returns>List of the students in the course with the added student</returns>
        [HttpPost]
        [Route("{id:int}/students", Name = "AddStudentToCourse")]
        public IHttpActionResult AddStudentToCourse(int id, StudentViewModel newStudent)
        {
            if (!ModelState.IsValid) { throw new HttpResponseException(HttpStatusCode.PreconditionFailed); }

            try
            {
                StudentDTO student = _service.AddStudentToCourse(id, newStudent);
                var location = Url.Link("GetStudentInCourse", new { id = id, ssn = student.SSN });
                return Created(location, student);
            }
            catch (CourseNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            catch (StudentNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            catch (StudentAlreadyRegisteredInCourseException)
            {
                throw new HttpResponseException(HttpStatusCode.PreconditionFailed);
            }
            catch (CourseIsFullException)
            {
                throw new HttpResponseException(HttpStatusCode.PreconditionFailed);
            }
        }
        #endregion

        #region Courses/{id}/students/{ssn}
        /// <summary>
        /// This method gets the given student in a given course
        /// </summary>
        /// <param name="id">The id of the course</param>
        /// <param name="ssn">The SSN of the student</param>
        /// <returns>The student</returns>
        [HttpGet]
        [Route("{id:int}/students/{ssn}", Name = "GetStudentInCourse")]
        public IHttpActionResult GetStudentInCourse(int id, string ssn)
        {
            try
            {
                return Ok(_service.GetStudentInCourse(id, ssn));
            }
            catch (CourseNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            catch (StudentNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }
        #endregion

        #region Courses/{id}/waitinglist
        /// <summary>
        /// This method returns a list of students registered on the waiting list for a give course
        /// </summary>
        /// <param name="id">The course id</param>
        /// <returns>A list of students of the waitinglist</returns>
        [HttpGet]
        [Route("Courses/{id:int}/waitinglist")]
        public IHttpActionResult GetWaitinglistForACourse(int id)
        {
            return Ok(_service.GetWaitinglistForACourse(id));
        }

        #endregion
    }
}
