using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AnnotationsAPI.Context;
using AnnotationsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using AnnotationsAPI.DTOs;
using AnnotationsAPI.CustomExceptions;

namespace AnnotationsAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/annotation")]
    public class AnnotationController : ControllerBase
    {
        private readonly ApplicationContext _applicationContext;
        private readonly UserManager<UserAplication> _userManager;

        public AnnotationController(ApplicationContext applicationContext, UserManager<UserAplication> userManager)
        {
            _applicationContext = applicationContext;
            _userManager = userManager;
        }

        /// <summary> Get all annotations. </summary>
        /// <returns> Returns a annotations page </returns>
        [HttpGet]
        public ActionResult<PageResponse<List<Annotation>>> GetAllAnnotations([FromQuery] Pagination pagination)
        {
            try
            {
                var userId = User?.Identity?.Name;

                var validPagination = new Pagination(pagination.Page, pagination.Size);

                var annotations = _applicationContext.Annotations
                    .Where(annotation => annotation.UserAplicationId == userId)
                    .Skip((validPagination.Page - 1) * validPagination.Size)
                    .Take(validPagination.Size)
                    .ToList();

                int totalRecords = _applicationContext.Annotations
                   .Where(annotation => annotation.UserAplicationId == userId)
                   .Count();

                string baseUri = $"{Request.Scheme}://{Request.Host}/api/annotation";

                PageResponse<List<Annotation>> pageResponse = new(
                    data: annotations,
                    page: validPagination.Page,
                    size: validPagination.Size,
                    totalRecords: totalRecords,
                    uri: baseUri
                );

                return Ok(pageResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error geting annotations!" });
            }
        }

        /// <summary> Find annotation by title. </summary>
        /// <returns> Returns a annotations page </returns>
        [HttpGet("find-by-title/{title}")]
        public ActionResult<PageResponse<List<Annotation>>> FindAnnotationByTitle([FromQuery] Pagination pagination, string title)
        {
            try
            {
                var userId = User?.Identity?.Name;

                var validPagination = new Pagination(pagination.Page, pagination.Size);

                var annotations = _applicationContext.Annotations
                    .Where(annotation => annotation.Title.Contains(title) && annotation.UserAplicationId == userId)
                    .Skip((validPagination.Page - 1) * validPagination.Size)
                    .Take(validPagination.Size)
                    .ToList();

                var TotalRecords = _applicationContext.Annotations
                    .Where(annotation => annotation.Title.Contains(title) && annotation.UserAplicationId == userId)
                    .Count();

                string baseUri = $"{Request.Scheme}://{Request.Host}/api/annotation/find-by-title/{title}";

                PageResponse<List<Annotation>> pageResponse = new(
                    data: annotations,
                    page: validPagination.Page,
                    size: validPagination.Size,
                    totalRecords: TotalRecords,
                    uri: baseUri
                );

                return Ok(pageResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error finding annotation!" });
            }
        }

        /// <summary> Find annotation by description. </summary>
        /// <returns> Returns a annotations page </returns>
        [HttpGet("find-by-description/{description}")]
        public ActionResult<PageResponse<List<Annotation>>> FindAnnotationByDescription([FromQuery] Pagination pagination, string description)
        {
            try
            {
                var userId = User?.Identity?.Name;

                var validPagination = new Pagination(pagination.Page, pagination.Size);

                var annotations = _applicationContext.Annotations
                    .Where(annotation => annotation.Description.Contains(description) && annotation.UserAplicationId == userId)
                    .Skip((validPagination.Page - 1) * validPagination.Size)
                    .Take(validPagination.Size)
                    .ToList();

                var TotalRecords = _applicationContext.Annotations
                    .Where(annotation => annotation.Description.Contains(description) && annotation.UserAplicationId == userId)
                    .Count();

                string baseUri = $"{Request.Scheme}://{Request.Host}/api/annotation/find-by-description/{description}";

                PageResponse<List<Annotation>> pageResponse = new(
                    data: annotations,
                    page: validPagination.Page,
                    size: validPagination.Size,
                    totalRecords: TotalRecords,
                    uri: baseUri
                );

                return Ok(pageResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error finding annotation!" });
            }
        }

        /// <summary> Get important annotations. </summary>
        /// <returns> Returns a annotations page </returns>
        [HttpGet("important-annotations")]
        public ActionResult<PageResponse<List<Annotation>>> GetImportantAnnotations([FromQuery] Pagination pagination)
        {
            try
            {
                return Ok(GetImportantOrUnimportantAnnotations(true, "not-important-annotations", pagination));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error geting annotation!" });
            }
        }

        /// <summary> Get unimportant annotations. </summary>
        /// <returns> Returns a annotations page </returns>
        [HttpGet("not-important-annotations")]
        public ActionResult<PageResponse<List<Annotation>>> GetNotImportantAnnotations([FromQuery] Pagination pagination)
        {
            try
            {
                return Ok(GetImportantOrUnimportantAnnotations(false, "not-important-annotations", pagination));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error geting annotations!" });
            }
        }

        private PageResponse<List<Annotation>> GetImportantOrUnimportantAnnotations(bool IsImportant, string endpoint, Pagination pagination)
        {
            var userId = User?.Identity?.Name;

            var valiPagination = new Pagination(pagination.Page, pagination.Size);

            var annotations = _applicationContext.Annotations
                                .Where(annotation => annotation.IsImportant == IsImportant && annotation.UserAplicationId == userId)
                                .Skip((valiPagination.Page - 1) * valiPagination.Size)
                                .Take(valiPagination.Size)
                                .ToList();

            var totalRecords = _applicationContext.Annotations
                                .Where(annotation => annotation.IsImportant == IsImportant && annotation.UserAplicationId == userId)
                                .Count();

            string baseUri = $"{Request.Scheme}://{Request.Host}/api/annotation/{endpoint}";

            PageResponse<List<Annotation>> pageResponse = new(
              data: annotations,
              page: valiPagination.Page,
              size: valiPagination.Size,
              totalRecords: totalRecords,
              uri: baseUri
             );

            return pageResponse;
        }

        /// <summary> Get a annotation by id. </summary>
        /// <returns> Returns a annotation </returns>
        /// <response code="404"> If annotation not exist </response>
        [HttpGet("{id}")]
        public ActionResult<Annotation> GetAnnotationById(int id)
        {
            try
            {
                var userId = User?.Identity?.Name;
                var annotationDataBase = _applicationContext.Annotations.Where(annotation => annotation.Id == id && annotation.UserAplicationId == userId).FirstOrDefault();

                if (annotationDataBase == null)
                {
                    return NotFound(new { Message = "Annotation not found!" });
                }

                return Ok(annotationDataBase);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error geting annotation!" });
            }

        }

        /// <summary> Create a annotation. </summary>
        /// <returns> A newly created annotation </returns>
        /// <response code="201"> Returns the newly created annotation </response>
        /// <response code="400"> If any fields are missing or invalid </response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Annotation> PostAnnotation(AnnotationDto annotationDto)
        {
            try
            {
                var userId = User?.Identity?.Name;

                if (annotationDto.Description == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Description is required!" });
                }

                Annotation annotation = new()
                {
                    Title = annotationDto.Title,
                    Description = annotationDto.Description,
                    IsImportant = annotationDto.IsImportant.HasValue ? annotationDto.IsImportant.Value : false,
                    UserAplicationId = userId
                };

                _applicationContext.Annotations.Add(annotation);
                _applicationContext.SaveChanges();

                return CreatedAtAction(nameof(GetAnnotationById), new { id = annotation.Id }, annotation);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error creating annotation!" });
            }

        }

        /* Function to update annotation, this function is used by PUT and PATCH */
        private Annotation UpdateAnnotation(int annotationId, AnnotationDto annotationDto)
        {
            var userId = User?.Identity?.Name;
            var annotation = _applicationContext.Annotations.Where(annotation => annotation.Id == annotationId && annotation.UserAplicationId == userId).FirstOrDefault();

            if (annotation == null)
            {
                throw new NotFoundException(message: $"Annotation of id {annotationId} is not found");
            }

            if (!string.IsNullOrEmpty(annotationDto.Title))
            {
                annotation.Title = annotationDto.Title;
            }

            if (!string.IsNullOrEmpty(annotationDto.Description))
            {
                annotation.Description = annotationDto.Description;
            }

            if (annotationDto.IsImportant.HasValue)
            {
                annotation.IsImportant = annotationDto.IsImportant.Value;
            }

            _applicationContext.Annotations.Update(annotation);
            _applicationContext.SaveChanges();

            return annotation;
        }

        /// <summary> Fully update a annotation </summary>
        /// <returns> Returns the updated annotation </returns>
        /// <response code="404"> If annotation not exist </response>
        [HttpPut("{id}")]
        public ActionResult<Annotation> PutAnnotation(int id, AnnotationDto annotationDto)
        {
            try
            {
                if (annotationDto.Title == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Title is required!" });
                }

                if (annotationDto.Description == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Description is required!" });
                }

                if (!annotationDto.IsImportant.HasValue)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Is Important is required!" });
                }

                var annotation = UpdateAnnotation(id, annotationDto);

                return Ok(annotation);
            }
            catch (NotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { ex.Message });
            }
            catch (BadHttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error updating annotation!" });
            }
        }

        /// <summary> Partially update a annotation </summary>
        /// <returns> Returns the updated annotation </returns>
        /// <response code="404"> If annotation not exist </response>
        [HttpPatch("{id}")]
        public ActionResult<Annotation> PatchAnnotation(int id, AnnotationDto annotationDto)
        {
            try
            {
                var annotation = UpdateAnnotation(id, annotationDto);

                return Ok(annotation);
            }
            catch (NotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { ex.Message });
            }
            catch (BadHttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error updating annotation!" });
            }
        }

        /// <summary> Deletes a specific annotation. </summary>
        /// <response code="204"> If annotation deleted success </response>
        /// <response code="404"> If annotation not exist </response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteAnnotation(int id)
        {
            try
            {
                var userId = User?.Identity?.Name;
                var annotation = _applicationContext.Annotations.Where(annotation => annotation.Id == id && annotation.UserAplicationId == userId).FirstOrDefault();

                if (annotation == null)
                {
                    return NotFound(new { Message = "annotation not found!" });
                }

                _applicationContext.Annotations.Remove(annotation);
                _applicationContext.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error deleting annotation!" });
            }
        }

    }
}