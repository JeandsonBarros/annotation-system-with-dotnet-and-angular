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
        [Authorize]
        [HttpGet]
        public ActionResult<PageResponse<List<Annotation>>> GetAllAnnotations([FromQuery] Pagination pagination)
        {
            try
            {
                var userId = User?.Identity?.Name;

                var validPagination = new Pagination(pagination.Page, pagination.Size);

                var annotations = _applicationContext.Annotation
                    .Where(annotation => annotation.UserAplicationId == userId)
                    .Skip((validPagination.Page - 1) * validPagination.Size)
                    .Take(validPagination.Size)
                    .ToList();

                var TotalRecords = _applicationContext.Annotation.Count();

                string baseUri = $"{Request.Scheme}://{Request.Host}/api/annotation";

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
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error geting annotations!" });
            }
        }

        /// <summary> Find annotation by title. </summary>
        /// <returns> Returns a annotations page </returns>
        [Authorize]
        [HttpGet("find-by-title/{title}")]
        public ActionResult<PageResponse<List<Annotation>>> GetAnnotationByTitle([FromQuery] Pagination pagination, string title)
        {
            try
            {
                var userId = User?.Identity?.Name;

                var validPagination = new Pagination(pagination.Page, pagination.Size);

                var annotations = _applicationContext.Annotation
                    .Where(annotation => annotation.Title.Contains(title) && annotation.UserAplicationId == userId)
                    .Skip((validPagination.Page - 1) * validPagination.Size)
                    .Take(validPagination.Size)
                    .ToList();

                var TotalRecords = _applicationContext.Annotation
                    .Where(annotation => annotation.Title
                    .Contains(title) && annotation.UserAplicationId == userId)
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
        [Authorize]
        [HttpGet("find-by-description/{description}")]
        public ActionResult<PageResponse<List<Annotation>>> GetAnnotationByDescription([FromQuery] Pagination pagination, string description)
        {
            try
            {
                var userId = User?.Identity?.Name;

                var validPagination = new Pagination(pagination.Page, pagination.Size);

                var annotations = _applicationContext.Annotation
                    .Where(annotation => annotation.Description.Contains(description) && annotation.UserAplicationId == userId)
                    .Skip((validPagination.Page - 1) * validPagination.Size)
                    .Take(validPagination.Size)
                    .ToList();

                var TotalRecords = _applicationContext.Annotation
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

        /// <summary> Get a annotation by id. </summary>
        /// <returns> Returns a annotation </returns>
        /// <response code="404"> If annotation not exist </response>
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<Annotation> GetAnnotationById(int id)
        {
            try
            {
                var userId = User?.Identity?.Name;
                var annotationDataBase = _applicationContext.Annotation.Where(annotation => annotation.Id == id && annotation.UserAplicationId == userId).FirstOrDefault();

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
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Annotation> PostAnnotation(AnnotationDto annotationDto)
        {
            try
            {
                var userId = User?.Identity?.Name;

                Annotation annotation = new()
                {
                    Title = annotationDto.Title,
                    Description = annotationDto.Description,
                    UserAplicationId = userId
                };

                _applicationContext.Annotation.Add(annotation);
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
        private Annotation UpdateAnnotation(int annotationId, AnnotationDtoViewModel annotationDtoViewModel)
        {
            var userId = User?.Identity?.Name;
            var annotation = _applicationContext.Annotation.Where(annotation => annotation.Id == annotationId && annotation.UserAplicationId == userId).FirstOrDefault();

            if (annotation == null)
            {
                throw new NotFoundException(message: $"Annotation of id {annotationId} is not found");
            }

            if (!string.IsNullOrEmpty(annotationDtoViewModel.Title))
            {
                annotation.Title = annotationDtoViewModel.Title;
            }

            if (!string.IsNullOrEmpty(annotationDtoViewModel.Description))
            {
                annotation.Description = annotationDtoViewModel.Description;
            }

            _applicationContext.Annotation.Update(annotation);
            _applicationContext.SaveChanges();

            return annotation;
        }

        /// <summary> Fully update a annotation </summary>
        /// <returns> Returns the updated annotation </returns>
        /// <response code="404"> If annotation not exist </response>
        [HttpPut("{id}")]
        public ActionResult<Annotation> PutAnnotation(int id, AnnotationDto annotationtDto)
        {
            try
            {
                AnnotationDtoViewModel annotationDtoViewModel = new()
                {
                    Title = annotationtDto.Title,
                    Description = annotationtDto.Description,
                };

                var annotation = UpdateAnnotation(id, annotationDtoViewModel);

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
        public ActionResult<Annotation> PatchAnnotation(int id, AnnotationDtoViewModel annotationDtoViewModel)
        {
            try
            {
                var annotation = UpdateAnnotation(id, annotationDtoViewModel);

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
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteAnnotation(int id)
        {
            try
            {
                var userId = User?.Identity?.Name;
                var annotation = _applicationContext.Annotation.Where(annotation => annotation.Id == id && annotation.UserAplicationId == userId).FirstOrDefault();

                if (annotation == null)
                {
                    return NotFound(new { Message = "annotation not found!" });
                }

                _applicationContext.Annotation.Remove(annotation);
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